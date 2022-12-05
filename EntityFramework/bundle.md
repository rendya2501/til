# bundle

## bundle概要

バンドルは移行を実行するために必要な全ての情報を内包した実行ファイル。
CLI等のツールは依存関係（.NET SDK、プログラム、移行ファイル、ツール自体等）を整える必要があるが、バンドルはそれらを内包しているので、環境に左右されず、単体で移行を行うことができる。

- 移行バンドルは、移行を実行するために必要なすべてのものを含む自己充足的な実行ファイル(exe)  
- バンドルは実行環境に影響を受けずにマイグレーションを実行することができる。  
  - CLIはツールの依存関係（.NET SDK、モデルをコンパイルするためのソースコード、ツール自体）が本番サーバーにインストールされている必要がある。  

- 移行バンドルはコマンドラインインターフェースと同じアクションを実行する。  
- 実質的にdotnet ef database updateと同等の動作を提供する。  

- 主要なツール (Docker、SSH、PowerShell 等) で動作する。  
- EFCore6.0(.Net6)からの機能  

- 移行を実行するだけの機能なので、細かい制御はできない。  

>移行バンドルは、データベースに移行を適用するために使用できる単一ファイルの実行可能ファイルです。  
>これらは、次のような SQL スクリプトとコマンドライン ツールの欠点の一部に対処します。  
>
>- SQL スクリプトを実行するには、追加のツールが必要です。  
>- これらのツールによるトランザクション処理とエラー時の続行動作には一貫性がなく、予期できない場合もあります。  
>  そのため、移行の適用時にエラーが発生した場合に、データベースが未定義の状態になる可能性があります。  
>- バンドルは、CI プロセスの一部として生成することができ、後で配置プロセスの一部として簡単に実行することができます。  
>- バンドルは、.NET SDK または EF ツールを (または、自己完結型の場合は .NET ランタイムさえも) インストールせずに実行でき、プロジェクトのソース コードは必要ありません。  
>[移行の適用](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli#bundles)  

<!--  -->
>生成される実行可能ファイルは、既定では efbundle という名前になります。  
>これは、データベースを最新の移行に更新するために使用できます。  
>これは、dotnet ef database update または Update-Database を実行するのと同等です。  
>[移行の適用](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli#efbundle)

---

## バンドルの作成

`dotnet ef migrations bundle`コマンドを実行することでバンドルを作成することができる。  

しかし、色々と前提条件がある。  

基本的にバンドルはコンソールアプリやWebアプリ等、単独実行可能なプロジェクトであれば作成可能。  
クラスライブラリからでも発行可能らしいが、そこまで検証していない。  

bundle生成において重要なのはスタートアッププログラムにおいてHostingを行う事。  
Webプロジェクトは最初からHostingを行う構成になっているので、必要なパッケージは少なく済む。  
コンソールアプリでも可能だが、Webプロジェクト以上に必要な操作が多い。  

>ASP.NET Core 2.2 アプリで dotnet ef コマンドを実行する場合は、 Program.cs に CreateWebHostBuilder メソッドが必要な模様。  
[dotnet ef migrations でエラーになった話](https://qiita.com/wukann/items/53462f4b21104ed75c31)  

上記サイトで紹介されている通り、Hosting関係のメソッドを使って作成している様なので、Hostingが必要であ る。  

### 作成できた例  

コンソールアプリの場合は`Microsoft.Extensions.Hosting`パッケージをnugetからインストールする必要がある。  

appsettings.jsonを読み取る場合は次のようになる。

``` cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddDbContext<DbContext>(options =>
            {
                var appsettings = hostContext.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(appsettings);
            });
    })
    .Build();
// Runまでせずともbundleを作成する事ができた。  
// host.Run();
```

作成するだけなら接続文字列も必要ないので以下のように記述することができる。  

``` cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => services.AddDbContext<DbContext>(options => options.UseSqlServer()))
    .Build();
```

Webアプリではサービスに登録することで作成可能となる。  
今回の例では、空のWebアプリとするが、基本的にどのWebアプリであってもスタートアップでサービスを登録すればよい。  
Webアプリでは最初からホスティングが保証されているらしいので、`Microsoft.Extensions.Hosting`は必要ない。  

``` cs
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DbContext>(options =>options.UseSqlServer());
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
```

作成だけのサンプルなので、ジェネリックはDbContextとしているが、本来であればDbContextを継承した対象のContextとする事。  

### 作成できなかった例  

コンソールアプリでの検証。  
ContextクラスにDIするだけではダメだった。  

``` cs
var services = new ServiceCollection();
services.AddDbContext<DatContext>(options => options.UseSqlServer(connectionString));
ServiceProvider serviceProvider = services.BuildServiceProvider();
_datContext = serviceProvider.GetService<DatContext>();
```

バンドルは作成できないが、`_datContext.Database.Migration();`とすればマイグレーション可能ではある。  

- 参考  
  - [デザイン時 DbContext 作成](https://learn.microsoft.com/ja-jp/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli)  
  - [Accessing dbContext in a C# console application](https://stackoverflow.com/questions/49972591/accessing-dbcontext-in-a-c-sharp-console-application)  
  - [How to Add Entity Framework Core DBContext in .NET Core Console Application](http://www.techtutorhub.com/article/How-to-Add-Entity-Framework-Core-DBContext-in-Dot-NET-Core-Console-Application/86)  

- 検索文字列 : dependency injection dbcontext console app  

---

## bundle作成の最小実装

とりあえず、バンドルを生成するためだけの一連の流れ。  
実行しても何もしないバンドルが生成される。  

- 開発環境  
  - Windows10  
  - .Net 6  
  - gitbash or CommandPrompt  
  - vscode or gitbash_vim  

### webプロジェクトで作成する場合

プロジェクト作成  
`dotnet new web`  

dotnet-efツールのローカルインストール  
`dotnet new tool-manifest`  
`dotnet tool install dotnet-ef`  

NuGetパッケージのインストール  
`dotnet add package Microsoft.EntityFrameworkCore`  

Program.csを以下のように書き換える  

- `using Microsoft.EntityFrameworkCore;`の追加
- `builder.Services.AddDbContext<DbContext>(options =>options.UseSqlServer());`を追加

``` cs
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DbContext>(options =>options.UseSqlServer());
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
```

バンドル発行  
`dotnet ef migrations bundle`

### consoleプロジェクトで作成する場合

プロジェクト作成  
`dotnet new console`  

dotnet-efツールのローカルインストール  
`dotnet new tool-manifest`  
`dotnet tool install dotnet-ef`  

NuGetパッケージのインストール  
`dotnet add package Microsoft.EntityFrameworkCore`  
`dotnet add package Microsoft.EntityFrameworkCore.Design`  
`dotnet add package Microsoft.EntityFrameworkCore.SqlServer`  
`dotnet add package Microsoft.Extensions.Hosting`  

Program.csを以下のように書き換える  

```cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => services.AddDbContext<DbContext>(options => options.UseSqlServer()))
    .Build();
```

バンドル発行  
`dotnet ef migrations bundle`  

---

## bundleの使い方

ダブルクリックで直接実行 or コンソールから実行  

コンソールの場合はオプションの指定が可能  
`efbundle --connection "Server=<sv_name>;Database=<db_name>;User ID=<user_id>;Password=<passwd>;`  

ヘルプコマンド  
`efbandle --help`  

[移行の適用](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli)  

---

## 接続文字列の有無による挙動の違い

コーディングの段階において、接続情報をプログラム内に埋め込んでいる場合、そのまま移行が実行される。  
コーディングの段階において、接続情報をappsettings.jsonから参照するようにしている場合、appsettings.jsonがないとエラーとなる。  
接続情報を一切記述しない状態でバンドルを発行した場合、実行すると接続エラーとなる。  

ダブルクリックによる誤動作防止対策として、接続文字列は記述せず、`--connection`コマンドによる、接続先の指定をするのがよいかと思われる。  

---

## bundleでdownを実行

bundleにDownコマンドは存在しないが、Downは可能。  
マイグレーションファイルを指定することで、その時点そこまで戻すことができる。  

しかし、バンドルのコマンドで内包しているマイグレーションファイル一覧を出力する機能はないので、戻すファイルを指定する時は、データベースの適応履歴からファイル名を検索して戻すことになる。  

例:  
`efbandle 20221110062826_Second`

---

## トラブルシューティング

### dotnet ef migrations bundle のエラー

バンドルを作成しようとした時にエラーが発生。  

``` txt
Build started...
Build succeeded.
Specify --help for a list of available options and commands.
Unrecognized command or argument 'bundle'.
```

この時の.Netバージョンは5。  
バンドルはEF Core 6.0からの機能なので、バージョンによるエラーとなる。  

[Unrecognized command or argument 'optimize' on Entity Framework Core .NET Command-line Tools 5.0.7](https://github.com/dotnet/efcore/issues/25135)  

### -oによる出力

`-o`で出力先を指定する場合、ファイル名まで指定しないといけない。  
フォルダも事前に作っておかなければならない。  
フォルダだけ指定して、後はefbundleで発行してもらえればと思ったが、駄目だった。  

`dotnet ef migrations bundle --self-contained -r linux-x64 -o linux-x64/efbundle -f`  

### /pオプションの指定

プロジェクトファイルに対して指定するオプションを入れて発行してみたが、エラーになった。  
使わなくてよい。  

`dotnet ef migrations bundle --self-contained -r linux-x64 -o linux-x64/efbundle -f /p:PublishProtocol=FileSystem`

---

## 参考

[Introduction to Migration Bundles - What can they do the migration scripts don't?](https://www.youtube.com/watch?v=mBxSONeKbPk)  
[EF Core 6  - Apresentando Migration Bundles](https://macoratti.net/21/09/efc6_migbndl1.htm)  
[EF Core 6.0: Introducing Migration Bundles](https://jaliyaudagedara.blogspot.com/2021/08/ef-core-60-introducing-migration-bundles.html?spref=tw)  

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

## bundleの最小実装

とりあえず、バンドルを生成するだけの方法。  
実行しても何もしないバンドルが生成される。  

bundle生成において重要なのはスタートアッププログラムにおいてHostingを行う事。  
Webプロジェクトは最初から行っているので実装が楽。  
コンソールアプリでも可能だが、Webプロジェクト以上に必要な操作が多い。  

- 開発環境  
  - windows 10  
  - dotnet 6  

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
`dontet add package Microsoft.Extensions.Hosting`  

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

バンドル作成時はVSCodeおよび、VisualStudioを閉じて実行すること。  
dllへの参照がある状態ではバンドルは作成できない模様。  

---

## bundleの使い方

ダブルクリックで直接実行 or CUIからオプションを指定して実行  

直接実行で接続情報内部埋め込み式ではない場合、appsetting.jsonの接続情報を元に移行だけを実行。  
appsettings.jsonが存在しない場合はエラーとなり、移行は実行されない。  

コーディングの段階において、接続情報をプログラム内に埋め込んでいる場合、そのまま移行が実行される。  
コーディングの段階において、接続情報をappsettings.jsonから参照するようにしている場合、appsettings.jsonがないとエラーとなる。  

CUIからの場合はオプションの指定が可能。  

`efbundle --connection "Server=<sv_name>;Database=<db_name>;User ID=<user_id>;Password=<passwd>;`  

ヘルプコマンド  
`efbandle --help`  

[移行の適用](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli)  

---

## bundleでロールバック

実質的なDownコマンドは存在しない。  
マイグレーションファイルを指定することで、その時点そこまで戻すことができる。  

しかし、バンドルのコマンドで内包しているマイグレーションファイル一覧を出力する機能はないので、戻すファイルを指定する時は、データベースの適応履歴からファイル名を検索して戻すことになるだろうか。  

例:  
`efbandle 20221110062826_Second`

---

## コンソールアプリにおける、bundle作成可能なスタートアップの書き方

`dotnet ef migrations bundle`コマンドでバンドルを作成する時のMainメソッドは何がOKなのか調査。  
DBContextクラスで直接、接続文字列を書くくらいならDIしたいとも思った。  

結論から言うと、公式の方法の通りにやれば良い。  

ASP.NET Core 2.2 アプリで dotnet ef コマンドを実行する場合は、 Program.cs に CreateWebHostBuilder メソッドが必要な模様。  

``` cs : ○ bundle作成できた
string connectionString = @"Server=.\SQLEXPRESS;Database=BundleDB2;Integrated Security=True";

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddDbContext<DatContext>(options =>
            {
                var appsettings = hostContext.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(appsettings ?? connectionString);
            });
    })
    .Build();
// Runまでせずともbundleを作成する事ができた。  
// host.Run();
```

[デザイン時 DbContext 作成](https://learn.microsoft.com/ja-jp/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli)  
[dotnet ef migrations でエラーになった話](https://qiita.com/wukann/items/53462f4b21104ed75c31)  

中途半端にDIするだけだとダメだった結果がこちら。  

``` cs : × bundle作成出来ず
var services = new ServiceCollection();
services.AddDbContext<DatContext>(options => options.UseSqlServer(connectionString));
ServiceProvider serviceProvider = services.BuildServiceProvider();
_datContext = serviceProvider.GetService<DatContext>();
```

[Accessing dbContext in a C# console application](https://stackoverflow.com/questions/49972591/accessing-dbcontext-in-a-c-sharp-console-application)  
[How to Add Entity Framework Core DBContext in .NET Core Console Application](http://www.techtutorhub.com/article/How-to-Add-Entity-Framework-Core-DBContext-in-Dot-NET-Core-Console-Application/86)  

検索文字列 : dependency injection dbcontext console app  

---

## トラブルシューティング

### dotnet ef migrations bundle のエラー

バンドルを作成しようとした時にエラーが発生。  
バンドルはEF Core 6.0からの機能。  
5.0では当然エラーになるというわけです。  

``` txt
Build started...
Build succeeded.
Specify --help for a list of available options and commands.
Unrecognized command or argument 'bundle'.
```

[Unrecognized command or argument 'optimize' on Entity Framework Core .NET Command-line Tools 5.0.7](https://github.com/dotnet/efcore/issues/25135)  

---

## 参考

[Introduction to Migration Bundles - What can they do the migration scripts don't?](https://www.youtube.com/watch?v=mBxSONeKbPk)  
[EF Core 6  - Apresentando Migration Bundles](https://macoratti.net/21/09/efc6_migbndl1.htm)  
[EF Core 6.0: Introducing Migration Bundles](https://jaliyaudagedara.blogspot.com/2021/08/ef-core-60-introducing-migration-bundles.html?spref=tw)  

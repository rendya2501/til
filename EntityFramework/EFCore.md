# EntityFrameworkCore

MigrationにおけるEntityFrameworkCoreの仕様をまとめる  

---

## 列を途中に追加する場合、順序は維持されない

単純にAddColumnされるだけである。  
なので、エンティティのプロパティの順序とデータベースのカラムの順序がずれることになる。  
順番を維持したければ、自前でクエリを書く必要がある。  
(tempテーブルを用意しつつ、新しいテーブルを作って、INSERT SELECTしてTEMPをDROPするやつ)  

---

## 列の変更

列の変更は、Alterではない。
→
drop columnした後、add columnする。
Alter Columnではない。
`EXEC sp_rename 'テーブル名.現在の列名','変更する列名','COLUMN'`  

列定義の変更も同様
alter tableされず、
`ALTER TABLE テーブル名 ALTER COLUMN 変更する列名 データ型`

[EntityFramework Core のエンティティを名前変更したら、テーブル削除/新しい名前でテーブル新規作成のマイグレーションコードが生成されてしまった](https://devadjust.exblog.jp/28190433/)  

---

## 論理名はxmlコメントとしてスキャフォールドされる

---

## WebプロジェクトがMigrationプロジェクトと親和性が高い理由

コンソールアプリからbundleを作成しようとした時、うまくいかなかった。  
スタートアップの書き方はWebをHostするような形でないと駄目らしい。  

参考リンク先の文献をそのまま引用する。  

>どうやら、EF Core CLI は ASP.NET Core アプリケーションの Program クラスに定義されている (であろう) CreateWebHostBuilder メソッドを必要とする模様。  
[dotnet ef migrations でエラーになった話](https://qiita.com/wukann/items/53462f4b21104ed75c31)  

- コンソールアプリではHostをBuildする必要がある。  
  - 最初からHostingが前提のWebアプリのほうが手っ取り早い。  
- コンソールアプリでappsettings.jsonを使う場合、パッケージをインストールする必要がある。  
  - Webアプリでは最初からjsonがサポートされているので手っ取り早い。  
- コンソールアプリではユーザーシークレットの機能が使えない。(たぶん頑張ればいけるけど、調べた感じパッと出てこなかった)  
  - Webアプリは問題なく使える。  

というわけで、webプロジェクトとして構築したほうがいろいろ都合がよい。  

---

## 初期データの投入

マイグレーションファイルに直接記述することで投入可能。  

>Entity Framework Coreのシードデータ  
>ほとんどのプロジェクトでは、作成されたデータベースにいくつかの初期データを持ちたいと思います。  
>そこで、データベースを作成し構成するために移行ファイルを実行するとすぐに、いくつかの初期データを投入したいと思います。  
>このアクションはデータシーディングと呼ばれます。  
>[Migrations and Seed Data With Entity Framework Core](https://code-maze.com/migrations-and-seed-data-efcore/)  

---

## ストアド等のマイグレーションは可能か？

EF6ではストアドは行けそうだが、Coreでは無理。  

・ストアド、シノニムのリバースエンジニアリングはできないことを確認。  
・テーブル以外の情報(view,index,ストアド,シノニム)はマイグレーションファイルのUpメソッドに生クエリで直接記述する。  
・同時にDownメソッドに`DROP VIEW view_hoge"`等の命令を記述すること  

・マイグレーションの時に生クエリをMigrationクラスの中に記述すれば行ける模様。  
・view,indexの作成はコードファーストでは無理。マイグレーションファイルに直接記述する必要がある模様。  

>viewが作れない  
>indexが貼れない  
[[C#]EntityFramework(dotnet ef)マイグレーションまとめ](https://codelikes.com/csharp-dotnet-ef-migrations/)  
[[C#]EntityFrameworkでViewをMigration時に作る](https://codelikes.com/entityframework-view-migration/)  

<!--  -->
>戴いた記事のリンク読みましたが、code firstでのきめ細かい実装は無理と思わざるを得ませんでした。なかなかこれといった解決策はなさそうです。  
[Entity Frameworkは、ストアドプロシージャを自動生成できますか？](https://teratail.com/questions/250125)  
[EF6_Code First での挿入、更新、削除にストアド プロシージャを使用する](https://learn.microsoft.com/ja-jp/ef/ef6/modeling/code-first/fluent/cud-stored-procedures?redirectedfrom=MSDN)  

---

## Migrationプロジェクトをクラスライブラリとして使う方法

まだ検証段階だが、この通りにやれば行けるかも。  

``` txt
sln
├─Sample.Web  Webアプリケーション。Sample.Dbを参照
├─Sample.API  Web APIプロジェクト。Sample.Dbを参照
└─Sample.Db   DBのEntityクラスを定義したクラスライブラリ
```

`dotnet ef migrations add NewMigration --startup-project ..//Sample.API`  

[Entity Framework CoreのMigrationをクラスライブラリに対して実施したい](https://qiita.com/gushwell/items/4c1e54ab3281670db0b9)  
[別の移行プロジェクトを使用する](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/projects?tabs=dotnet-core-cli)  

---

## MigrationメソッドでDownを実行する

dotnet-efコマンドやDbContextクラス等でも、直接的なDownコマンドは存在しない。  
Downしたい場合は、マイグレーションファイル名を直接指定する必要がある。  
そうすることで指定したマイグレーションまで戻ることができ、その時Downメソッドが実行される。  
DbContextクラスのMigrationメソッドを普通に使う場合、引数を受け付けるようにはできていないので、ファイル名を指定したDownを実行することができないが、参考リンク先の方法を使えばそれができるようになる。  

>まず知っておくべき事として、実は、dbContext.Database オブジェクトは、`IInfrastructure<IServiceProvider>` インターフェースを実装している。  
>このインターフェース経由で dbContext.Database オブジェクトに問い合わせすると、dbContext.Database オブジェクトが隠し持っている (?) 各種サービスの参照を手に入れることができる。  
>そして、それらサービスのひとつとして、EFCore におけるマイグレーションの諸々を司る IMigrator インターフェースのサービスがある。  
>この IMigrator インターフェースには、指定の名前のマイグレーション定義にまでマイグレーションを進める、引数に対象マイグレーション定義名を持つ、`MigrateAsync(string)` 又はその同期バージョンである `Migrate(string)`メソッドが用意されているのだ。  
>
>``` cs
>var services = dbContext.Database as IInfrastructure<IServiceProvider>;
>var migrator = services.GetService<IMigrator>();
>await migrator.MigrateAsync("M2");
>```
>
>これで指定したマイグレーション定義、上記例だと "M2" までのマイグレーション適用が可能とな>る。
>[Entity Framework Core + Code Style で、指定名のマイグレーションまでに留めてマイグレーションする](https://devadjust.exblog.jp/28746582/)  

因みにリンク先の内容はM1,M2,M3という移行が必要でM2まで適応させたい場合の方法についての解説しているところである。

---

## 複合主キー

複合主キーはEntityの`Key`アノテーションの指定だけでは無理。  
DbContextクラスのOnModelCreatingメソッドの中でHasKeyメソッドによる複合主キーの設定が必要。  
そうしなければ、マイグレーションファイル作成時にエラーとなり、移行処理を先に進めることができなくなる。  

■`Category`エンティティに`Key`アノテーションで複合主キーを設定する。  

``` cs : Category.cs
public class Category
{
    [Key]
    public int Id { get; set; }
    [Key]
    public string Name { get; set; }
    public ICollection<Product> Products { get; set; }
}
```

■マイグレーションファイル追加コマンド実行  

`dotnet ef migrations add <name>`  

すると、以下のエラーが発生する。  

``` txt
The entity type 'Category' has multiple properties with the [Key] attribute. Composite primary keys can only be set using 'HasKey' in 'OnModelCreating'.
```

DbContextでOnModelCreatingメソッドをoverrideし、Linq中でHasKeyで複合主キーを設定しなければならない。  

``` cs : DbContext
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Name })
                .HasName("Category_PKC");

            // 省略
        }
    }
```

---

## IMigrator インターフェイス

>EF Core Migrations スクリプトを生成したり、データベースを直接移行したりするために使用されるメイン サービス。  
[IMigrator インターフェイス](https://learn.microsoft.com/ja-jp/dotnet/api/microsoft.entityframeworkcore.migrations.imigrator?view=efcore-6.0)  

こいつを掌握できればコマンドからしかアクセスできない処理でも実行できるかもしれない。

---

この方式でバンドルを発行すると、絶対にappsettings.jsonが隣にないと動かない。  

``` cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DatContext>(options =>options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
```

接続情報を空白にすることで問題なくappsettings.jsonがない場合に--connectionで設定可能。  

``` cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DatContext>(options =>options.UseSqlServer());
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
```

コンソールアプリで以下のようにappsettings.jsonを参照するように記述するとappsettings.jsonがなくても、--connectionオプションを指定することで動く。  

``` cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"));
            });
    })
    .Build();
```

■バンドル + web方式

この方式でバンドルを発行すると、絶対にappsettings.jsonが隣にないと動かない。  

``` cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DatContext>(options =>options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
```

●linux
jsonあり ない状態で直接実行 ×→もちろんだめ
jsonあり ある状態で直接実行 ○→もちろん普通に実行される
jsonあり ない状態でコネクション指定 ×　→これだ。これのせいで混乱したんだ。windowsではこれは許可される。
linux jsonあり ある状態でコネクション指定 ○ →コネクションの設定もちゃんと反映される
linux 内包 直接実行 → ○いけた
linux 内包 + connectionstring →○オプションの設定が優先して使用されることを確認した。

●win
win jsonあり ない状態で直接実行 ×→もちろんだめ
win jsonあり ない状態でコネクション指定 ×→あれ？windowsはOKな気がしたけど、駄目みたい。。となれば、バンドルの動作はwinもlinuxも同じか？

●バンドル + console方式 + linux
linux jsonあり ない状態で直接実行 ×→もちろんだめ
linux jsonあり ある状態で直接実行 ○→もちろん普通に実行される
linux jsonあり ない状態でコネクション指定 ○→行けた。
linux jsonあり ある状態でコネクション指定 ○ →オプションの設定が優先して使用される事を確認した。
linux 内包 ○→動く
linux 内包 + connectionstring →○オプションの設定が優先して使用される事を確認した。

●バンドル + console方式 + win
win json ない状態でコネクション指定 →○動いた。web方式では動かないやつはこちらでは動く。
他もおそらくLinuxと同じはず。
win 内包 ○→動く
win 内包 + connectionstring →○オプションの設定が優先して使用される事を確認した。

---

## Consoleアプリで単一実行可能ファイルとして出力した時の

target linux-x64で出力しただけだと動く。  
だけどsingleにまとめると動かない。  
ということは、うまいことまとめられていないということでは？  

-p:PublishTrimmed=trueが悪さしてた。  
これを消したらうまくいった。  

- 発行元環境  
  - Windons10  
  - .Net 7.0.100  
  - efcore 6  
- 検証先環境  
  - AlmaLinux relase 8.7 (Stone Smilodon)
  - .Net 3.1.424

○  
`dotnet publish -o Output -c Release -r linux-x64 -p:PublishSingleFile=true`  

○  
`dotnet publish -o Output -c Release --self-contained true -r linux-x64 -p:PublishSingleFile=true`  

×  
`dotnet publish -o Output -c Release --self-contained=true -r linux-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true`  
→  
PublishTrimmedオプションが有効だとEFCoreのdllが消される？っぽい  

×  
`dotnet publish -o Output -c Release --self-contained false -r linux-x64 -p:PublishSingleFile=true`  
→  
単一exeファイルにはなるが、必要なsdkを内包していないため、そもそも実行できない。  
--self-containedはデフォルトではtrueであることも確認出来た。  

[単一ファイルの配置と実行可能ファイル](https://learn.microsoft.com/ja-jp/dotnet/core/deploying/single-file/overview?tabs=cli)

○  
`dotnet publish -o Output-win-non -c Release --self-contained true -r win-x64 -p:PublishSingleFile=true`  

×  
`dotnet publish -o Output-win-trimmed -c Release --self-contained true -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true`  

trimmedするとwindowsでもエラーになる。  
でもってwindowsの場合、Microsoft.Data.SqlClient.SNI.dllは絶対についてくる模様。  
もちろんこのdllがないとエラーになる。  

>.NET Core に完全に移行されています。  
ただし、Windows (win-x64) では、一部のネイティブ コンポーネントに依存します。これは、Linux では当てはまりません。  
[Why does Microsoft.Data.SqlClient.SNI.dll get published under runtimes?](https://github.com/dotnet/efcore/issues/26175)  

なるほど。  
だからLinuxで発行するとできないのか。  

`IncludeNativeLibrariesForSelfExtract=true`  
これをつけるとこのdllも必要なくなる。  

`IncludeNativeLibrariesForSelfExtract`はcsprojのタグなので`-p:`で指定する  

`dotnet publish -o Output-win-non -c Release --self-contained true -r win-x64 -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true`  

- 単一ファイル出力に必要なオプション  
  - `PublishSingleFile`  
  - `--self-contained`  
  - `IncludeNativeLibrariesForSelfExtract`  

[How do I get rid of SNI.dll when publishing as a "single file" in Visual Studio 2019?](https://stackoverflow.com/questions/65045224/how-do-i-get-rid-of-sni-dll-when-publishing-as-a-single-file-in-visual-studio)
[.NET6の「単一ファイル」](https://qiita.com/up-hash/items/39fa0671bf390147eca9)  
[単一ファイルの配置と実行可能ファイル](https://learn.microsoft.com/ja-jp/dotnet/core/deploying/single-file/overview?tabs=cli#output-differences-from-net-3x)
[.NET 6 で単一ファイルの出力](https://blog.goo.ne.jp/pianyi/e/0a7482af785a4e46c8e04c1c8b28424f)

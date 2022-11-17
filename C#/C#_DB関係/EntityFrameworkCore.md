# EntityFrameworkCore

EntityFrameworkCore(以下EFCore)を使ったデータベース管理およびマイグレーション管理(CREATE,ALTER等の情報)の方法をまとめていく。  

メモ

EFCoreの移行プロジェクト
○・コンソールアプリ
×・クラスライブラリ
？・WebAPI

---

## パッケージ マネージャー コンソール コマンド

`Microsoft.EntityFrameworkCore.Tools`による物  

[Entity Framework Core ツールのリファレンス - Visual Studio のパッケージ マネージャー コンソール](https://learn.microsoft.com/ja-jp/ef/core/cli/powershell)  

---

## dotnet ef コマンド (.NET Core CLI)

[Entity Framework Core ツールのリファレンス - .NET Core CLI](https://learn.microsoft.com/ja-jp/ef/core/cli/dotnet)  

---

## 初期データの投入

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

## dotnet ef のローカルインストール

対象プロジェクトの階層で下記コマンドを実行  

`dotnet new tool-manifest`  

`dotnet tool install dotnet-ef --version 5.0.0`  

`dotnet tool update --version 5.0.17 dotnet-ef`  

[.NET ツールの管理方法](https://learn.microsoft.com/ja-jp/dotnet/core/tools/global-tools#install-a-local-tool)  

## メジャーバージョンの最新をインストールする

`dotnet tool install dotnet-ef --version 5.*`  

`dotnet tool install dotnet-ef --version 5.0.*`  
→  
メジャー バージョンが 5 で、マイナー バージョンが 0 で最新を取得する。  

[dotnet tool update](https://learn.microsoft.com/ja-jp/dotnet/core/tools/dotnet-tool-update)  

---

## リバースエンジニアリングコマンド

``` txt : dotnet cli
Scaffold-DbContext 'Data Source=TestServer;Initial Catalog=TestDatabase;User ID=sa;Password=123456789' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Model -ContextDir Context -Context DatContext -DataAnnotations -UseDatabaseNames -Force
```

``` txt : dotnet ef
dotnet ef scaffold dbcontext 以下同じ
```

``` txt : コマンドの意味
サーバー             : TestServer
データベース         : TestDatabase
ユーザー             : sa
パスワード           : 123456789
モデルの出力先       : Model
コンテキストの出力先 : Context
コンテキスト名       : DatContext
プロパティにアノテーションをつける : -DataAnnotations
データベースのテーブル名に準拠する : -UseDatabaseNames
リバース結果を上書きする           : -force
```

``` txt
Project
├─Context
├─Model
└─Program.cs
```

---

## Entity Framework Core tools

>Entity Framework Core ツールは、設計時の開発タスクに役立ちます。  
主に移行の管理と、DbContext およびエンティティ型のスキャフォールディング (データベースのスキーマをリバース エンジニアリングする) に使用されます。  
[Entity Framework Core tools reference](https://learn.microsoft.com/ja-jp/ef/core/cli/)  

---

## データベースファーストからコードファーストへの移行

1. リバースエンジニアリングする  
2. `dotnet ef migrations add <MigrationName>`コマンドによりMigrationを作成する  
3. MigrationファイルのUpメソッドの内容を全て削除する  
4. `dotnet ef database update`する  

一番最初に移行を行う時の問題。  

途中からEFCoreによるマイグレーション管理を行うということはデータベースファーストからコードファーストへの移行を意味する。  
そうなった場合、最初にリバースエンジニアリングを行い、テーブル構造を取り込んでからバージョン情報だけを記載して、運用を開始するという流れになると思われるが、その方法が分からなかった。  

EF6では`add-migration <MigrationName> -ignoreChanges`というコマンドがあり、これにより、リバースエンジニアリングした後、テーブルの変更を無視してバージョン情報だけを追記することができる模様。  
しかし、EFCoreはまだ荒削りでそのような機能がないということなので、対象法としてUpメソッドの中身を全て消去してマイグレーションを実行する方法になる模様。  

>EF Core Code First は優れていますが、ツールはまだ荒削りです。  
>そこにないものや、機能が完全でないものがあります。  
>
>したがって、この例での問題は、既存のデータベースとモデルに対する移行を行うことでした。  
>古い EF6x の世界では、次のコマンドを使用できました。  
>
>`add-migration MyMigrationName -ignoreChanges`
>
>既存のデータベースとデータに対して最初の移行スクリプトをセットアップします。  
>これは、移行を通じてさらにスキーマの変更を適用できるようにするための開始点です。  
>残念ながら、これは欠けている EF Core 機能の 1 つです。  
>-ignoreChangesパラメータが利用できません。  
>以下に、回避策を説明します。同じ問題に直面している場合に、これが役立つことを願っています。  
[EF Core migrations with existing database schema and data](https://cmatskas.com/ef-core-migrations-with-existing-database-schema-and-data/)  

<!--  -->
>最初にdb firstアプローチを持つプロジェクトがあります。  
>しかし、それを顧客に展開するのに苦労しています。  
>データベースの更新が必要なため、これを手動で行います。  
>DB ファースト アプローチをコード ファースト アプローチに切り替えるソリューションはありますか?  
>→  
>おそらく次のことができます。  
>・生成されたモデル クラスから通常のモデル クラスを作成する  
>・DbFirst アプローチに関連する他のすべてを削除します  
>・上記のモデル クラスに基づいてコード ファーストの移行を作成する  
>・上記で作成した移行の名前を使用して、(使用するすべてのデータベースで) テーブルに手動で行を挿入し__efmigrationshistoryます  
>db スキーマが既に作成されているため、最初に生成された移行が再度実行されません。  
>[EF Migration / How to change DB First approach to the Code First Approach for the existing project](https://stackoverflow.com/questions/69399606/ef-migration-how-to-change-db-first-approach-to-the-code-first-approach-for-th)  

<!--  -->
>ただし、データベースファーストのアプローチはもう使われていないということだけは覚えておいてください。  
>マイクロソフトがコードファーストを重視しているように、私たちもコードファーストでいきます。  
>[Database-First approach in Entity Framework Core](https://www.yogihosting.com/database-first-approach-entity-framework-core/)  

検索キーワード :  
efcore Database First Approach change to code first approach
efcore Add-Migration InitialCreate –IgnoreChanges

---

## 移行の適用

- SQL スクリプト  
- べき等 SQL スクリプト  
- コマンド ライン ツール  
- バンドル  
- 実行時に移行を適用する  

[移行の適用](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli)  

[Handling Entity Framework Core database migrations in production – Part 2](https://www.thereformedprogrammer.net/handling-entity-framework-core-database-migrations-in-production-part-2/)
[Introduction to Migration Bundles - What can they do the migration scripts don't?](https://www.youtube.com/watch?v=mBxSONeKbPk)
[EF Core 6  - Apresentando Migration Bundles](https://macoratti.net/21/09/efc6_migbndl1.htm)  

---

## Add-Migration InitialCreate --IgnoreChanges

EF6にある命令。  
`--IgnoreChanges`は何もせず、バージョンだけを適応する。  
EFCoreで同じ事をやろうとした場合、Upメソッドの中身を全て削除して実行する必要がある模様。  

---

## Trusted_Connection

>Trusted_Connection=Trueは、Windows 認証を指定します。  
>つまり、Windows 資格情報を使用して SQL Server に接続します。  
>ライブ サーバーでは、SQL Server のユーザー名とパスワードを持つSQL Server 認証を使用します。  
>[Database-First approach in Entity Framework Core](https://www.yogihosting.com/database-first-approach-entity-framework-core/)  

YouTubeで見るサンプルでは、たいていこの命令を書いていた。  

---

## EFCore + Console

[EntityFrameworkCoreを.NET Core コンソールアプリでCodeFirstに使う](https://qiita.com/namoshika/items/7d1bf911bc03ed03e17d)  

---

## EFCore + bundle

[Introduction to Migration Bundles - What can they do the migration scripts don't?](https://www.youtube.com/watch?v=mBxSONeKbPk)  
[EF Core 6  - Apresentando Migration Bundles](https://macoratti.net/21/09/efc6_migbndl1.htm)  

## dotnet ef migrations bundle のエラー

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

## bundle作成のスタートアップ

dotnet ef migrations bundleでバンドルを作成する時のMainメソッドは何がOKなのか調査。  
DBContextクラスで直接、接続文字列を書くくらいなら、DIしたいとも思った。  

結論から言うと、公式の方法の通りにやれば良い。  

ASP.NET Core 2.2 アプリで dotnet ef コマンドを実行する場合は、 Program.cs に CreateWebHostBuilder メソッドが必要な模様。  

``` cs : ○ bundle作成できた
string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=BundleDB2;Integrated Security=True";

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

## コンソールアプリからMigrationの実行

DIするならこのような形になる。

``` cs
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using EFCoreSample.Model;

namespace EFCoreSample.Context;

public partial class DatContext : DbContext
{
    public DatContext(){}
    public DatContext(DbContextOptions<DatContext> options) : base(options) { }

    public virtual DbSet<HogeEntity> HogeEntity { get; set; }
}
```

``` cs
using EFCoreSample.Context;
using Microsoft.EntityFrameworkCore;
using System;

Console.WriteLine("開始");
var ob = new DbContextOptionsBuilder<DbContext>();
ob.UseSqlServer("Server=.\SQLEXPRESS;Database=[db_name];Integrated Security=true");
using var dbContext = new DbContext(ob.Options);
dbContext.Database.Migrate();
```

これでもいける。

``` cs
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddDbContext<DatContext>(options =>
            {
                var appsettings = hostContext.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(appsettings);
            });
    })
    .Build();
host.Services.GetService<DatContext>().Database.Migrate();
```

DIしないのであれば、OnConfigureメソッドに直接記述して実行する。

``` cs
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using EFCoreSample.Model;

namespace EFCoreSample.Context;

public partial class DatContext : DbContext
{
    public DatContext(){}

    // Contextクラスにおいて直接、接続情報を記述した場合
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=[database_name];Integrated Security=True");
    }

    public virtual DbSet<HogeEntity> HogeEntity { get; set; }
}
```

``` cs
// Contextクラスにおいて直接、接続情報を記述した場合のMigrate実行
Console.WriteLine("開始");
using var datContext = new DatContext();
datContext.Database.Migrate();
```

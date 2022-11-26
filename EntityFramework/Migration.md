# Migration

EntityFrameworkCore(以下EFCore)を使ったデータベース管理およびマイグレーション管理(CREATE,ALTER等の情報)の方法をまとめていく。  

EF Coreを使用する大きな利点は、マイグレーションと呼ばれるメカニズムを通じてスキーマの変更を管理できることです。  

---

## 移行のアプローチの種類

マイクロソフトが提示する移行のアプローチとして5種類の方法が提示されている。  

- SQL スクリプト  
- べき等 SQL スクリプト  
- コマンド ライン ツール  
- 実行時に移行を適用する  
- バンドル  

[microsoft_移行の適用](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli)  

伝統的な3つの方法としては「SQLスクリプト」「べき等スクリプト」「CLI」が挙げられる。  
実行と同時に移行を適応する方法はリスクがあるため推奨されていない。  
EFCore 6から新しい方法としてバンドルが提供された。  

---

## 各移行のアプローチ全文

>スクリプト作成  
>EF Core ツールを使用して移行から SQL スクリプトを生成する方法があります。  
>この利点は、展開前にスクリプトを検査し、必要に応じて修正することができることです。  
>スクリプトは純粋なSQLであり、自動化プロセスまたはデータベース管理者（DBA）の手動介入を問わず、EF Coreとは独立して管理および展開することが可能です。  
>デフォルトでは、スクリプトは生成元の移行に固有のものであり、以前の変更が適用されていることを前提としています。  
>スクリプトを順番に実行しないと、スクリプトが失敗して予期しない副作用が発生する可能性があります。  
>
>べき等スクリプト  
>2つ目のオプションは、べき等スクリプトを生成することです。  
>これは、既存のバージョンをチェックし、データベースを最新の状態にするために必要に応じて複数のマイグレーションを適用するスクリプトです。  
>どちらのアプローチにもトレードオフがありますが、idempotentオプションは、1つのサイズのスクリプトを持つ最も安全なアプローチです。  
>
>コマンドラインインターフェース (CLI)  
>コマンドラインツールを使って直接アップデートを展開することが可能です。  
>これは、生成されたマイグレーションを検査する機会を与えずに変更を即座にデプロイしてしまうというリスクを伴います。  
>また、ツールの依存関係（.NET SDK、モデルをコンパイルするためのソースコード、ツール自体）が本番サーバーにインストールされている必要があります。  
>
>アプリケーションの起動  
>Database.Migrate() メソッドを呼び出すことにより、アプリケーションの一部として移行を実行することができます。  
>これはうまくいくかもしれませんが、この方法には問題があります。  
>分散システムでは、複数のノードが同時に起動すると、互いに衝突して同時にアップグレードしようとしたり、部分的に更新されたデータベースに対して移行しようとしたりすることがあります。  
>スキーマを変更するには、アプリケーションに昇格した権限が必要ですが、その権限を付与することはセキュリティリスクとなります。  
>CLI のアプローチと同様に、SQL を適用する前に確認する機会がありません。  
>
>移行バンドルの導入  
>スクリプトは依然として移行に有効な選択肢の一つです。  
>コードアプローチを選択する人のために、そしてコマンドラインとアプリケーション起動アプローチに関連するいくつかのリスクを軽減するために、EF CoreチームはEF Core 6.0 Preview 7での移行バンドルのプレビュー利用を発表します。  
>移行バンドルはコマンドラインインターフェースと同じアクションを実行します。  
>
>移行バンドルは、移行を実行するために必要なすべてのものを含む自己充足的な実行ファイルです。  
>パラメータとして接続文字列を受け取ります。これは、すべての主要なツール (Docker、SSH、PowerShell など) で動作する、継続的な展開で使用される成果物を意図しています。  
>ソースコードのコピーや.NET SDKのインストールは必要なく（ランタイムのみ）、DevOpsパイプラインのデプロイメントステップとして統合することができます。  
>また、マイグレーション活動をメインアプリケーションから切り離すため、レースコンディションの心配がなく、メインアプリケーションのパーミッションを上げる必要もありません。  
>将来的には、Visual StudioやGitHub ActionsのようなDevOpsツールセットで、マイグレーションバンドルを使用するファーストクラスのビルディングブロックを利用できるようにすることを目指しています。  
>今のところ、私たちはバンドルを提供し、あなたはそれを実行する責任を負います。
>
>[Introducing DevOps-friendly EF Core Migration Bundles](https://devblogs.microsoft.com/dotnet/introducing-devops-friendly-ef-core-migration-bundles/)  

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

## データベースファーストからコードファーストへの移行

1. リバースエンジニアリングする  
2. `dotnet ef migrations add <MigrationName>`コマンドによりMigrationを作成する  
3. MigrationファイルのUpメソッドの内容を全て削除する  
4. `dotnet ef database update`コマンドを実行する  

一番最初に移行を行う時の問題。  

途中からEFCoreによるマイグレーション管理を行うということはデータベースファーストからコードファーストへの移行を意味する。  
その場合、最初にリバースエンジニアリングを行い、テーブル構造を取り込んでからバージョン情報だけを記載して、運用を開始するという流れになると予想し、方法を調査した。  

EF6では`add-migration <MigrationName> -ignoreChanges`というコマンドがあり、これにより、リバースエンジニアリングした後、テーブルの変更を無視してバージョン情報だけを記載することができる模様。  
しかし、EFCoreはまだ荒削りでそのような機能がないので、対処法としてUpメソッドの中身を全て消去してからマイグレーションを実行する必要がある。  

>EF Core Code First は優れていますが、ツールはまだ荒削りです。  
>そこにないものや、機能が完全でないものがあります。  
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

## WebプロジェクトがMigrationプロジェクトと親和性が高い理由

コンソールアプリからバンドル等を作成しようとした時、うまくいかなかった。  

参考リンク先の文献をそのまま引用する。  

>どうやら、EF Core CLI は ASP.NET Core アプリケーションの Program クラスに定義されている (であろう) CreateWebHostBuilder メソッドを必要とする模様。  

- コンソールアプリではHostをBuildする必要がある。  
  - 最初からHostingが前提のWebアプリのほうが手っ取り早い。  
- コンソールアプリでappsettings.jsonを使う場合、パッケージをインストールする必要がある。  
  - Webアプリでは最初からjsonがサポートされているので手っ取り早い。  
- コンソールアプリではユーザーシークレットの機能が使えない。(たぶん頑張ればいけるけど、調べた感じパッと出てこなかった)  
  - Webアプリは問題なく使える。  

というわけで、webアプリで構築したほうがいろいろ都合がよい。  

[dotnet ef migrations でエラーになった話](https://qiita.com/wukann/items/53462f4b21104ed75c31)  

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

因みにリンク先の内容はM1,M2,M3という移行が必要でM2まで適応させたい場合の方法についての解説しているところである。

[Entity Framework Core + Code Style で、指定名のマイグレーションまでに留めてマイグレーションする](https://devadjust.exblog.jp/28746582/)  

---

## コードからのEFCoreのマイグレーションを実行する

バンドルやCLI以外でも、管理画面から移行を実行したいという要望は結構ある模様。  

>しかし、まれに、APIエンドポイントや管理画面を呼び出して、C#/.NET Coreコードからオンデマンドで移行を実行する必要がある場合があります。  
>これは一般に、Entity Framework の古いバージョンでより多く見られた問題で、データベースの「バージョン」と EF Code が考える「バージョン」とで実際に問題が発生しました。  
>実際、簡単に言えば、それは爆弾のようなものでした。  

[Running EFCore Migrations From Your Own Code](https://dotnetcoretutorials.com/2020/10/06/running-efcore-migrations-from-your-own-code/)  

---

## IMigrator インターフェイス

>EF Core Migrations スクリプトを生成したり、データベースを直接移行したりするために使用されるメイン サービス。  
[IMigrator インターフェイス](https://learn.microsoft.com/ja-jp/dotnet/api/microsoft.entityframeworkcore.migrations.imigrator?view=efcore-6.0)  

こいつを掌握できればコマンドからしかアクセスできない処理でも実行できるかもしれない。

## efcoreソースコード  

[github_dotnet/efcore](https://github.com/dotnet/efcore/blob/main/src/EFCore.Relational/Migrations/IMigrator.cs)  

---

本番環境での Entity Framework データベースの移行の処理について解説しているところ  
[Handling Entity Framework Core database migrations in production – Part 1](https://www.thereformedprogrammer.net/handling-entity-framework-core-database-migrations-in-production-part-1/)  
[Handling Entity Framework Core database migrations in production – Part 2](https://www.thereformedprogrammer.net/handling-entity-framework-core-database-migrations-in-production-part-2/)  
[How to take an ASP.NET MVC web site “Down for maintenance”](https://www.thereformedprogrammer.net/how-to-take-a-asp-net-mvc-web-site-down-for-maintenance/)  
[Handling Entity Framework database migrations in production – part 4, release of EfSchemaCompare](https://www.thereformedprogrammer.net/handling-entity-framework-database-migrations-in-production-part-4-release-of-efschemacompare/)  
[JonPSmith/EfSchemaCompare](https://github.com/JonPSmith/EfSchemaCompare/blob/master/Readme.md)  

EFCoreバンドルの作成について一番参考になる動画
[Introduction to Migration Bundles - What can they do the migration scripts don't?](https://www.youtube.com/watch?v=mBxSONeKbPk)

コンソールアプリでバンドルを作成する方法とコードを紹介しているところ  
[EF Core 6  - Apresentando Migration Bundles](https://macoratti.net/21/09/efc6_migbndl1.htm)  

`dotnet ef database update <Migration Name>`で戻したい時点のMigrationを指定します。  
こうすることでDBの状態が指定したMigration時点に戻ります。  
[EntityFramework CoreでDBの状態を過去のマイグレーションに戻す。](https://kitigai.hatenablog.com/entry/2019/03/05/163622)  

Microsoft公式によるバンドルの紹介  
[Introducing DevOps-friendly EF Core Migration Bundles](https://devblogs.microsoft.com/dotnet/introducing-devops-friendly-ef-core-migration-bundles/)

一連の流れを説明してくれている。  
[Entity Framework のマイグレーションを基礎から理解する](https://qiita.com/yutotakakura/items/31ab539321502deacd88)  

EF Coreのmigrate機能を使わずにデータベースのスキーマを更新する方法  
DbUpいいんじゃないって紹介  
[How to update a database’s schema without using EF Core’s migrate feature](https://www.thereformedprogrammer.net/how-to-update-a-databases-schema-without-using-ef-cores-migrate-feature/)  

EFによる移行をグラフィカルに確認できるっぽいツール  
バンドルだけではできない移行を確認をある程度行える模様。  
まぁ、面白そうだなというか、ある程度グラフィカルに確認したいという欲求はどこの誰しも考えることなんだなと思ったのでリンクしておく。  
.NetFrameworkで作られているので、できれば作り直したい。  
[fatihgurdal/EntityFrameworkMigrationEditor](https://github.com/fatihgurdal/EntityFrameworkMigrationEditor)  

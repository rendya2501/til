# Migration

EntityFrameworkCore(以下EFCore)を使ったデータベース管理およびマイグレーション管理(CREATE,ALTER等の情報)の方法をまとめていく。  

---

## 移行アプローチの種類

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

## 各移行アプローチにおける説明 by Microsoft

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

## 各移行アプローチにおける説明2

>従来のマイグレーション方法  
>
>- 生のSQLスクリプトをデータベースサーバーで実行  
>- dotnet-efツールで指定された接続文字列で適用  
>- アプリケーション起動時に適用  
>
>これらのオプションはうまくいきましたが、それぞれに問題がありました。  
>最初のものは、すべてのマイグレーションを順次適用する必要がありました。  
>2つ目は、展開段階でアプリケーションのソースコードが利用可能である必要があり、.NET Core SDKがインストールされている必要がありました。  
>最後のものは、分散セットアップにおいて、複数のインスタンスが同時にマイグレーションを適用しようとし、さまざまな問題を引き起こす可能性がありました。  
>
>新しいアプローチは、これらの問題を解決しようとするものです。  
>Migration Bundleと呼ばれるもので、マイグレーションを実行するために必要なすべてのものを含む自己完結型の実行ファイルです。  
>これは、先に述べた問題を次のように解決します。  
>
>- 既に適用されているマイグレーションを把握し、必要な変更のみをデータベースに反映させます。  
>- .NET SDK のインストールやアプリケーションのソースコードを必要としません。  
>- 移行をアプリケーションコードから切り離し、分散セットアップでの競合状態を防止します。  
>
>[GitLab CI/CD Series: Building .NET API Application and EF Core Migration Bundle](https://maciejz.dev/gitlab-ci-cd-series-building-net-api-application-and-ef-core-migration-bundle/)  

---

## MigrationにおいてEFCoreを利用するメリット

EF Coreを使用する大きな利点は、マイグレーションと呼ばれるメカニズムを通じてスキーマの変更を管理できることです。  

[Introducing DevOps-friendly EF Core Migration Bundles](https://devblogs.microsoft.com/dotnet/introducing-devops-friendly-ef-core-migration-bundles/)  

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

## 管理画面からEFCoreのマイグレーションを実行する

バンドルやCLI以外でも、管理画面から移行を実行したいという要望は結構ある模様。  

>しかし、まれに、APIエンドポイントや管理画面を呼び出して、C#/.NET Coreコードからオンデマンドで移行を実行する必要がある場合があります。  
>これは一般に、Entity Framework の古いバージョンでより多く見られた問題で、データベースの「バージョン」と EF Code が考える「バージョン」とで実際に問題が発生しました。  
>実際、簡単に言えば、それは爆弾のようなものでした。  

[Running EFCore Migrations From Your Own Code](https://dotnetcoretutorials.com/2020/10/06/running-efcore-migrations-from-your-own-code/)  

---

## ChatGPTに質問したこと

Q.EntityFrameworkCoreを用いたマイグレーション管理のベストラクティスを教えてください。

>1. マイグレーションを管理するツールを使用してください。Entity Framework Coreでは、「dotnet ef」を使用します。  
>2. マイグレーションを実行するときは、データベースを最新の状態に保つようにしてください。  
>3. マイグレーションを行う前に、必ずバックアップを取得してください。  
>4. マイグレーションを行う際には、必ずテストを実行してください。  
>5. マイグレーションを行うときは、必ず現在の状態を開発環境、テスト環境、本番環境などに合わせてください。  
>6. マイグレーションを行うときは、必ず正しいタイミングで行いましょう。  
>7. マイグレーションを行うときは、必ず全ての変更を正しい順番で行ってください。  
>8. マイグレーションを行うときは、必ず全てのインデックスを正しい順番で行ってください。  
>9. マイグレーションを行うときは、必ず全てのデータを正しい順番で行ってください。  
>10. マイグレーションを行うときは、必ず最後にバージョンを更新してください。  

Q.EntityFrameworkCoreによるマイグレーション管理を行うことになりました。
本番のデータベースは各拠点でそれぞれ独立しており、それらの整合性をEntityFrameworkCoreのマイグレーション管理によって管理しなければなりません。
不整合を防ぐことはできますか？
また、このような場合における適切なマイグレーション管理の方法としてどのようなものがあるでしょうか？

>EntityFrameworkCoreによるマイグレーション管理を行うことで、各拠点のデータベースの整合性を管理することができます。  
>マイグレーション管理を行う上で、以下のような方法を採用することが望ましいと考えられます。  
>
>1. 拠点ごとに環境を分けてマイグレーションを行う。  
>2. マイグレーションには、各拠点の処理を反映した変更を記録するためのタイムスタンプを付与する。  
>3. マイグレーションが完了した拠点のデータベースへの変更を、他の拠点のデータベースにも反映させる。  
>4. マイグレーションの差分を記録し、マイグレーションの適用と変更を確認する。  
>
>マイグレーション管理を行うことで、各拠点のデータベースの整合性を保つことができます。  
>また、マイグレーションを行う際には、必ずマイグレーションの差分を記録し、マイグレーションの適用と変更を確認することを忘れないようにしましょう。  

Q.EntityFrameworkCoreのマイグレーション管理における差分の記録はどのように行えばよいですか？  

>EntityFrameworkCoreでは、マイグレーションの差分を記録するために、以下の2つの方法があります。  
>
>1. マイグレーションを実行した時点で、変更されたテーブルやカラムなどのスキーマ定義を記録する。  
>2. マイグレーションを実行した時点で、変更されたレコードを記録する。  
>
>特に、2.の方法を採用することで、差分を把握しやすくなり、マイグレーションの管理が行いやすくなります。  

Q.具体的にどのようなことをすればよいですか？  

>1. マイグレーションを実行した時点で、変更されたテーブルやカラムなどのスキーマ定義を記録する。  
>    - EntityFrameworkCoreのMigrations機能を使用して、スキーマ定義の変更を記録する。  
>    - マイグレーションを実行した時点で、スキーマ定義の変更を記録するために、EntityFrameworkCoreのMigrationsを使用する。  
>
>2. マイグレーションを実行した時点で、変更されたレコードを記録する。  
>     - EntityFrameworkCoreのChange Tracking機能を使用して、レコードの変更を記録する。  
>     - マイグレーションを実行した時点で、レコードの変更を記録するために、EntityFrameworkCoreのChange Trackingを使用する。  

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

efcoreソースコード  
[github_dotnet/efcore](https://github.com/dotnet/efcore/blob/main/src/EFCore.Relational/Migrations/IMigrator.cs)  

プログラムでマイグレーションを実行するための命令について紹介してくれている。  
[EF Core – Apply migrations programmatically](https://makolyte.com/ef-core-apply-migrations-programmatically/)  

WebAPI + EF6 の構築サンプル  
[Ways To Run Entity Framework Migrations in ASP.NET Core 6](https://medium.com/geekculture/ways-to-run-entity-framework-migrations-in-asp-net-core-6-37719993ddcb#2be2)  

EntityFrameworkCoreでは無いほうのドキュメント  
既にデータベースが存在する場合の対応について書かれている。  
[dotnet/EntityFramework.Docs](https://github.com/dotnet/EntityFramework.Docs/blob/main/entity-framework/ef6/modeling/code-first/migrations/existing-database.md)  

EFCoreのマイグレーションに関する考察。  
なんだかんだDbUpのほうがよくね？みたいな事が書いてある。  
[Deploying an Entity Framework Database into Production](https://www.red-gate.com/simple-talk/development/dotnet-development/deploying-an-entity-framework-database-into-production/)  

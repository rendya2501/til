# EFCore + Migration

EntityFrameworkCore(以下EFCore)を使ったデータベース管理およびマイグレーション管理(CREATE,ALTER等の情報)の方法をまとめていく。  

メモ

EFCoreの移行プロジェクト
○・コンソールアプリ
×・クラスライブラリ
？・WebAPI

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
そうなった場合、最初にリバースエンジニアリングを行い、テーブル構造を取り込んでからバージョン情報だけを記載して、運用を開始するという流れになると予想し、方法を調査した。  

EF6では`add-migration <MigrationName> -ignoreChanges`というコマンドがあり、これにより、リバースエンジニアリングした後、テーブルの変更を無視してバージョン情報だけを追記することができる模様。  
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

## 移行の適用

- SQL スクリプト  
- べき等 SQL スクリプト  
- コマンド ライン ツール  
- バンドル  
- 実行時に移行を適用する  

[移行の適用](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli)  

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

M1,M2,M3という移行が必要でM2まで適応させたい場合の方法についての解説  
[Entity Framework Core + Code Style で、指定名のマイグレーションまでに留めてマイグレーションする](https://devadjust.exblog.jp/28746582/)  

`dotnet ef database update <Migration Name>`で戻したい時点のMigrationを指定します。  
こうすることでDBの状態が指定したMigration時点に戻ります。  
[EntityFramework CoreでDBの状態を過去のマイグレーションに戻す。](https://kitigai.hatenablog.com/entry/2019/03/05/163622)  

プロジェクトの中でMigrationプロジェクトをクラスライブラリとして運用したい場合の対処法  
[Entity Framework CoreのMigrationをクラスライブラリに対して実施したい](https://qiita.com/gushwell/items/4c1e54ab3281670db0b9)  

公式でも別のプロジェクトを使う方法を解説してくれている。  
[別の移行プロジェクトを使用する](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/projects?tabs=dotnet-core-cli)  

Microsoft公式によるバンドルの紹介  
[Introducing DevOps-friendly EF Core Migration Bundles](https://devblogs.microsoft.com/dotnet/introducing-devops-friendly-ef-core-migration-bundles/)

一連の流れを説明してくれている。  
[Entity Framework のマイグレーションを基礎から理解する](https://qiita.com/yutotakakura/items/31ab539321502deacd88)  

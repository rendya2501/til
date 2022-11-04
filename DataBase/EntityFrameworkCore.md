# EntityFrameworkCore

EntityFrameworkCore(以下EFCore)を使ったデータベース管理およびマイグレーション管理(CREATE,ALTER等の情報)の方法をまとめていく。  

---

## 初期データの投入

>Entity Framework Coreのシードデータ  
>ほとんどのプロジェクトでは、作成されたデータベースにいくつかの初期データを持ちたいと思います。  
>そこで、データベースを作成し構成するために移行ファイルを実行するとすぐに、いくつかの初期データを投入したいと思います。  
>このアクションはデータシーディングと呼ばれます。  
>[Migrations and Seed Data With Entity Framework Core](https://code-maze.com/migrations-and-seed-data-efcore/)  

---

## モデル以外の管理

EF6ではストアドは行けそうだが、Coreでは無理。  

・ストアド、シノニムのリバースエンジニアリングはできないことを確認。  
・マイグレーションの時に生クエリをMigrationクラスの中に記述すれば行ける模様。  
・view,indexの作成はコードファーストでは無理。マイグレーションファイルに直接記述する必要がある模様。

[[C#]EntityFramework(dotnet ef)マイグレーションまとめ](https://codelikes.com/csharp-dotnet-ef-migrations/)  
[[C#]EntityFrameworkでViewをMigration時に作る](https://codelikes.com/entityframework-view-migration/)  

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

---

## リバースエンジニアリングコマンド

``` dotnet cli
Scaffold-DbContext 'Data Source=TestServer;Initial Catalog=TestDatabase;User ID=sa;Password=123456789' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Model -ContextDir Context -Context DatContext –DataAnnotations –UseDatabaseNames –Force
```

``` txt : コマンドの意味
サーバー             : TestServer
データベース         : TestDatabase
ユーザー             : sa
パスワード           : 123456789
モデルの出力先       : Model
コンテキストの出力先 : Context
コンテキスト名       : DatContext
プロパティにアノテーションをつける : –DataAnnotations
データベースのテーブル名に準拠する : –UseDatabaseNames
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

# Tools・Command

## パッケージ マネージャー コンソール コマンド (dotnet cli)

NuGetから`Microsoft.EntityFrameworkCore.Tools`をインストールすることで使用可能になるコマンド。  
主にVisualStudio内部からコマンドを実行するときに使用する。  

ツール → NuGetパッケージマネージャー → パッケージ マネージャー コンソール  

[Entity Framework Core ツールのリファレンス - Visual Studio のパッケージ マネージャー コンソール](https://learn.microsoft.com/ja-jp/ef/core/cli/powershell)  

---

## dotnet ef コマンド (.NET Core CLI)

パッケージマネージャーコンソールと同等の動作を他のCLIツールから実行するためのコマンド。  
多分こっち使っていればよい。  

使用するためには`dotnet tool`を用いて`dotnet-ef`をインストールする必要がある。  

[Entity Framework Core ツールのリファレンス - .NET Core CLI](https://learn.microsoft.com/ja-jp/ef/core/cli/dotnet)  

---

## dotnet ef のローカルインストール

対象プロジェクトの階層で下記コマンドを実行  

`dotnet new tool-manifest`  

`dotnet tool install dotnet-ef --version 5.0.0`  

`dotnet tool update --version 5.0.17 dotnet-ef`  

[.NET ツールの管理方法](https://learn.microsoft.com/ja-jp/dotnet/core/tools/global-tools#install-a-local-tool)  

---

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
dotnet ef scaffold dbcontext 'Data Source=TestServer;Initial Catalog=TestDatabase;User ID=sa;Password=123456789' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Model -ContextDir Context -Context DatContext -DataAnnotations -UseDatabaseNames -Force
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

``` txt : コマンド実行後のフォルダ構成
Project
├─Context
├─Model
└─Program.cs
```

---

## Add-Migration InitialCreate --IgnoreChanges

EF6にある命令。  
EFCoreにはない。  

※EF6とEFCoreは別物。  

既にデータベースが存在して、データベースファーストからコードファーストへの最初の移行を実施する時に使用する。
データベースには何もせずバージョン情報だけを記載したい場合に実行する。  

`--IgnoreChanges`はデータベースに対して何もしないことを表すオプション。  
EFCoreで同じ事をやろうとした場合、Upメソッドの中身を全て削除して実行する必要がある模様。  

---

## Entity Framework Core tools

EFCoreのパッケージ マネージャー コンソール コマンドを実行できるようにするためのツール。  

>Entity Framework Core ツールは、設計時の開発タスクに役立ちます。  
主に移行の管理と、DbContext およびエンティティ型のスキャフォールディング (データベースのスキーマをリバース エンジニアリングする) に使用されます。  
[Entity Framework Core tools reference](https://learn.microsoft.com/ja-jp/ef/core/cli/)  

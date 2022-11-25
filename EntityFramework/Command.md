# Command

## PMC

PowerShell用コマンド  

[Entity Framework Core ツールのリファレンス - Visual Studio のパッケージ マネージャー コンソール](https://learn.microsoft.com/ja-jp/ef/core/cli/powershell)  

---

## dotnet-ef

[Entity Framework Core ツールのリファレンス - .NET Core CLI](https://learn.microsoft.com/ja-jp/ef/core/cli/dotnet)  

---

## リバースエンジニアリングコマンド

``` txt : PMC
Scaffold-DbContext 'Data Source=TestServer;Initial Catalog=TestDatabase;User ID=sa;Password=123456789' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Model -ContextDir Context -Context DatContext -DataAnnotations -UseDatabaseNames -Force
```

``` txt : dotnet-ef
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

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

---

## `--configurations Bundle`の意味

[Introduction to Migration Bundles - What can they do the migration scripts don't?](https://www.youtube.com/watch?v=mBxSONeKbPk)  
この動画で紹介されているバンドルの作成コマンドは`dotnet ef migrations bundle --configurations Bundle`で紹介されている。  
別にこれで普通にできるのだが、`--configurations Bundle`の意味は何なのか気になったので調べて見た。  

>`--configuration`はビルドの設定（例：Release、Debug）に対して行うモノであり、appsettings.jsonの設定を使いたい場合は`--environment`を指定すれば良いとのこと。  
それか、コマンドを実行する前に`ASPNETCORE_ENVIRONMENT`環境変数を設定してから`dotnet ef databese update`を実行すべしとの事。  
[dotnet ef does not respect configuration](https://stackoverflow.com/questions/52665058/dotnet-ef-does-not-respect-configuration)  

<!--  -->
>ありがたいことに、この問題はBundle-Migration errors with "cannot access file...being used by another process" #25555で報告されており、将来的には修正されるはずです。  
回避策として、コマンドにパラメータ --configuration Bundle を追加すると、このエラーはなくなります。  
[GitLab CI/CD Series: Building .NET API Application and EF Core Migration Bundle](https://maciejz.dev/gitlab-ci-cd-series-building-net-api-application-and-ef-core-migration-bundle/)  

どうやらエラー回避のためのオプションとして有効な模様。  
それ以外はわからないが、バンドル生成の時につけておいて損はないのかもしれない。  

あー、`--configurations`でdebug,release以外の文言で生成しないとdllを参照しているからブロックされてエラーになってしまうのか。
それを回避するのがこのコマンドということかな？  

[EF Core 6 new features and changes for .NET 6](https://www.roundthecode.com/dotnet/entity-framework/ef-core-6-new-features-and-changes-for-net-6)  

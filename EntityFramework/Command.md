# Command

## パッケージ マネージャー コンソール (PMC)

[Entity Framework Core ツールのリファレンス - Visual Studio のパッケージ マネージャー コンソール](https://learn.microsoft.com/ja-jp/ef/core/cli/powershell)  

---

## dotnet-ef

[Entity Framework Core ツールのリファレンス - .NET Core CLI](https://learn.microsoft.com/ja-jp/ef/core/cli/dotnet)  

---

## リバースエンジニアリングコマンド

PMC

``` txt
Scaffold-DbContext 'Server=TestServer;Database=TestDatabase;User ID=sa;Password=123456789' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Model -ContextDir Context -Context DatContext -DataAnnotations -UseDatabaseNames -Force
```

dotnet-ef

``` txt
dotnet ef dbcontext scaffold 'Server=TestServer;Database=TestDatabase;User ID=sa;Password=123456789' Microsoft.EntityFrameworkCore.SqlServer -o Entity --context-dir Context --context DatContext --data-annotations --use-database-names --no-onconfiguring --force
```

コマンドの意味

``` txt
サーバー             : TestServer
データベース         : TestDatabase
ユーザー             : sa
パスワード           : 123456789
モデルの出力先       : Model
コンテキストの出力先 : Context
コンテキスト名       : DatContext
プロパティにアノテーションをつける : -DataAnnotations  --data-annotations
データベースのテーブル名に準拠する : -UseDatabaseNames --use-database-names
リバース結果を上書きする           : -Force  --force
```

コマンド実行後のフォルダ構成

``` txt
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

>`--configuration`はビルドの設定（例：Release、Debug）に対して行うモノであり、appsettings.jsonの設定を使いたい場合は`--environment`を指定すれば良い。  
それか、コマンドを実行する前に`ASPNETCORE_ENVIRONMENT`環境変数を設定してから`dotnet ef databese update`を実行すべし。  
[dotnet ef does not respect configuration](https://stackoverflow.com/questions/52665058/dotnet-ef-does-not-respect-configuration)  

<!--  -->
>ありがたいことに、この問題はBundle-Migration errors with "cannot access file...being used by another process" #25555で報告されており、将来的には修正されるはずです。  
回避策として、コマンドにパラメータ --configuration Bundle を追加すると、このエラーはなくなります。  
[GitLab CI/CD Series: Building .NET API Application and EF Core Migration Bundle](https://maciejz.dev/gitlab-ci-cd-series-building-net-api-application-and-ef-core-migration-bundle/)  

<!--  -->
>ここでも、上記と同じエラーが表示されました。  
この問題を回避する方法は、dotnetコマンドラインスクリプトに-configurationパラメータを追加し、そのパラメータにdebugやrelease以外のものが含まれていることを確認することでした。  
`dotnet ef migrations bundle --verbose --configuration abc`  
これでうまくいき、efbundles.exe ファイルが生成されました。  
[EF Core 6 new features and changes for .NET 6](https://www.roundthecode.com/dotnet/entity-framework/ef-core-6-new-features-and-changes-for-net-6)  

どうやらエラー回避のためのオプションとして有効な模様。  
それ以外はわからないが、バンドル生成の時につけておいて損はないのかもしれない。  

2つある海外の記事では

■**予想**  

`--configurations`でdebug,release以外の文言で生成しないとdllを参照しているからブロックされてエラーになってしまう。  
それをこのコマンドで回避できる可能性。  

■**検証結果**  

実際に検証してみたが、効果はなかった。  
VisualStudioからPMCコマンドで`Migration-Bundle`を実行した場合、普通にエラーとなった。  
dotnet-efコマンドで`dotnet ef migrations bundle`を実行して、同じくエラー。  
dotnet-efコマンドで`dotnet ef migrations bundle --configurations Bundle` or `--configurations abc`を実行してみたが、エラーとなった。  
コンフィグレーションの指定は関係ない模様。  
`--verbose`結果の中で`--runtime' を使用する場合は、'--self-contained' または '--no-self-contained' オプションのいずれかが必要です。`とあったので、試しに`--self-contained`を入れて見たらコンフィグレーション関係なくバンドルが作成できてしまった。  

検証結果的には、`--configuration Bundle`ではなく`--self-contained`が重要であることが分かった結果となった。  
結果として、`--configuration Bundle`はバンドル生成時のエラー回避のためのおまじない程度でしかないということで決着だろうか。  

検証が終わった後に#25555のスレッドを見てみたが、`--configuration Bundle`は常にうまくいくとは限らない模様。  
うまくいったり行かなかったりで、やはりおまじないの域は出ないのかもしれない。  
2022/11/25 Friの書き込みでは`dotnet ef migrations bundle --no-build --force --configuration Production`で回避できるらしい。  
`appsetting.[EnvironmentName].json`を用意して、`EnvironmentName`をConfigurationに指定するといいっぽい。  
[「ファイルにアクセスできません...別のプロセスで使用されています」というバンドル移行エラー #25555](https://github.com/dotnet/efcore/issues/25555)  

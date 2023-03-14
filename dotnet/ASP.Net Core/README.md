# ASP.Net Core

ASP.Net Coreについてまとめるところ

---

## WebAPIのSwaggerをデフォルトページとしてIISにデプロイしたい

色々迷走したが、このサイトの通りにやれば問題ない。  
[Host And Publish .NET Core 6 Web API Application On IIS Server](https://www.c-sharpcorner.com/article/host-and-publish-net-core-6-web-api-application-on-iis-server2/)  

嵌ったポイントとしては新しくWebサイトを追加するのではなく、アプリケーションの追加で頑張ってしまったため、うまく行かなかった。  
プログラム側でガチャガチャやる必要はなく、完全にIIS側の設定で済む話だった。  
デフォルトページにきたらリダイレクトも考えたが、そもそもアプリのurlに到達しての話なので、全然関係なかった。  
→  
いや、関係あったわ。  

■**手順**  

- 開発環境  
.Net6  
VisualStudioCode
IISインストール済み  

1.**プロジェクト作成**  

実行コマンド  
`dotnet new webapi -n iis_webapi`  

2.**Program.csを編集**  

- `app.Environment.IsDevelopment()`の部分に条件文追加  
- リダイレクト命令の追加  

``` cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", async context =>
{
    context.Response.Redirect("/swagger/index.html");
    await context.Response.CompleteAsync();
});

app.Run();
```

3.**プロジェクト発行**  

実行コマンド  
`dotnet publish -c Release`  

発行物を確認する。  
`iis_webapi\bin\release\net6.0\publish`  

4.**サイトの設定**  

iisを開いて`サイト`を右クリック→`Webサイトの追加`  

- サイト名 : `iis_webapi`  
- ポート番号 : `8080`  
  ※`8080`が使われている場合は他のポート番号を指定する
- 物理パス : `C:\inetpub\wwwroot\iis_webapi`  
  ※`iis_webapi`フォルダは作成する  

`bin\release\net6.0\publish`の中身を`C:\inetpub\wwwroot\iis_webapi`に全てコピー  

5.**サイトへアクセス**  

iisの右側の操作タブ,`Webサイトの参照`から`*:8080(http)参照`をクリック or `http://localhost:8081/`にアクセス。  

- 他参考サイト  
[How To Set Default Swagger Welcome Page In Web Api](https://www.adoclib.com/blog/how-to-set-default-swagger-welcome-page-in-web-api.html)  
[How to redirect root to swagger in Asp.Net Core 2.x?](https://stackoverflow.com/questions/49290683/how-to-redirect-root-to-swagger-in-asp-net-core-2-x)  
[How to set Swagger as default start page?](https://stackoverflow.com/questions/47457681/how-to-set-swagger-as-default-start-page)  
[How to use Swagger as Welcome Page of IAppBuilder in WebAPI](https://stackoverflow.com/questions/30028736/how-to-use-swagger-as-welcome-page-of-iappbuilder-in-webapi/50127631#50127631)  
[How to host Asp.net core web API on IIS](https://www.youtube.com/watch?v=6HiaXCAlRr0)  
[ASP.NET Core でリダイレクトする (301,302 リダイレクトの実装) (ASP.NET Core プログラミング)](https://www.ipentec.com/document/asp-net-core-implement-301-302-redirect)  

- 検索文字列  
webapi iis swagger default  
.net core web api redirect swagger  
launchsettings.json iissettings applicationurl  
set swagger as default page iis  
asp.net core redirect to url .net6 program.cs  
iis default url  
.net core default url  

---

## Swaggerにドキュメントコメントを反映させる

.Net6の場合  

プロジェクト右クリック,プロパティ→ビルド→出力→ドキュメントファイル→APIドキュメントを含むファイルを生成にチェック  

Program.csに以下のコードを記述

``` cs : Program.cs
services.AddSwaggerGen(c =>
{
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
```

[Swagger in ASP.NET Core: Tips & Tricks](https://blog.georgekosmidis.net/swagger-in-asp-net-core-tips-and-tricks.html)  

---

## ASP.NET Core開発におけるユーザーシークレットの安全な保存

dotnet-efコマンドでスキャフォールドする時に、`--no-onconfiguring`オプションを入力しない状態で接続先情報を直接入力すると黄色い文字で警告が表示される。  

``` txt : 実行コマンド
dotnet ef dbcontext scaffold 'Server=<sv_name>;Database=<db_name>;User ID=<user_id>;Password=<passwd>;' Microsoft.EntityFrameworkCore.SqlServer --output-dir Entity --context-dir Context --context DatContext --data-annotations --use-database-names --force
```

``` txt : 警告文
To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
```

>接続文字列に含まれる潜在的な機密情報を保護するために、接続文字列をソースコードの外に移動する必要があります。
>接続文字列を構成から読み込むためにName=構文を使用することで、scaffoldを回避することができます - <https://go.microsoft.com/fwlink/?linkid=2131148> を参照してください。  
>接続文字列の保存に関するより詳しいガイダンスは、<http://go.microsoft.com/fwlink/?LinkId=723263> を参照してください。  

警告文を訳すとそのままなのだが、接続情報はセンシティブなので、機密データを格納するいい方法があるからそっち使ったほうがいいよって言ってくれる。  

[リバース エンジニアリング](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/scaffolding/?tabs=dotnet-core-cli#configuration-and-user-secrets)  
上記リンク先の[リンク構成とユーザーシークレット]で紹介されている構文を利用することで`Name=ConnectionString:<alias>`で安全？にスキャフォールドできるっぽい。  

そこから先はまた何か分かったらまとめる。  
因みにこの方法はコンソールアプリでは使えない。  

``` txt : ユーザーシークレットを使った場合
dotnet user-secrets init
dotnet user-secrets set connectionStrings:<Alias> "Server=<sv_name>;Database=<db_name>;User ID=<user_id>;Password=<passwd>;"
dotnet ef dbcontext scaffold Name=ConnectionStrings:<Alias> Microsoft.EntityFrameworkCore.SqlServer --output-dir Entity --context-dir Context --context DatContext --data-annotations --use-database-names --force
```

[ASP.NET Core での開発におけるアプリ シークレットの安全な保存](https://learn.microsoft.com/ja-jp/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows)  
[ASP.NET CoreにおけるUserSecretを使用した設定情報の保存](https://shuhelohelo.hatenablog.com/entry/2019/11/13/101328)  

---

## Name=\<connection-string\> 構文によるappsettings.jsonの読み取り

EFCoreによるスキャフォールド及びバンドル作成において検証。  
appsettings.jsonの読み取りは`Configuration.GetConnectionString("DefaultConnection")`  

ユーザーシークレット使用  
appsettings.jsonにConnectionStrings.DefaultConnectionを追加  
`name=ConnectionStrings:DefaultConnection`で読み取り  
→OK  

[ConnectionStrings.DefaultConnection]→[ConnectionStrings.DefaultConnection2] に変更  
バンドル作成  
→エラー  

`name=ConnectionStrings:DefaultConnection`を`name=ConnectionStrings:DefaultConnection2`に変更  
バンドル作成  
→OK  

これから見るに、name=構文はappsettings.jsonを見ている。  

試しにユーザーシークレットを使わないでスキャフォールドして同じような状況を作ってみた。  

appsetting.jsonにConnectionStrings.DefaultConnectionを追加  
name=ConnectionStrings:DefaultConnectionで読み取り  
バンドル作成  
→OK  

というわけで、name構文でもappsetting.jsonを読み取れることが分かった。  
ユーザーシークレットの有無は関係ない模様。  

まぁ、それでappsettings.jsonを読み取れるならそれでいいか。  

``` json : appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection2": "Server=<sv_name>;Database=<db_name>;User ID=<user_id>;Password=<passwd>"
  },
  "AllowedHosts": "*"
}
```

``` cs
using namespace.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DatContext>(
    options =>options.UseSqlServer("name=ConnectionStrings:DefaultConnection2"));
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
```

[[ASP.NET]多層システムにおけるDbContextのバケツリレーをDIコンテナで解消する](https://qiita.com/jun1s/items/484cb55e7b6023e8fd23)  
[ASP.NET Core の依存関係の挿入における DbContext](https://learn.microsoft.com/ja-jp/ef/core/dbcontext-configuration/#dbcontext-in-dependency-injection-for-aspnet-core)  

---

## ASP.Net Core Web API のSwaggerを VSCodeのCLIから立ち上げる

`WepAPI`を起動する時の話。  

Visual StudioでWebAPIをF5実行するとき、`IIS Express`の起動オプションがあるので、それで実行すると、自動的にSwaggerが表示される。  
同じ感覚でVSCodeからCLIで`dotnet run`すると、何も立ち上がらない。  
表示されるURLに飛んでもエラーの画面が出るだけ。  
VSCodeからCLIで起動した場合にSwaggerを使うためにはどうすればよいのか調べた。  

結果的に表示されるURLに`/Swagger`でアクセスするだけでよかった。  

`https://localhost:port/swagger/`  

[VSCODE ASP.NET Coreで「Swagger」を導入して利用するまで](https://mebee.info/2021/07/17/post-35547/)  

`dotnet run --launch-profile "IIS Express"`  
このコマンドでProperties/launchSettings.jsonの起動プロファイルを指定することができる。  
その中に`IIS Express`とあるので、指定して起動してみるのだが、`起動プロファイル "IIS Express" を適用できませんでした。起動プロファイルの種類 'IISExpress' はサポートされていません。`という警告が表示されてVisualStudioみたく、Swaggerを起動させることができなかった。  

検索文字列  
dotnet watch run swagger  
dotnet swagger cli

---

## ASP.Net Core Web API のデフォルトURLを変更する

`WebAPI`を`dotnet run`で立ち上げたときに表示されるデフォルトのURLの末尾に`/Swagger`って追加して、Ctrl+クリックですぐにSwaggerに飛べるようにできないか調べてみた。  

できなかったが、同じようなことを考えている人たちはいたのでリンクだけまとめる。  

[Set start URL in ASP.NET Core – Quick & Easy ways](https://procodeguide.com/programming/how-to-set-start-url-in-aspnet-core/)  
[How to set up URL of ASP.NET Core Web Application in Visual Studio 2015](https://stackoverflow.com/questions/38737847/how-to-set-up-url-of-asp-net-core-web-application-in-visual-studio-2015)  

---

## ASP.Net Core WebAPI の IISへのデプロイ

自己完結型での発行でない場合は`.NET Core ホスティング バンドル`のバージョンをプロジェクトの.Netのバージョンと合わせる必要がある。  

[.NET Core の WebAPI（C#）をIISにデプロイする（VisualStudioCode）](https://qiita.com/HyunwookPark/items/92f06f917b3a479438c1)  

---

## C# .Net6のWebプロジェクトにおけるappsettings.jsonの取得

`dotnet new web`で`ASP.Net Core (空)`プロジェクトを作成した時のデフォルトのProgram.csの状態は以下の通り  

``` cs : Program.cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
```

この状態において`appsettings.json`に`ConnectionStrings.DefaultConnection`を追加する。  

``` json : appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=<sv_name>;Database=<db_name>;User ID=<user_id>;Password=<passwd>"
  },
  "AllowedHosts": "*"
}
```

`builder.Configuration`から`GetConnectionString()`メソッドによって読み取ることができる。  

``` cs
var builder = WebApplication.CreateBuilder(args);
// buiderのConfigurationから取得可能
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
```

---

## ANCM Failed to Find Native Dependencies

.Net5の環境に.Net6のアプリをデプロイした時に発生。  
古いバージョンの環境に最新のバージョンを乗せるのだから当たり前か。  

[HTTP Error 500.31 Failed to load ASP NET Core runtime](https://www.youtube.com/watch?v=4oGwobgBkWU)  
この動画を参考に`ASP.NET Core 6.0 Runtime`をダウンロードしてインストールしてみたらローカルは解決した。  

[ASP.NET > ASP.NET Core > HTTP エラー 500.19 0x8007000d](https://qiita.com/sugasaki/items/02927c18497c0d2379fa)  

---

## dotnet runからSwaggerを起動する

これで行けた。  
`dotnet watch run`  

プロファイルを指定した起動  
`dotnet run --launch-profile "IIS Express"`  

上記のように、プロファイルを指定した起動も可能らしいが、以下のようなエラーとなってしまう。  
dotnet run からIISのプロファイル起動はデフォルトの状態では無理な模様。  

``` txt
起動プロファイル "IIS Express" を適用できませんでした。
起動プロファイルの種類 'IISExpress' はサポートされていません。
```

CLIからの起動はkestrelだけがサポートされているためだと思われる。  
同じようなことを考えている人はいた。  
→[Launching from CLI with IIS Express profile fails #18925](https://github.com/dotnet/AspNetCore.Docs/issues/18925)  

少々話が逸れるが、デフォルトルートにリダイレクト命令をかませることでも実現可能。  
`dotnet run`を実行した時に表示されるURLを開くことで、swaggerのページに飛んでくれる。  

``` cs
app.MapGet("/", async context =>
{
    context.Response.Redirect("/swagger/index.html");
    await context.Response.CompleteAsync();
});

app.Run();
```

- 参考  
  - [VS CodeでWebコーディング環境を作ろう（IIS向け）](https://machdesign.net/blog/article/vscode-iis-windows)  
  - [Run Dotnet Core App With Code Examples](https://www.folkstalk.com/tech/run-dotnet-core-app-with-code-examples/)  
- 検索文字列 : web api dotnet run iis express vscode  

---

## profileの指定の仕方

dotnet watch runだけではだめ  

■**dotnet run**  

| 成否 | TH | コマンド |
| :--- | :--- | :--- |
| ○ | 手動 | `dotnet run --profile IIS Express` |
| ○ | 手動 | `dotnet run --profile "IIS Express"` |
| ○ | 手動 | `dotnet run --profile WebAPISample` |
| ○ | 手動 | `dotnet run --profile "WebAPISample"` |
| × | ---- | `dotnet run --launch-profile IIS Express` |
| × | ---- | `dotnet run --launch-profile "IIS Express"` |
| ○ | 手動 | `dotnet run --launch-profile WebAPISample` |
| ○ | 手動 | `dotnet run --launch-profile "WebAPISample"` |

■**dotnet watch run**  

| 成否 | TH | コマンド |
| :--- | :--- | :--- |
| ○ | 自動 | `dotnet watch run --profile IIS Express` |
| ○ | 自動 | `dotnet watch run --profile "IIS Express"` |
| ○ | 自動 | `dotnet watch run --profile WebAPISample` |
| ○ | 自動 | `dotnet watch run --profile "WebAPISample"` |
| ○ | 自動 | `dotnet watch run --launch-profile IIS Express` |
| × | ---- | `dotnet watch run --launch-profile "IIS Express"` |
| × | ---- | `dotnet watch run --launch-profile WebAPISample` |
| × | ---- | `dotnet watch run --launch-profile "WebAPISample"` |

launchSettings.jsonのプロファイル名を指定して上げないと立ち上がらない。  

---

## InterfaceでRequestを受け取る

InterfaceではRequestを受け取れない。  
以下のエラーが発生する。  
`System.NotSupportedException: 'Deserialization of interface types is not supported.`

リクエストの問題というよりはJsonのデシリアライズの問題の模様。  
インターフェースを実装した具象クラスであれば問題は発生しない。  

`dotnet new web -o MinimalAPI -f net6.0`  

``` cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
// OK
app.MapPost("/OK1", (Todo todo) => new { todo.ID, todo.Name });
app.MapPost("/OK2", (ConcreateTodo todo) => new { todo.ID, todo.Name });
// NG
app.MapPost("/NG", (ITodo todo) => new { todo.ID, todo.Name });
app.Run();


class Todo
{
    public int ID { get; set; }
    public string? Name { get; set; }
}

class ConcreateTodo : ITodo
{
    public int ID { get; set; }
    public string? Name { get; set; }
}

interface ITodo
{
    int ID { get; set; }
    string? Name { get; set; }
}
```

[[NET 6] Cannot deserialize an Interface from JSON in ASP.NET Core? : csharp](https://www.reddit.com/r/csharp/comments/rakcvp/net_6_cannot_deserialize_an_interface_from_json/)  
[C# – Casting interfaces for deserialization in JSON.NET – iTecNote](https://itecnote.com/tecnote/c-casting-interfaces-for-deserialization-in-json-net/)  

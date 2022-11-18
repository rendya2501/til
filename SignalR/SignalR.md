# SignalR

## SignalR 概要

[SignalR を Windows Mixed Reality で使いたい](https://blog.okazuki.jp/entry/2018/04/26/125929)  

- リアルタイムの双方向通信を簡単におこなうライブラリ  
- WebSocket とかをいい感じに隠してくれてサーバーからクライアント（Webやネイティブアプリなど）に対して処理を実行することが出来る  
- SignalRの通信はURLを指定するのではなく、サーバー、クライアントのお互いの関数を呼び合う感じで行う  
- Web側にHubを実装し、フロント側からHubにアクセスすることで実現できる  

---

## .Net6 + ASP.NET Core SignalR における最小実装

[SignalR Core復習](https://shuhelohelo.hatenablog.com/entry/2020/02/10/162108)  

大体ここ参考にした。  
かなり良い記事なのだが、一番上に出てこないタイプだったので、たどり着くまでに苦労した。  

### 開発環境

- Windows 11 Home  
- .Net6  
  - ConsolApp  
  - ASP.NET Core(空)  

### 実行手順

- サーバー側  
  - SignalRServerのソリューションを立ち上げて実行  

- クライアント側  
  - SignalRClientソリューションを2つ立ち上げてそれぞれで実行  

---

### サーバ実装

[1] プロジェクト作成  

[ASP.NET Core(空)] プロジェクトを選択する。  
.Net6 を選択して作成。  

※  
[ASP.NET Core Web API] でも動作することを確認した。  
こちらは実務で使っているSwaggerが起動するタイプのプロジェクト。  
やることも以下の手順と同じようにやればOK。  
しかし、ただ最小実装を目指すなら余計なモノが入っていない ASP.NET Coreの空プロジェクトで十分。  

[2] nugetからSignalRライブラリをインストール  

nugetから以下の2点をインストールする。  

- AspNetCore.SignalR.Core  
- AspNetCore.SignalR  

※  
[ASP.NET Core SignalR でルーム付きチャットアプリを作ってみた](https://www.tetsis.com/blog/asp-net-core-signalr-group-chat/)

→  
マイクロソフト公式、及びこのサイトでは以下のように設定すべしと紹介しているが、nugetだけで十分であることがわかった。  
一応この通りにやっても動くので備忘録として残しておく。  

→  
ソリューションエクスプローラーで、プロジェクトを右クリックし、[追加] -> [クライアント側のライブラリ]を選択する  

[クライアント側のライブラリを追加します]ダイアログで以下の通り選択する  
プロバイダー: unpkg  
ライブラリ：@microsoft/signalr@latest  
[特定のファイルの選択]を選択する  
dist/browser フォルダを展開し、 signalr.js と signalr.min.js を選択する  
[ターゲットロケーション]に wwwroot/js/signalr/ と入力する  
[インストール]をクリック  

[3] SignalR Hubの実装  

``` C#
using Microsoft.AspNetCore.SignalR;

namespace WebApplication1
{
    public class ChatHub : Hub
    {
        /// <summary>
        /// クライアントの呼び出しに対応するメソッド
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <remarks>
        /// クライアントからチャットが送られてきた場合、それを接続している全てのクライアントに対して送り返しているだけ
        /// </remarks>
        public async Task SendMessage(string name, string message)
        {
            // クライアントのメソッドを呼び出す処理
            await Clients.All.SendAsync("ReceiveMessage", name, message);
        }
    }
}
```

[4] StartUpの実装  

Asp.Netまでは [StartUp.cs] が存在したのかもしれないが、 .Net6 からは [Program.cs] に変わった模様。  
どちらであっても、同じように実装すればよろしい。  

``` C# : Program.cs
// Hubを実装した名前空間
using WebApplication1;

var builder = WebApplication.CreateBuilder(args);
// SignalR追加
builder.Services.AddSignalR();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
// SignalR Hubの登録
app.MapHub<ChatHub>("/chatHub");

app.Run();
```

[5] サービスの起動と動作確認。  

F5でデバッグ実行。  
[https://localhost:~~~~/chatHub] にアクセスして、 `Connection ID required` と表示されれば動作確認はOK。  

---

### クライアント側実装

[1] プロジェクト作成  

コンソールアプリで十分なので、.Net6でコンソールアプリプロジェクトを新規で作成する。  

[2] nugetからSignalRライブラリをインストール  

nugetから以下2点をインストール  

- SignalR.Client
- SignalR.Client.Core

[3] クライアント実装  

``` C#
// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.SignalR.Client;

MainAsync(args).GetAwaiter().GetResult();

static async Task MainAsync(string[] args)
{
    Console.Write("What is your name? : ");
    var yourname = Console.ReadLine();

    // 1. Hub へのコネクション
    var connection = new HubConnectionBuilder()
        .WithUrl("https://localhost:7263/chatHub")
        .WithAutomaticReconnect()
        .Build();

    // 2. サーバーからの呼び出しに対応するメソッドの登録
    connection.On<string, string>(
        "ReceiveMessage",
        (user, message) =>
        {
            Console.WriteLine($"{user} : {message}");
            Console.Write("Message :");
        }
    );

    // 3. コネクション開始
    try
    {
        //サーバー側のSignalRハブと接続
        await connection.StartAsync();
        Console.WriteLine("接続できました");
    }
    catch (Exception ex)
    {
        Console.WriteLine("接続できませんでした:" + ex.Message);
        return;
    }

    Console.Write("Message :");
    while (true)
    {
        // メッセージ待機
        var yourmessage = Console.ReadLine();

        //メッセージ送信
        try
        {
            // 4. サーバーのメソッド呼び出し
            await connection.InvokeAsync("SendMessage", yourname, yourmessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine("メッセージの送信に失敗しました:" + ex.Message);
        }
    }
}
```

---

### 躓いたところ

[1]  
クライアント実装において、HubConnectionBuilderのWithUrlメソッドが存在しないと言われた。  

→  
nugetで[Microsoft.AspNetCore.SignalR.Client.Core]のインストールが必要であった。  
SignalRのClientとつくものを全てインストールしていればこんな事にはならなかったのだろうけど、初めだから何も知らなくて、そういうチュートリアルもどこにもなく、地味に嵌ったのでメモしておく。  

[WithUrl() not found in Core 3 client - Stack Overflow](https://stackoverflow.com/questions/58677179/withurl-not-found-in-core-3-client)  

[2]  
クライアント実装において、設定はあっているはずなのに全然サーバーに接続できなかった。  
ふと気が付いて、そもそも表示されているURLをそのままコピーして貼り付けたら接続できた。  

→  
結論は接続プロトコルがHTTPだったから。  
ローカルでの接続だからてっきりHTTPだと思っていたけど、HTTPSで接続する必要があった。  
スキームをHTTPSにしてつないだら問題なく実現できた。  

プロトコルはHTTPかHTTPSか確認すること。  

---

## 他ミドルウェアとの連携

[RedisをBackplaneとしたSignalRのスケールアウト](https://www.buildinsider.net/web/iis8/07)  
[ASP.NET Core SignalR のスケールアウトのために Redis バックプレーンを設定する](https://docs.microsoft.com/ja-jp/aspnet/core/signalr/redis-backplane?view=aspnetcore-6.0)  

現実問題、大規模になれば間にRedis等を挟むことになるはず。  
そうなった時にSignalRは動くのか、また性能を確保できるのかという問題が発生してくる。  
備忘録として残す。  

---

## 参考

[Microsoft_ASP.NET Core 用 SignalR でハブを使用する](https://docs.microsoft.com/ja-jp/aspnet/core/signalr/hubs?view=aspnetcore-6.0)  
[Microsoft_チュートリアル: ASP.NET Core SignalR の概要](https://docs.microsoft.com/ja-jp/aspnet/core/tutorials/signalr?view=aspnetcore-6.0&tabs=visual-studio)  
[Microsoft_ASP.NET Core SignalR .NET クライアント](https://docs.microsoft.com/ja-jp/aspnet/core/signalr/dotnet-client?view=aspnetcore-6.0&tabs=visual-studio)  

[SignalR でChat アプリを作ってみた](https://qiita.com/TsuyoshiUshio@github/items/65ea4e2669afa19f6a31)  
[ASP.NET Core SignalR でルーム付きチャットアプリを作ってみた](https://www.tetsis.com/blog/asp-net-core-signalr-group-chat/)  
→
このサンプルはあまり参考にならなかったが、個別に送信する機能を有しているので、そういうのを実装する時になったらやくに立つのでは。  

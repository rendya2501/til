# .Net6 + ASP.NET Core SignalR における最小実装

大体[ここ(SignalR Core復習)](https://shuhelohelo.hatenablog.com/entry/2020/02/10/162108)参考にした。  
かなり良い記事なのだが、一番上に出てこないタイプだったので、たどり着くまでに苦労した。  

---

## 開発環境

- Windows 10以降
- .Net6
- VisualStudio2022
- サーバー側
  - ASP.NET Core Minimal API
- クライアント側
  - コンソールアプリ

---

## プロジェクトの用意

MinimalSampleでソリューションを作成。  
サーバーサイドは`Server`で作成。  
クライアントは`Client`で作成。  

Dotnetコマンドで作成するなら以下の通り  

ソリューションの作成  
`dotnet new sln -n MinimalSample`  

MinimalAPIプロジェクトを作成する。  
`dotnet new web -o Server -f net6.0`

コンソールアプリプロジェクト作成を作成する。  
`dotnet new console -o Client -f net6.0`  

ソリューションにプロジェクトを追加  
`dotnet add sln ./Server/Server.csproj`  
`dotnet add sln ./Client/Client.csproj`  

---

## サーバ側の実装

### ライブラリ

ASP.NET Coreは最初からSignalRをサポートしているのでnugetからライブラリをインストールする必要はない。  

### Hubの実装

クライアントから呼び出す関数の定義を行う。  

``` C#
using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs;

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
```

### StartUp

SignalRサービスの登録を行う。  

``` cs
using Server.Hubs;

var builder = WebApplication.CreateBuilder(args);
// SignalRサービス追加
builder.Services.AddSignalR();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
// SignalR Hubの登録
app.MapHub<ChatHub>("/chatHub");

app.Run();
```

---

## クライアント側の実装

### SignalRライブラリのインストール  

nugetから以下のライブラリをインストールする。  

- SignalR.Client  

Dotnetコマンドでインストールするなら以下のコマンドを実行する。  
`dotnet add package SingalR.Client`  

### Program.csの実装  

``` cs
using Microsoft.AspNetCore.SignalR.Client;

Console.Write("What is your name? : ");
var yourname = Console.ReadLine();

// 1. Hub へのコネクション
await using var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7265/chatHub")
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
```

---

## 動作確認

VisualStudioの場合  

- Serverプロジェクトを右クリック  
- デバッグ→新しいインスタンスの開始をクリック。  

- Clientプロジェクトを右クリック  
- デバッグ→新しいインスタンスの開始をクリック。  

もう一度同じ動作を行って、ターミナルを立ち上げる。  

2つのターミナルでそれぞれで名前を入力して適当に発言すると、もう片方のターミナルにも反映されることが確認できるはず。  

VisualStudioCodeで開発している場合は、`dotnet run`でそれぞれのプロジェクトを実行すれば良い。  

---

## SignalRのサービス確認

<https://localhost:~~~~/chatHub> にアクセスして、 `Connection ID required` と表示されれば動作確認はOK。  

---

## サーバーサイドの別の実装

マイクロソフト公式、及び[このサイト(ASP.NET Core SignalR でルーム付きチャットアプリを作ってみた)](https://www.tetsis.com/blog/asp-net-core-signalr-group-chat/)では以下のように設定すべしと紹介されているが、nugetだけで十分であることがわかった。  
一応この通りにやっても動くので備忘録として残しておく。  

- ソリューションエクスプローラーで、プロジェクトを右クリックし、[追加] -> [クライアント側のライブラリ]を選択する  
- [クライアント側のライブラリを追加します]ダイアログで以下の通り選択する  
  - プロバイダー: unpkg  
  - ライブラリ：@microsoft/signalr@latest  
  - [特定のファイルの選択]を選択する  
  - dist/browser フォルダを展開し、 signalr.js と signalr.min.js を選択する  
  - [ターゲットロケーション]に wwwroot/js/signalr/ と入力する  
  - [インストール]をクリック  

---

## 躓いたところ

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

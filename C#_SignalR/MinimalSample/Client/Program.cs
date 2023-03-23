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

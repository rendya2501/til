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

# SignalR

[Microsoft_ASP.NET Core 用 SignalR でハブを使用する](https://docs.microsoft.com/ja-jp/aspnet/core/signalr/hubs?view=aspnetcore-6.0)  
[Microsoft_チュートリアル: ASP.NET Core SignalR の概要](https://docs.microsoft.com/ja-jp/aspnet/core/tutorials/signalr?view=aspnetcore-6.0&tabs=visual-studio)  
[Microsoft_ASP.NET Core SignalR .NET クライアント](https://docs.microsoft.com/ja-jp/aspnet/core/signalr/dotnet-client?view=aspnetcore-6.0&tabs=visual-studio)  

[SignalR でChat アプリを作ってみた](https://qiita.com/TsuyoshiUshio@github/items/65ea4e2669afa19f6a31)  
[ASP.NET Core SignalR でルーム付きチャットアプリを作ってみた](https://www.tetsis.com/blog/asp-net-core-signalr-group-chat/)  
→
このサンプルはあまり参考にならなかったが、個別に送信する昨日を有しているので、そういうのを実装する時になったらやくに立つのでは。  

## SignalR 概要

[SignalR を Windows Mixed Reality で使いたい](https://blog.okazuki.jp/entry/2018/04/26/125929)  

- リアルタイムの双方向通信を簡単におこなうライブラリ  
- WebSocket とかをいい感じに隠してくれてサーバーからクライアント（Webやネイティブアプリなど）に対して処理を実行することが出来る  
- SignalRの通信は関数名を指定することで行う  
- Web側にHubを実装することで実現できる  

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

### サーバ実装

[1] プロジェクト作成  

[ASP.NET Core(空)] プロジェクトを選択する。  
.Net6 を選択して作成。  

※  
[ASP.NET Core Web API] でも動作することを確認した。  
こちらは実務で使っているSwaggerが起動するタイプのプロジェクト。  
やることも以下の手順と同じようにやればOK。  
しかし、ただ、動かすだけなら余計なモノが何も入っていない [ASP.NET Core] の空プロジェクトで十分。  

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

### クライアント側実装

[1] プロジェクト作成  

コンソールアプリで十分なので、.Net6でコンソールアプリプロジェクトを新規で作成する。  

[2] nugetからSignalRライブラリをインストール  

nugetから以下3点をインストール  

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
結論は接続プロトコルがHTTPだったから。本来はHTTPSの必要があった。  
ローカルでの接続だからてっきりHTTPだと思っていたけど、HTTPSだった。  
スキームをHTTPSにしてつないだら問題なく実現できた。  

プロトコルはHTTPかHTTPSか確認すること。  

---

## 実務コード

SignalR発火の起点はAccessorによる受信か？  
それをどうやってViewModelはイベントとして感知している？  

``` C# : Web Hub
    /// <summary>
    /// スタートハブ
    /// </summary>
    public class StartHub : Microsoft.AspNetCore.SignalR.Hub
    {
        /// <summary>
        /// 更新プレイヤーを送信します。
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task SendUpdateStartPlayer(IEnumerable<UpdateParam<TSheetStartPlayer>> param)
        {
            return Clients.All.SendAsync("ReceiveUpdateStartPlayer", param);
        }

        /// <summary>
        /// 更新枠を送信します。
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task SendUpdateStartFrame(string param)
        {
            return Clients.All.SendAsync("ReceiveUpdateStartFrame", param);
        }

        /// <summary>
        /// 更新予約排他を送信します。
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task SendUpdateReservationExclusive(IEnumerable<UpdateParam<TSheetReservationExclusive>> param)
        {
            return Clients.All.SendAsync("ReceiveUpdateReservationExclusive", param);
        }

        /// <summary>
        /// 入替スタートを送信します。
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task SendSwapStart(IEnumerable<PlayerSwapParam> param)
        {
            return Clients.All.SendAsync("ReceiveSwapStart", param);
        }
    }

    /// <summary>
    /// 予約ハブ
    /// </summary>
    public class ReservationHub : Microsoft.AspNetCore.SignalR.Hub
    {
        /// <summary>
        /// 更新予約を送信します。
        /// </summary>
        /// <param name="group"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task SendUpdateReservation(DateTime group, IEnumerable<UpdateParam<TSheetReservationPlayer>> param)
        {
            return Clients.Group(ConvertGroupName(group)).SendAsync("ReceiveUpdateReservation", param);
        }

        /// <summary>
        /// 更新予約枠を送信します。
        /// </summary>
        /// <param name="group"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task SendUpdateReservationFrame(DateTime group, string param)
        {
            return Clients.Group(ConvertGroupName(group)).SendAsync("ReceiveUpdateReservationFrame", param);
        }

        /// <summary>
        /// 更新予約排他を送信します。
        /// </summary>
        /// <param name="group"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task SendUpdateReservationExclusive(DateTime group, IEnumerable<UpdateParam<TSheetReservationExclusive>> param)
        {
            return Clients.Group(ConvertGroupName(group)).SendAsync("ReceiveUpdateReservationExclusive", param);
        }

        /// <summary>
        /// 日付グループを設定する
        /// サーバからの呼び出しは行わないでください。(connectionIDを正しく設定できないため)
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="date"></param>
        public Task AddGroup_BusinessDate(string connectionID, DateTime date)
        {
            //新しい日付グループに入る
            return Groups.AddToGroupAsync(connectionID, ConvertGroupName(date));
        }

        /// <summary>
        /// 日付グループを解除する
        /// サーバからの呼び出しは行わないでください。(connectionIDを正しく設定できないため)
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public Task RemoveGroup_BusinessDate(string connectionID, DateTime date)
        {
            //現在の日付グループから抜ける
            return Groups.RemoveFromGroupAsync(connectionID, ConvertGroupName(date));
        }

        /// <summary>
        /// グループ名に変換します。
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static string ConvertGroupName(DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }
    }
```

``` C# Web StartUp
        /// <summary>
        /// アプリケーションが HTTP 要求にどのように応答するかを設定します。
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="errorHandler"></param>
        /// <remarks>
        /// 要求パイプラインは、ミドルウェア コンポーネントを IApplicationBuilder インスタンスに追加することで構成されます。
        /// IApplicationBuilder は Configure メソッドから使用できますが、サービス コンテナーに登録されていません。
        /// ホスティングによって IApplicationBuilder が作成され、Configure に直接渡されます。
        /// </remarks>
        [Obsolete]
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, ErrorHandler errorHandler)
        {
            if (env.IsDevelopment())
            {
                app.UseStaticFiles();
                app.UseDeveloperExceptionPage();
                // ミドルウェアにSwaggerを登録
                app.UseSwagger();
                // ミドルウェアにSwaggerUIを登録
                app.UseSwaggerUI(setupAction =>
                {
                    // JSONエンドポイントを登録
                    setupAction.SwaggerEndpoint("./accounts_receivable/swagger.json", "RN3 API AccountsReceivable");
                    setupAction.SwaggerEndpoint("./analysis/swagger.json", "RN3 API Analysis");
                    setupAction.SwaggerEndpoint("./caddy/swagger.json", "RN3 API Caddy");
                    setupAction.SwaggerEndpoint("./common/swagger.json", "RN3 API Common");
                    setupAction.SwaggerEndpoint("./competition/swagger.json", "RN3 API Competition");
                    setupAction.SwaggerEndpoint("./front/swagger.json", "RN3 API Front");
                    setupAction.SwaggerEndpoint("./inventory_management/swagger.json", "RN3 API InventoryManagement");
                    setupAction.SwaggerEndpoint("./master/swagger.json", "RN3 API Master");
                    setupAction.SwaggerEndpoint("./member_and_customer/swagger.json", "RN3 API MemberAndCustomer");
                    setupAction.SwaggerEndpoint("./option/swagger.json", "RN3 API Option");
                    setupAction.SwaggerEndpoint("./reservation/swagger.json", "RN3 API Reservation");
                    setupAction.SwaggerEndpoint("./start_up/swagger.json", "RN3 API StartUp");
                    setupAction.SwaggerEndpoint("./sys/swagger.json", "RN3 API System");
                });
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // エラー処理
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(errorHandler.Handle);
            });

            //app.UseHttpsRedirection();

            // 社員名変換ミドルウェア
            app.UseConvertStaffName();
            // アクセスログミドルウェア
            app.UseAccessLog();

            app.UseRouting();

            // 認証ミドルウェア
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ReservationHub>("/reservation");
                endpoints.MapHub<StartHub>("/start");
                endpoints.MapHub<CTIHub>("/cti");
                endpoints.MapHub<AggregateHub>("/aggregate");
            });
        }
```

``` C# : Front Recieve
        /// <summary>
        /// 更新予約枠を受信する関数
        /// </summary>
        /// <param name="paramStr"></param>
        public void ReceiveUpdateReservationFrame(string paramStr)
        {
            var param = JsonConvert.DeserializeObject<IEnumerable<UpdateParam<TSheetReservationFrame>>>(paramStr);
            if (param != null)
            {
                List<TSheetReservationFrame> createList = new List<TSheetReservationFrame>();
                List<TSheetReservationFrame> updateList = new List<TSheetReservationFrame>();
                List<TSheetReservationFrame> deleteList = new List<TSheetReservationFrame>();

                foreach (var p in param)
                {
                    if (p.Value == null)
                    {
                        continue;
                    }

                    if (p.UpdateType == UpdateType.Create)
                    {
                        createList.Add(p.Value);
                    }
                    else if (p.UpdateType == UpdateType.Update)
                    {
                        updateList.Add(p.Value);
                    }
                    else if (p.UpdateType == UpdateType.Cancel || p.UpdateType == UpdateType.Delete)
                    {
                        deleteList.Add(p.Value);
                    }
                }
                if (0 < createList.Count)
                {
                    ReservationFrameCreate?.Invoke(createList);
                }
                if (0 < updateList.Count)
                {
                    ReservationFrameUpdate?.Invoke(updateList);
                }
                if (0 < deleteList.Count)
                {
                    ReservationFrameDelete?.Invoke(deleteList);
                }
            }
        }
```

``` C# : Front SignalRAccessor
namespace RN3.Wpf.Common.Accessor
{
    /// <summary>
    /// SignalRを用いたサーバ接続クラスです。
    /// </summary>
    /// <typeparam name="THub">ハブ</typeparam>
    public class SignalRAccessor<THub> : NotifyPropertyChanged where THub : IHub
    {
        #region プロパティ
        private HubConnectionState _State;
        /// <summary>
        /// 接続状態
        /// </summary>
        public HubConnectionState State
        {
            get { return _State; }
            set { SetProperty(ref _State, value); }
        }

        /// <summary>
        /// ハブ
        /// </summary>
        public THub Hub { get; set; }

        /// <summary>
        /// 再接続イベント
        /// </summary>
        public EventHandler Reconnected;

        /// <summary>
        /// サーバへのコネクション
        /// </summary>
        private HubConnection _Connection = null;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="hub"></param>
        public SignalRAccessor(THub hub)
        {
            Hub = hub;
            try
            {
                if (_Connection == null)
                {
                    //接続先の設定
                    var configConnection = ConfigUtil.GetConfigOrDefault().SelectionConnection;
                    _Connection = new HubConnectionBuilder()
                        .WithUrl(new UriBuilder(configConnection.Host + Hub.Pattern).Uri.OriginalString)
                        .WithAutomaticReconnect()
                        .Build();
                    _Connection.Reconnected -= SignalRAccessor_Reconnected;
                    _Connection.Reconnected += SignalRAccessor_Reconnected;
                    _Connection.Reconnecting -= SignalRAccessor_Reconnecting;
                    _Connection.Reconnecting += SignalRAccessor_Reconnecting;
                    _Connection.Closed -= SignalRAccessor_Closed;
                    _Connection.Closed += SignalRAccessor_Closed;
                    State = _Connection.State;
                }
            }
            catch (Exception e)
            {
                //接続でエラーが発生
                Logger.SaveErrorLog(e);
                //TODO：エラー時の処理
            }
        }

        /// <summary>
        /// 接続を試みます。
        /// </summary>
        /// <returns></returns>
        public async Task ConnectingAsync()
        {
            if (_Connection != null && _Connection.State  == HubConnectionState.Disconnected)
            {
                State = _Connection.State;
                await _Connection.StartAsync();
                State = _Connection.State;
            }
        }

        /// <summary>
        /// 接続を破棄します。
        /// </summary>
        /// <returns></returns>
        public async Task DisConnectingAsync()
        {
            if (_Connection != null && _Connection.State == HubConnectionState.Connected)
            {
                State = _Connection.State;
                await _Connection.StopAsync();
                State = _Connection.State;
            }
        }

        /// <summary>
        /// サーバからクライアントへの処理登録
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="handler"></param>
        public void Register(string methodName, Action handler)
        {
            _Connection.Remove(methodName);
            _Connection.On(methodName, handler);
        }

        /// <summary>
        /// サーバからの受信時処理登録
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="handler"></param>
        public void Register<T>(string methodName, Action<T> handler)
        {
            _Connection.Remove(methodName);
            _Connection.On(methodName, handler);
        }

        /// <summary>
        /// サーバからの受信時処理登録
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="handler"></param>
        public void Register<T1, T2>(string methodName, Action<T1, T2> handler)
        {
            _Connection.Remove(methodName);
            _Connection.On(methodName, handler);
        }

        /// <summary>
        /// サーバからの受信時処理登録
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="handler"></param>
        public void Register<T1, T2, T3>(string methodName, Action<T1, T2, T3> handler)
        {
            _Connection.Remove(methodName);
            _Connection.On(methodName, handler);
        }

        /// <summary>
        /// サーバからの受信時処理登録
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="handler"></param>
        public void Register<T1, T2, T3, T4>(string methodName, Action<T1, T2, T3, T4> handler)
        {
            _Connection.Remove(methodName);
            _Connection.On(methodName, handler);
        }

        /// <summary>
        /// サーバからの受信処理登録解除
        /// </summary>
        /// <param name="methodName"></param>
        public void Unregister(string methodName)
        {
            _Connection.Remove(methodName);
        }

        /// <summary>
        /// クライアントからサーバへ送信
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async void Send(string methodName, params object[] args)
        {
            if (_Connection.State != HubConnectionState.Connected)
            {
                return;
            }
            switch (args.Length)
            {
                case 0:
                    await _Connection.SendAsync(methodName);
                    break;
                case 1:
                    await _Connection.SendAsync(methodName, args[0]);
                    break;
                case 2:
                    await _Connection.SendAsync(methodName, args[0], args[1]);
                    break;
                case 3:
                    await _Connection.SendAsync(methodName, args[0], args[1], args[2]);
                    break;
                case 4:
                    await _Connection.SendAsync(methodName, args[0], args[1], args[2], args[3]);
                    break;
                case 5:
                    await _Connection.SendAsync(methodName, args[0], args[1], args[2], args[3], args[4]);
                    break;
                case 6:
                    await _Connection.SendAsync(methodName, args[0], args[1], args[2], args[3], args[4], args[5]);
                    break;
                case 7:
                    await _Connection.SendAsync(methodName, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                    break;
                case 8:
                    await _Connection.SendAsync(methodName, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
                    break;
                case 9:
                    await _Connection.SendAsync(methodName, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
                    break;
                case 10:
                    await _Connection.SendAsync(methodName, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);
                    break;
                default:
                    throw new Exception("argsの長さが未対応です。");
            }
        }

        /// <summary>
        /// 現在の接続をグループに追加します。
        /// </summary>
        /// <param name="groupName">グループに指定するための項目名</param>
        /// <param name="group">追加するグループ</param>
        public async Task AddGroup(string groupName, object group)
        {
            if (_Connection.State != HubConnectionState.Connected)
            {
                return;
            }
            if (group == null)
            {
                return;
            }
            await _Connection.SendAsync("AddGroup_" + groupName, _Connection.ConnectionId, group);
        }

        /// <summary>
        /// 現在の接続をグループに追加します。
        /// </summary>
        /// <param name="groupName">グループに指定するための項目名</param>
        /// <param name="oldGroup">破棄するグループ</param>
        /// <param name="newGroup">追加するグループ</param>
        public async void AddGroup(string groupName, object oldGroup, object newGroup)
        {
            if (_Connection.State != HubConnectionState.Connected)
            {
                return;
            }
            await RemoveGroup(groupName, oldGroup);
            await AddGroup(groupName, newGroup);
        }

        /// <summary>
        /// 現在の接続をグループから解除します。
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task RemoveGroup(string groupName, object group)
        {
            if (_Connection.State != HubConnectionState.Connected)
            {
                return;
            }

            if (group == null)
            {
                return;
            }
            await _Connection.SendAsync("RemoveGroup_" + groupName, _Connection.ConnectionId, group);
        }

        /// <summary>
        /// 再接続処理
        /// </summary>
        /// <param name="connectionID"></param>
        /// <returns></returns>
        private Task SignalRAccessor_Reconnected(string connectionID)
        {
            State = _Connection.State;
            Reconnected?.Invoke(this, new EventArgs());
            return Task.CompletedTask;
        }

        /// <summary>
        /// 再接続中処理
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task SignalRAccessor_Reconnecting(Exception arg)
        {
            State = _Connection.State;
            return Task.CompletedTask;
        }
        /// <summary>
        /// 再接続中処理
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task SignalRAccessor_Closed(Exception arg)
        {
            State = _Connection.State;
            return Task.CompletedTask;
        }
    }
}
```

``` C#
namespace RN3.Wpf.Front.StartTSheet
{
    /// <summary>
    /// アプリケーション情報を扱います。
    /// </summary>
    public class AppInfo : IAppInfo
    {
        /// <summary>
        /// クラス登録
        /// </summary>
        /// <param name="containerRegistry"></param>
        public void RegisterTypes(IUnityContainer containerRegistry)
        {
            containerRegistry.RegisterType<StartHub>()
                .RegisterType<SignalRAccessor<StartHub>>()
                .RegisterType<ListServiceAdapter>()
                .RegisterType<PrintSettingServiceAdapter>();
        }
    }
}

        private SignalRAccessor<AggregateHub> _AggregateAccessor;
        /// <summary>
        /// SignalRアクセサAggregateHub
        /// </summary>
        [Dependency]
        public SignalRAccessor<AggregateHub> AggregateAccessor
        {
            get { return _AggregateAccessor; }
            set { SetProperty(ref _AggregateAccessor, value); }
        }

        #region RAccessor
        private SignalRAccessor<ReservationHub> _RAccessor;
        /// <summary>
        /// SignalRアクセサ
        /// </summary>
        [Dependency]
        public SignalRAccessor<ReservationHub> RAccessor
        {
            get { return _RAccessor; }
            set { SetProperty(ref _RAccessor, value); }
        }
        #endregion
```

# SignalR

発火させたい関数名を指定する。  
SignalR用のアクセサーが必要。  
Hubなるものの実装は必須。  

SignalR発火の起点はAccessorによる受信か？  
それをどうやってViewModelはイベントとして感知している？  

[ASP.NET Core SignalR でルーム付きチャットアプリを作ってみた](https://www.tetsis.com/blog/asp-net-core-signalr-group-chat/)  

クライアント実装において、HubConnectionBuilderのWithUrlメソッドが存在しないと言われた。  
nugetで[Microsoft.AspNetCore.SignalR.Client.Core]のインストールが必要であった。  
SignalRのClientとつくものを全てインストールしていればこんな事にはならなかったのだろうけど、初めだから何も知らなくて、そういうチュートリアルもどこにもなく、地味に嵌ったのでメモしておく。  
[WithUrl() not found in Core 3 client - Stack Overflow](https://stackoverflow.com/questions/58677179/withurl-not-found-in-core-3-client)  

---

## 実務コード

``` C# : Web Send
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
    /// 更新予約枠を送信します。
    /// </summary>
    /// <param name="group"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public Task SendUpdateReservationFrame(DateTime group, string param)
    {
        return Clients.Group(ConvertGroupName(group)).SendAsync("ReceiveUpdateReservationFrame", param);
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
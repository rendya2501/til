# Prism.EventAggregator

---

## Prism.EventAggregator概要

>EventAggregator とは Publisher-Subscriber パターンを Prism で実装したもの。  
Publisher-Subscriber パターンとは、アプリケーションの非同期通信を容易にするため設計されたメッセージングのパターンのこと。  
具体的には、リンクすることが困難なコンポーネント間でメッセージをやり取りするといった、問題を解決します。  
Prism のプロジェクトでは、EventAggregator は、2つ以上の ViewModel の間や、お互いに参照を持たないサービスの間でメッセージを送受信するためによく利用される。  
[Prism EventAggregator をなぜ使うべきか](https://shikaku-sh.hatenablog.com/entry/wpf-prism-why-should-use-eventaggregator)  

---

## Prism.EventAggregatorの実装例

``` C# : Publisher
using Prism.Events;

    /// <summary>
    /// Publisherビューモデル
    /// </summary>
    public class PublisherViewModel
    {
        /// <summary>
        /// 決定時EventAggregater発火用コマンド
        /// </summary>
        public DelegateCommand<string> OnDecide { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PublisherViewModel(IEventAggregator eventAggregator)
        {
            // 決定発火イベント
            OnDecide = new DelegateCommand<string>(
                payload => eventAggregator.GetEvent<PubSubEvent<string>>().Publish(payload)
            );
        }

        // ボタンを押下してコマンド実行した呈
        private void OnButton(){
            // 返信コンテキスト生成 & イベント発火
            OnDecide.Execute("Test");
        }
    }
```

``` C# : Subscriber
using Prism.Events;

    /// <summary>
    /// Subscriberビューモデル
    /// </summary>
    public class SubscribeViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SubscribeViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<PubSubEvent<string>>().Subscribe(Callback);
        }

        /// <summary>
        /// Subscribeコールバック
        /// </summary>
        /// <param name="replyContext"></param>
        private void Callback(string replyContext)
        {
            // Test
            MsgBox.Show(replyContext);
        }
    }
```

---

``` C# : Publisher
using Prism.Events;

    /// <summary>
    /// Publisherビューモデル
    /// </summary>
    public class PublisherViewModel : ILinkAccessory
    {
        /// <summary>
        /// 決定時EventAggregater発火用コマンド
        /// </summary>
        public DelegateCommand<string> OnDecide { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PublisherViewModel(IEventAggregator eventAggregator)
        {
            // 決定発火イベント
            OnDecide = new DelegateCommand<string>(
                payload => eventAggregator.GetEvent<NotifyDecideEvent>().Publish(this.CreatePayload(payload))
            );
        }

        // ボタンを押下してコマンド実行した呈
        private void OnButton(){
            // 返信コンテキスト生成 & イベント発火
            OnDecide.Execute("Test");
        }
    }
```

``` C# : EventClass
using Prism.Events;

    /// <summary>
    /// 発火させるイベント
    /// </summary>
    public class NotifyDecideEvent : PubSubEvent<LinkEventArg<string>>
    {
    }
```

``` C# : Subscriber
using Prism.Events;

    /// <summary>
    /// Subscriberビューモデル
    /// </summary>
    public class SubscribeViewModel : IUsageLink
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SubscribeViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<NotifyDecideEvent>().Subscribe(Callback);
        }

        /// <summary>
        /// Subscribeコールバック
        /// </summary>
        /// <param name="replyContext"></param>
        private void Callback(LinkEventArg<string> replyContext)
        {
            // Test
            MsgBox.Show(replyContext.Payload);
        }
    }
```

``` C# : ILinkAccessory
    /// <summary>
    /// 接続付属インターフェース
    /// </summary>
    public interface ILinkAccessory
    {
        /// <summary>
        /// キー
        /// </summary>
        int LinkedKey { get; set; }
    }

    /// <summary>
    /// ILinkAccessory拡張クラス
    /// </summary>
    public static class LinkAccessoryExtension
    {
        /// <summary>
        /// ペイロードを生成します。
        /// </summary>
        /// <param name="accessory"></param>
        /// <returns></returns>
        public static LinkEventArg CreatePayload(this ILinkAccessory accessory)
        {
            return new LinkEventArg(accessory.LinkedKey);
        }

        /// <summary>
        /// ペイロードを生成します。
        /// </summary>
        /// <typeparam name="TPayload"></typeparam>
        /// <param name="accessory"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static LinkEventArg<TPayload> CreatePayload<TPayload>(this ILinkAccessory accessory, TPayload payload)
        {
            return new LinkEventArg<TPayload>(payload, accessory.LinkedKey);
        }

        /// <summary>
        /// ペイロードを購読可能か判定する
        /// </summary>
        /// <param name="accessory"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static bool IsSubscribable(this ILinkAccessory accessory, LinkEventArg payload)
        {
            return payload?.KeyEquals(accessory.LinkedKey) ?? false;
        }
    }
```

``` C# : IUsageLink
    /// <summary>
    /// Linkを使用するためのインターフェース
    /// </summary>
    public interface IUsageLink { }

    /// <summary>
    /// IUsageLink拡張クラス
    /// </summary>
    public static class UsageLinkExtension
    {
        /// <summary>
        /// ペイロードを生成します。
        /// </summary>
        /// <param name="usagePayload"></param>
        /// <returns></returns>
        public static LinkEventArg CreatePayload(this IUsageLink usagePayload)
        {
            return new LinkEventArg(usagePayload.GetHashCode());
        }

        /// <summary>
        /// ペイロードを生成します。
        /// </summary>
        /// <typeparam name="TPayload"></typeparam>
        /// <param name="usagePayload"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static LinkEventArg<TPayload> CreatePayload<TPayload>(this IUsageLink usagePayload, TPayload payload)
        {
            return new LinkEventArg<TPayload>(payload, usagePayload.GetHashCode());
        }

        /// <summary>
        /// ペイロードを購読可能か判定する
        /// </summary>
        /// <param name="usagePayload"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static bool IsSubscribable(this IUsageLink usagePayload, LinkEventArg payload)
        {
            return payload?.KeyEquals(usagePayload.GetHashCode()) ?? false;
        }
    }
```

``` C# : LinkEventArg
    /// <summary>
    /// リンクイベント引数
    /// </summary>
    public class LinkEventArg
    {
        /// <summary>
        /// 一意のキー
        /// </summary>
        public int UniqueKey { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LinkEventArg() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key"></param>
        public LinkEventArg(int key)
        {
            UniqueKey = key;
        }

        /// <summary>
        /// キーの評価
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyEquals(int key)
        {
            return UniqueKey == key;
        }
    }

    /// <summary>
    /// リンクイベント引数
    /// </summary>
    public class LinkEventArg<TPayload> : LinkEventArg
    {
        /// <summary>
        /// ペイロード
        /// </summary>
        public TPayload Payload { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LinkEventArg() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="payload"></param>
        public LinkEventArg(TPayload payload)
        {
            Payload = payload;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="key"></param>
        public LinkEventArg(TPayload payload, int key) : base(key)
        {
            Payload = payload;
        }
    }
```

---

## 参考

[Prism EventAggregator をなぜ使うべきか](https://shikaku-sh.hatenablog.com/entry/wpf-prism-why-should-use-eventaggregator)  
[Prism入門 その5 - IEventAggregator](https://qiita.com/swd/items/2910c1d87a4c54f16077)  

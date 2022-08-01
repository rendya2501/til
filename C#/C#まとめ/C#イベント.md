# イベント

---

## 概要

[1]  
デリゲートによるコールバックメカニズムに、安全な購読と購読解除機能を追加するための文法  

[2]  
イベントとは通知である。  
通知されたら、対応した処理を実行する仕組みである。  

[C#のイベント機能](https://dobon.net/vb/dotnet/vb2cs/event.html)  
[[C#] イベント入門](https://qiita.com/laughter/items/e9cf666e0430acc39e95)  

---

## とりあえず最小実装

1. イベント定義  
2. 処理を定義  
3. イベントに処理を登録  
4. イベント発火  
5. 登録した処理の実行  

``` C#
    class EventTest
    {
        // 1. イベント定義  
        private event EventHandler TestEvent;

        // 2. 処理を定義  
        private void DoSomething(object sender, EventArgs e)
        {
            // 5. 登録した処理の実行  
            Console.WriteLine("Event!");
        }

        // 3. イベントに処理を登録  
        public EventTest()
        {
            TestEvent += DoSomething;
        }

        // 4. イベント発火
        public void OnRaiseEvent()
        {
            TestEvent?.Invoke(this, new EventArgs());
        }
    }
```

---

## 任意の値を通知先に渡すタイプのイベント

ジェネリックタイプのEventHandlerを使うとよろしい。  
`EventHandler<TEventArgs>`  

1. ジェネリックタイプのイベントを定義  
2. 処理を定義  
3. イベントに処理を登録  
4. イベント発火  
5. 登録した処理の実行  

``` C#
    class EventTest
    {
        // 1. ジェネリックタイプのイベントを定義
        private event EventHandler<string> TestEvent;

        // 2. 処理を定義
        private void DoSomething(object sender, string msg)
        {
            // 5. 登録した処理の実行
            Console.WriteLine(msg);
        }

        // 3. イベントに処理を登録
        public EventTest()
        {
            TestEvent += DoSomething;
        }

        // 4. イベント発火
        public void OnRaiseEvent()
        {
            TestEvent?.Invoke(this, "Event!");
        }
    }
```

---

## 任意の値を通知先に渡すタイプのイベント2

独自のEventArgsとEventHandlerを定義するタイプの実装。  
ジェネリックタイプの実装ができる今、実装する意味はまったくないけど、備忘録として残しておく。  

1. EventArgsの派生クラスを定義  
2. オリジナルのEventHandlerを定義  
3. イベント定義  
4. 処理を定義  
5. イベントに処理を登録  
6. イベント発火  
7. 登録した処理の実行  

``` C#
    // 1. EventArgsの派生クラスを定義
    public class HogeEventArgs : EventArgs { 
        public string Message; 
    }

    public class EventTest
    {
        // 2. オリジナルのEventHandlerを定義
        public delegate void HogeEventHandler(object sender, HogeEventArgs e);

        // 3. イベント定義  
        private event HogeEventHandler TestEvent;

        // 4. 処理を定義  
        private void DoSomething(object sender, HogeEventArgs e)
        {
            // 7. 登録した処理の実行  
            Console.WriteLine(e.Message);
        }

        // 5. イベントに処理を登録
        public EventTest()
        {
            TestEvent += DoSomething;
        }

        // 6. イベント発火
        public void OnRaiseEvent()
        {
            TestEvent?.Invoke(this, new HogeEventArgs { Message = "終わったよ。" });
        }
    }
```

>EventArgsの派生クラスを用いてデータを返していたが、必ずしもそうする必要はない。  
>しかし、EventArgsを使った方法が.NETでは推奨されているので余程のことがない限りは従うべき。  

と、メモしているが、一体何だったか。

---

## イベントのOverride

・Overrideしたイベントは 「-=」や「+=」で登録、解除はできない。  
なので発動させたくなかったらフラグ使って、returnしたりして実行されないようにする必要がある。  

[C# eventのオーバーライドと基底クラスで発生するイベントの処理](https://opcdiary.net/c-event%E3%81%AE%E3%82%AA%E3%83%BC%E3%83%90%E3%83%BC%E3%83%A9%E3%82%A4%E3%83%89%E3%81%A8%E5%9F%BA%E5%BA%95%E3%82%AF%E3%83%A9%E3%82%B9%E3%81%A7%E7%99%BA%E7%94%9F%E3%81%99%E3%82%8B%E3%82%A4%E3%83%99/)  

もしかしてこの方法の通りにやればそんなことする必要ないかも？  
余裕があれば次の機会に照らし合わせて見てみたい。  

# イベント

[C#のイベント機能](https://dobon.net/vb/dotnet/vb2cs/event.html)  

イベントは要するにデリゲートである。  
そのイベントが発火したときに何をやらせたいのか、その処理を登録するだけ。  

``` C# : 基本的なイベント
Console.WriteLine("ボタンを押した体");
SleepClass clsSleep = new();
// ① イベントハンドラの追加
// イベントが発生した時に実行したい処理を登録する。
clsSleep.Time += new EventHandler(SleepClass_Time);
// ② 処理開始
clsSleep.Start();

/// <summary>
/// イベントが発生したときに呼び出されるメソッド
/// </summary>
void SleepClass_Time(object sender, EventArgs e)
{
    // ⑥ 登録された処理として、画面にハローワールドが表示される。
    Console.WriteLine("Hello, World!");
}

public class SleepClass
{
    // データを持たないイベントデリゲートの宣言
    // ここでは"Time"というイベントデリゲートを宣言する
    public event EventHandler? Time;

    /// <summary>
    /// イベントを発火させるメソッド
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnTime(EventArgs e)
    {
        // ⑤ Timeに紐づけられた処理が実行される。
        // 今回の例の場合SleepClass_Timeが発動する。
        Time?.Invoke(this, e);
    }

    /// <summary>
    /// 処理を開始する。
    /// 1秒後にイベント発火
    /// </summary>
    public void Start()
    {
        // ③ 1秒待つ
        Thread.Sleep(1000);
        // ④ イベントを発火させるメソッド
        OnTime(EventArgs.Empty);
        // 別にここにこう書いてもいい
        //  Time?.Invoke(this, e);
    }
}
```

``` C# : 値が帰ってくるサンプル
Console.WriteLine("値が帰ってくるサンプル");
SleepClass2 SleepClass2 = new();
SleepClass2.Time += new SleepClass2.TimeEventHandler(SleepClass_Time2);
SleepClass2.Start();

/// <summary>
/// 値が帰ってくるイベントのサンプルクラス
/// </summary>
public class SleepClass2
{
    //デリゲートの宣言
    //TimeEventArgs型のオブジェクトを返すようにする
    public delegate void TimeEventHandler(object sender, TimeEventArgs e);

    //イベントデリゲートの宣言
    public event TimeEventHandler? Time;

    protected virtual void OnTime(TimeEventArgs e) => Time?.Invoke(this, e);

    public void Start()
    {
        Thread.Sleep(1000);
        //返すデータの設定
        TimeEventArgs e = new();
        e.Message = "終わったよ。";
        //イベントの発生
        OnTime(e);
    }
}

/// <summary>
/// Timeイベントで返されるデータ
/// ここではstring型のひとつのデータのみ返すものとする
/// </summary>
/// <remarks>
/// EventArgsの派生クラスを用いてデータを返していたが、必ずしもそうする必要はない。
/// しかし、EventArgsを使った方法が.NETでは推奨されているので余程のことがない限りは従うべき。
/// </remarks>
public class TimeEventArgs : EventArgs{ public string Message; }
```

---

## イベントのOverride

・Overrideしたイベントは 「-=」や「+=」で登録、解除はできない。  
なので発動させたくなかったらフラグ使って、returnしたりして実行されないようにする必要がある。  

[C# eventのオーバーライドと基底クラスで発生するイベントの処理](https://opcdiary.net/c-event%E3%81%AE%E3%82%AA%E3%83%BC%E3%83%90%E3%83%BC%E3%83%A9%E3%82%A4%E3%83%89%E3%81%A8%E5%9F%BA%E5%BA%95%E3%82%AF%E3%83%A9%E3%82%B9%E3%81%A7%E7%99%BA%E7%94%9F%E3%81%99%E3%82%8B%E3%82%A4%E3%83%99/)  

もしかしてこの方法の通りにやればそんなことする必要ないかも？  
余裕があれば次の機会に照らし合わせて見てみたい。  

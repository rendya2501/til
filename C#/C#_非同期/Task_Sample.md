# Task実行集

## 基本

TaskクラスはStaticなので宣言する必要はなく、Runさせたい時はTask.Run()でよろしい。  
New Task().Run()はできない。  

Newの意味合いは実行したいTaskを作ることになる。  
Task型は仕事を意味する型なので、オブジェクト作ってもその仕事をRunできない。  
できてWaitさせることくらい。  

1. Taskを並列実行する
2. Taskの中で非同期処理を実行して、その非同期が終わってから次の処理を実行する
3. メソッド単位でTaskを定義する

```C# : 並列実行
private void Test() {
    var task1 = New Task();
    var task2 = New Task();
    Task.Run(task1,task2).WaitAll()
}
```

```C# : Taskの中で非同期処理を実行して、その非同期が終わってから次の処理を実行する
private void Test() {
    // この地点で非同期になる
    Task.Run(
        async () =>
        {
            Product data = null;
            // 非同期検索処理
            await ServiceErrorHandlingAsync(
                async () => data = await ServiceAdapter.GetProductAsync(key),
                nameof(IsBusy)
            );
            // 非同期検索処理が終わってから次へ行く
            HogeHoge(data);
        }
    );
}
```

```C# : メソッド単位でTaskを定義する
class Program
{
    /// <summary>
    /// https://tech-lab.sios.jp/archives/15711
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        var task = HeavyMethod1(); // ①処理の実行
        // ④処理が戻るのでHeavyMethod2が実行される
        HeavyMethod2();
        Console.WriteLine(task.Result);
        Console.ReadLine();
    }
    static async Task<string> HeavyMethod1()
    {
        Console.WriteLine("すごく重い処理その1(´・ω・`)始まり");
        // ②重い処理の実行
        // ③一度実行もとに戻る。その間HeavyMethod1の処理は続行される(バックグラウンド)。
        await Task.Delay(3000); 
        Console.WriteLine("すごく重い処理その1(´・ω・`)終わり");
        // ⑤バックグラウンド(別スレッド)で動いているHeavyMethod1の重い処理が終了すると、
        // HeavyMethod2が実行中でもHeavyMethod1に戻り、hogeを返す。
        return "hoge";
    }
    static void HeavyMethod2()
    {
        Console.WriteLine("すごく重い処理その2(´・ω・`)始まり");
        Thread.Sleep(3000);
        Console.WriteLine("すごく重い処理その2(´・ω・`)終わり");
    }
}
```

Taskを単体で実行させる場合は、単純にTask.Runさせるだけで、その部分だけ非同期で実行される。  
.Resultなどで結果を取得しない限り、いつ終わったのかを観測できないが、処理自体は勝手に実行される。  

``` C#
    // Taskの実行
    Task.Run(() => {
        Console.WriteLine("task1開始");
        Task.Delay(5000);
        Console.WriteLine("task1終了");
    });

    // Taskの実行
    Task.Run(() => {
        Console.WriteLine("task2開始");
        Task.Delay(3000);
        Console.WriteLine("task2終了");
    });

    Console.WriteLine("tas111111終了");

    // 結果が毎回違うので、Task.Runだけで非同期実行されていることが分かる。

    //task2開始
    //tas111111終了
    //task1開始
    //task2終了
    //task1終了

    //task1開始
    //task2開始
    //tas111111終了
    //task1終了
    //task2終了
```

---

## whenAllで待機する方法

``` cs
/// <summary>
/// データの取得処理(キー)
/// </summary>
private async Task<(object obj1, object obj2)> GetData(Key1 key1, Key2 key2)
{
    Task<object> obj1 = ServiceAdapter.Get1(key1);
    Task<object> obj2 = ServiceAdapter.GetAsync(key2);

    await Task.WhenAll(obj1, obj2);

    return (obj1.Result, obj2.Result);
}
```

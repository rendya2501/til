# Task

TaskクラスはStaticなので宣言する必要はなく、Runさせたい時はTask.Run()でよろしい。  
New Task().Run()はできない。  

Newの意味合いは実行したいTaskを作ることになる。  
Task型は仕事を意味する型なので、オブジェクト作ってもその仕事をRunできない。  
できてWaitさせることくらい。  

とっさのことだったがこれに対応できなかった。  
Task使用例を作って簡単にまとめたい。  
→  
2020/12/19 Satようやくまとめた。  
パターンとしては3つだろうか。  

1. Taskを並列実行する
2. Taskの中で非同期処理を実行して、その非同期が終わってから次の処理を実行する
3. メソッド単位でTaskを定義する

```C#
private void Test() {
    var task1 = New Task();
    var task2 = New Task();
    Task.Run(task1,task2).WaitAll()
}
```

```C#
private void Test() {
    // この地点で非同期になる
    Task.Run(
        async () =>
        {
            var key = new TMa_ProductKey
            {
                OfficeCD = Data.Product.OfficeCD,
                DepartmentCD = Data.Product.DepartmentCD.Value,
                ProductCD = setProduct.ProductCD
            };
            TMa_Product data = null;
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

```C#
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

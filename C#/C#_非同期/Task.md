# Task

---

## Taskとは

Task は非同期処理のことではない。  
名の通り「仕事、課題、作業の最小単位」。  

タスクAがタスクBの完了を待ちたい場合、これらの状態を知りたいはずです。  

- 正常終了したかどうか  
- 例外が発生して異常終了したかどうか  
- 処理が途中でキャンセルされたかどうか  

また、タスクBに戻り値がある場合、タスクBが完了したらその値ももらいたいはずです。  

このときに登場するのがTask型です。  
Taskオブジェクトはこれらの状態と戻り値を教えてくれます。  

``` C#
public int RunTaskA()
{
    Task taskB = RunTaskBAsync(); // Task型のオブジェクトを返す
    
    bool status1 = taskB.IsCompletedSuccessfully; // 正常終了したかどうか
    bool status2 = taskB.IsFaulted; // 例外が発生して異常終了したかどうか
    bool status3 = taskB.IsCanceled; // 処理が途中でキャンセルされたかどうか
    var returnValue = taskB.Result; // 戻り値(ある場合)
}
```

[C#_Task とは](https://koshinran.hateblo.jp/entry/2018/05/12/132407)  
[async/await を完全に理解する](https://zenn.dev/vatscy/articles/ba2263bdfadfeb805379)  
[async/await を完全に理解してからもう少し理解する](https://zenn.dev/vatscy/articles/d4782637dd4257ef9822)  

---

## Task.Run

「タスクを生成し開始する」という意味。  
実際の処理は別のだれかが裏で行う。  

``` C#
// 変数 task は「HogeA を実行後 HogeB を実行する、という『タスク』を作成してそれを開始したもの」を表わす。  
var task = Task.Run(() =>
{
    HogeA();
    HogeB();
});
```

### Task.Run の使い方

同期的な一連の処理を、一つのタスクとみなす。

非同期タスクを組み合わせ、合成して、一つのタスクを組み上げるのが非同期メソッドだとしたら、その中に同期処理もタスクの一つとして組み込みたい時に使う。  

もう一つ別の使い方はTask.Runの中に非同期ラムダ式を書くパターン。  
これは、単純にその非同期メソッドを呼び出しているというもの。  

これ以外の使い方をしているなら、それはTask.Runの濫用。  
・同期処理を Task.Run でラップした場合「スレッドプール上で動作する一連の処理」となる。  

---

スレッド：
プログラムの処理の実行単位（ のひとつ ）。
タスクやプロセスより細かい処理の実行単位が「スレッド」。
プロセスのサブスレッドとして動作し、
実行に必要なメモリ空間を複数のスレッドで共有。

マルチスレッド：
A という処理と B という処理を同時に行える。( 並行処理 )

スレッドプール：
スレッドの増加は ( スレッドを生成したり消したりして ) パフォーマンスへの影響が非常に大きい。
故に、1 度作ったスレッドを可能な限り使いまわす仕組み。
つまり、あらかじめスレッドを生成しておき、消さずに再利用する。
1 ) 一定数のスレッドを常に立てておく。
2 ) タスクを持たせておくためのキューを持つ。
3 ) 処理中のタスクが終わり次第、次のタスクをキューから取り出して実行する。

プロセス：
プログラム実行の為の固有のメモリ空間を持っていおり、独立性の高い実行単位。
単体で実行可能なプログラムの単位。
複数のプロセスが存在する時は、それぞれ独立したアドレス空間を持つ。

ファイバ
スレッドをさらに軽量化したもの。

タスク：
曖昧。
1 ) プロセスと同じ意味。
2 ) スレッドのことを指す。
3 ) プロセスよりも大きな集合を指す。

---

## Task.Run と Task.Factory.StartNew

いつもと同じ感覚でTask.Run()で処理を定義しようとしたらできなくて、何かないかと探したらTask.Factroy.StartNewなる構文だといけることが分かった。  
それはいいのだが、そもそもこれは何なのか分からなかったのでまとめることにした。  

``` C#
Task thread1 = Task.Factory.StartNew(() => fun1());
Task thread2 = Task.Factory.StartNew(() => fun2());
```

``` txt
項目                 Task.Run      StartNew  戻り値         備考

CancellationToken    ○             ○       -              キャンセルに使うTokenを指定
TaskCreationOptions  ×             ○       -              どのように作成するかを指定するオプション
TaskScheduler        ×             ○       -              同期コンテキストを指定

Action               ○             ○       Task
Func<TResult>        ○             ○       Task<TReuslt>
Action<object>       ×             ○       Task           第2引数にOjbectが必須
Func<Object,TResult> ×             ○       Task<TReuslt>
Func<Task>           ○             ×       Task           入れ子のTaskの内側が戻る
Func<Task<TResult>>  ○             ×       Task<TReuslt>
```

Task.Facotry.StartNewを一般的な用途でもっと手早く使うためにTask.Runが作られたようだ。  
確かに、タイプ数が少ないし使い勝手が良い。自分も多用している。  
ただ、Task.RunのTaskCreationOptionが「DenyChildAttach」なので、Task.Runの中でTask.Factory.StartNewを「AttachedToParent」で使っても無効になってしまうことには注意が必要。  
また、Task.Runは手軽だけど細かい設定はできない。  
Task.Factory.StartNewが不要になったわけではない。  

[Task.Run と Task.Factory.StartNew](http://outside6.wp.xdomain.jp/2016/08/04/post-205/)  

---

## Task実行集

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

## Task.WaitAll and Exceptions

ASP.Net側のTask.WaitAllで実行しているタスクの中でエラーが発生した場合、エラーが`AggregateException`にラッピングされ、単純なメッセージだけが表示されない現象が発生した。  

下記例であれば`Oops`とエラーダイアログで表示されるのだが、`●●EXCEPTION(Oops)`みたいな感じで表示されてしまう。  
単純にTRY CATCHしてInnerExceptionをThrowしなおせば解決はした。  

``` C#
    Task t1 = Task.Factory.StartNew(() => Task.Delay(1000));
    Task t2 = Task.Factory.StartNew(() => {
        Task.Delay(500);
        throw new Exception("Oops");
    });

    try
    {
        Task.WaitAll(t1, t2);
        System.Diagnostics.Debug.WriteLine("All done");
    }
    catch (AggregateException e)
    {
        System.Diagnostics.Debug.WriteLine("Something went wrong");
        throw e.InnerException;
    }
```

[Task.WaitAll and Exceptions](https://stackoverflow.com/questions/4217643/task-waitall-and-exceptions)  

---

## タスクのキャンセル

CancellationTokenSourceを使う。  

サンプルコード1  
実務で実現したい内容  

``` cs
// ダイアログの確認
// 変更理由の確認
// 料金の確認
// 実行

var csource = new CancellationTokenSource();
var task = Task.Run(() =>
    {
        Console.Write("ダイアログの確認");
        var input = Console.ReadLine();
        if (input?.ToLower() is "n" or null)
        {
            csource.Cancel();
        }
    }
)
.ContinueWith(t =>
    {
        Console.WriteLine("Task1 Cancel:{0}", t.IsCanceled);
        Console.WriteLine("Task1 Faulted:{0}", t.IsFaulted);
        Console.Write("変更理由の確認");
        var input = Console.ReadLine();
        if (input?.ToLower() is "n" or null)
        {
            csource.Cancel();
        }
    },
    csource.Token
)
.ContinueWith(t =>
    {
        Console.WriteLine("Task2 Cancel:{0}", t.IsCanceled);
        Console.WriteLine("Task2 Faulted:{0}", t.IsFaulted);
        Console.Write("料金の確認");
        var input = Console.ReadLine();
        if (input?.ToLower() is "n" or null)
        {
            csource.Cancel();
        }
    },
    csource.Token
)
.ContinueWith(t =>
    {
        Console.WriteLine("実行");
    },
    csource.Token
);

Console.WriteLine("Press key any key to cancel\n");
Console.ReadKey();
task.Wait();
Console.WriteLine("\n\nEnd Application");
Console.ReadKey();
```

サンプルコード2

``` cs
// https://qiita.com/ken200/items/32718b558dec7153fd16
var csource = new CancellationTokenSource();
var task = Task.Run(() =>
{
    Console.Write("入力：");
    var input = Console.ReadLine();

    if (input.ToLower() == "cancel")
    {
        csource.Cancel();
    }

    if (input.ToLower() == "error")
    {
        throw new Exception("何らかのエラー");
    }

    if (input.ToLower() == "error2")
    {
        throw new Exception("何らかのエラー2");
    }
})
.ContinueWith(
    t =>
    {
        if (t.IsFaulted && t.Exception.Flatten().InnerExceptions.Any((e) => e.Message == "何らかのエラー2"))
        {
            Console.WriteLine("Task1でタスクの取り消しを行います。");
            csource.Cancel();
        }

        Console.WriteLine("Task1 Cancel:{0}", t.IsCanceled);
        Console.WriteLine("Task1 Faulted:{0}", t.IsFaulted);

        Console.Write("入力2：");
        var input = Console.ReadLine();
        if (input.ToLower() == "cancel")
        {
            csource.Cancel();
        }
    },
    csource.Token
)
.ContinueWith(
    t =>
    {
        Console.WriteLine("Task2 Cancel:{0}", t.IsCanceled);
        Console.WriteLine("Task2 Faulted:{0}", t.IsFaulted);
    },
    csource.Token,TaskContinuationOptions.OnlyOnCanceled,TaskScheduler.Default
)
.ContinueWith(
    t =>
    {
        Console.WriteLine("Task3 Cancel:{0}", t.IsCanceled);
        Console.WriteLine("Task3 Faulted:{0}", t.IsFaulted);
    },
    csource.Token
);

try
{
    task.Wait();
}
catch (AggregateException exc)
{
    foreach (var innnerExc in exc.InnerExceptions)
    {
        Console.WriteLine("エラー:{0} {1}", innnerExc.Message, innnerExc.GetType().FullName);
    }
}

Console.ReadLine();
```

サンプルコード3  
TaskContinuationOptionsの比較。  

``` cs
// https://qiita.com/hiki_neet_p/items/d6b3addda6c248e53ef0#task-%E3%81%AE%E3%82%AD%E3%83%A3%E3%83%B3%E3%82%BB%E3%83%AB
var tokenSource = new CancellationTokenSource();
tokenSource.Cancel();

Task.CompletedTask
    .ContinueWith(task => Console.WriteLine("OK"), TaskContinuationOptions.OnlyOnRanToCompletion);
Task.FromCanceled(tokenSource.Token)
    .ContinueWith(task => Console.WriteLine("NG"), TaskContinuationOptions.OnlyOnRanToCompletion);
Task.FromException(new Exception())
    .ContinueWith(task => Console.WriteLine("NG"), TaskContinuationOptions.OnlyOnRanToCompletion);

Task.CompletedTask
    .ContinueWith(task => Console.WriteLine("NG"), TaskContinuationOptions.NotOnRanToCompletion);
Task.FromCanceled(tokenSource.Token)
    .ContinueWith(task => Console.WriteLine("OK"), TaskContinuationOptions.NotOnRanToCompletion);
Task.FromException(new Exception())
    .ContinueWith(task => Console.WriteLine("OK"), TaskContinuationOptions.NotOnRanToCompletion);

Task.CompletedTask
    .ContinueWith(task => Console.WriteLine("NG"), TaskContinuationOptions.OnlyOnCanceled);
Task.FromCanceled(tokenSource.Token)
    .ContinueWith(task => Console.WriteLine("OK"), TaskContinuationOptions.OnlyOnCanceled);
Task.FromException(new Exception())
    .ContinueWith(task => Console.WriteLine("NG"), TaskContinuationOptions.OnlyOnCanceled);

Task.CompletedTask
    .ContinueWith(task => Console.WriteLine("NG"), TaskContinuationOptions.OnlyOnFaulted);
Task.FromCanceled(tokenSource.Token)
    .ContinueWith(task => Console.WriteLine("NG"), TaskContinuationOptions.OnlyOnFaulted);
Task.FromException(new Exception())
    .ContinueWith(task => Console.WriteLine("OK"), TaskContinuationOptions.OnlyOnFaulted);

Console.ReadLine();
```

サンプルコード4  
ContinueWithのチェーンでOnlyOnCanceledオプションの指定によるキャンセルの観測。  
結局出来なかった。  

``` cs
// https://stackoverflow.com/questions/33156963/task-continuewith-does-not-work-with-onlyoncanceled
CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
var token = cancellationTokenSource.Token;

Task task = Task.Run(() =>
    {
        while (!token.IsCancellationRequested)
        {
            Console.Write("*");
            Thread.Sleep(1000);
        }
    },
    token
).ContinueWith(t =>
    {                                                     //  THIS
        t.Exception.Handle((e) => true);                  //  ISN'T
        Console.WriteLine("You have canceled the task");  //  EXECUTING
    },
    TaskContinuationOptions.OnlyOnCanceled
);

Console.ReadLine();
cancellationTokenSource.Cancel();
task.Wait();

Console.ReadLine();
```

[継続タスクでのキャンセル処理](https://qiita.com/ken200/items/32718b558dec7153fd16)  
[Task.ContinueWith does not work with OnlyOnCanceled](https://stackoverflow.com/questions/33156963/task-continuewith-does-not-work-with-onlyoncanceled)  
[C# 非同期、覚え書き。](https://qiita.com/hiki_neet_p/items/d6b3addda6c248e53ef0)  

[await 中の Task をキャンセルしてみる](https://azyobuzin.hatenablog.com/entry/2014/05/01/210507)  
[【C#】Taskのキャンセル](https://takap-tech.com/entry/2022/05/03/002600)  
[【C#】タスクのキャンセル方法](https://outofmem.hatenablog.com/entry/2014/04/02/014201)  

>キャンセル時のコールバック登録
>
>``` cs
>private void Start()
>{
>    var tokenSource = new CancellationTokenSource();
>    var token = tokenSource.Token;
>    token.Register(() => Debug.Write("canceled"));
>
>    Task.Run(() => HeavyMethod(token), token);
>
>    tokenSource.Cancel();
>}
>```
>
[【C#】async / Taskの中断](https://note.com/fuqunaga/n/n9ff1be3b479e)  

---

## Taskのキャンセルで例外を発生させない方法はないのか？

頑張ればありそうだけど、基本的にトークンを使った方法が一番良い模様。  

c# task cancel no exception  

[Why Do You Need a Cancellation Token in C# for Tasks?](https://hackernoon.com/why-do-you-need-a-cancellation-token-in-c-for-tasks)  

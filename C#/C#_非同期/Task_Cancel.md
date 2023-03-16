# タスクのキャンセル

---

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
    csource.Token,
    TaskContinuationOptions.OnlyOnCanceled,TaskScheduler.Default
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

Task task = Task
    .Run(() =>
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

## 参考

cancellationtokensource
OperationCanceledException
throwifcancellationrequested c#
c# task 強制終了

[TPL入門 (13) - タスクのキャンセル - xin9le.net](https://blog.xin9le.net/entry/2011/08/20/180329)  
[タスクのキャンセル | Microsoft Learn](https://learn.microsoft.com/ja-jp/dotnet/standard/parallel-programming/task-cancellation)  

[c# - What's the purpose of Task.FromCanceled - Stack Overflow](https://stackoverflow.com/questions/67597488/whats-the-purpose-of-task-fromcanceled)  

>継続タスクは、キャンセルするCancellationTokenを受け取ります。これは、継続タスクが未着手のままキャンセルされることを意味する。キャンセルしたくないものには、そのトークンを渡してはいけない。
CancellationTokenは、アクションのグラフ全体を一度にキャンセルするためのものだ。トークンを渡さないことで、キャンセルから除外することができる。
[c# 4.0 - ContinueWith a cancelled Task - Stack Overflow](https://stackoverflow.com/questions/11892315/continuewith-a-cancelled-task)  

[C# 非同期、覚え書き。 - Qiita](https://qiita.com/hiki_neet_p/items/d6b3addda6c248e53ef0#task-%E3%81%AE%E3%82%AD%E3%83%A3%E3%83%B3%E3%82%BB%E3%83%AB)  
[[UniTask] キャンセルされても処理を続ける](https://zenn.dev/murnana/articles/unitask-continue-with-cancel)  

[C#のタスクキャンセル | ftvlog](https://ftvoid.com/blog/post/260)  

[C#_Task の処理を止める - …Inertia](https://koshinran.hateblo.jp/entry/2018/05/17/235911)  
[【C#】Taskのキャンセル - PG日誌](https://takap-tech.com/entry/2022/05/03/002600)  
[Canceling A Running Task](https://www.c-sharpcorner.com/UploadFile/80ae1e/canceling-a-running-task/)  
[【Unity】Taskでメインスレッドを止める・止めない実例集 - はなちるのマイノート](https://www.hanachiru-blog.com/entry/2020/05/26/120000)  

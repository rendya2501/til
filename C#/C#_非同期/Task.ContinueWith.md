# Task.ContinueWith

---

## 概要

Taskが完了したとき、継続して実行させたい処理を指定することができる。  
PromiseでいうところのThenに相当するモノだと思われる。  

---

## サンプルコード

[TPL入門 (10) - タスクの継続](https://blog.xin9le.net/entry/2011/08/19/002113)  
このサイトがとてもわかりやすいので、全部使わせてもらう。  

``` C#
static void Main()
{
    Console.WriteLine("Main : Begin");

    var task = new Task<int>(() =>
    {
        Console.WriteLine("Task1 : Begin");
        var sum = Enumerable.Range(1, 10000).Sum();
        Console.WriteLine("Task1 : End");
        return sum;
    });

    task.Start();
    task.ContinueWith(task1 =>
    {
        Console.WriteLine("Task2 : Begin");
        Console.WriteLine("Sum = {0}", task1.Result);
        Console.WriteLine("Task2 : End");
    });

    Console.WriteLine("Main : End");
    Thread.Sleep(1000);
}
 
//----- 結果
/*
Main : Begin
Main : End
Task1 : Begin
Task1 : End
Task2 : Begin
Sum = 50005000
Task2 : End
*/
```

---

## ContinueWithを使ったワンセンテンステク

このようなダイアログを表示する非同期処理があったとする。  
警告文があれば表示して常にfalseを返却するのだが、以下のようにワンクッション挟まなければならない。  
それがもどかしい。  

``` C#
private async Task<bool> ConfirmAsync()
{
    // 警告文を生成
    string msg = CreateWarningMessage();
    // 警告文があれば、ダイアログを表示する
    if (!string.IsNullOrEmpty(msg))
    {
        // ダイアログを表示している間は処理を止める
        await MessageDialogUtil.ShowWarningAsync(Messenger, msg);
        // 警告なので常にfalse
        return false;
    }
    // 警告文がなければ常にtrue
    return true;
}
```

TaskのContinueWithを使うことでワンセンテンス化が可能になる。  
ContinueWithにより、ダイアログを閉じたときに続けてfalseを返却するように記述することができる。  

``` C#
private async Task<bool> ConfirmAsync()
{
    // 警告文を生成
    string msg = CreateWarningMessage();
    // 警告文がなければtrueでそのまま処理終了。
    // 警告文があれば、ダイアログを表示しつつ、falseを返却する。
    return string.IsNullOrEmpty(msg)
        || await MessageDialogUtil.ShowWarningAsync(Messenger, msg).ContinueWith(_ => false);
}
```

---

## ContinueWithのチェーン

``` cs
Console.WriteLine("処理開始");

if (await ConfirmAsync())
{
    // trueの処理のつもり
    Console.WriteLine("trueです");
}
else
{
    // falseの処理のつもり
    Console.WriteLine("falseです");
}

// 確認処理のつもり
async Task<bool> ConfirmAsync()
{
    var result = await ConfirmRequestAsync(new Object())
        .ContinueWith(async response =>
            // 警告文がなければtrueで処理終了。
            // 警告文があればダイアログを表示して結果をboolで返却する。
            string.IsNullOrEmpty(response.Result) || await ShowDialogAsync(response.Result)
        );
    return result.Result;
}

// RESTで確認処理を叩いた体
async Task<string> ConfirmRequestAsync(object request)
{
    // リクエストを投げてレスポンスを受け取るまでの時間という体
    await Task.Delay(1000);
    // 確認処理の結果、アナウンスがあったという体
    return "警告:○○です";
}

// OK,Cancelダイアログを表示し、ボタンの入力があるまで待機する処理の体
async Task<bool> ShowDialogAsync(string msg)
{
    var result = await Task.Run(() =>
    {
        Console.WriteLine($"{msg}{Environment.NewLine}処理を続行しますか？{Environment.NewLine} y or n");
        return Console.ReadLine() ?? string.Empty;
    });
    return result == "y";
}
```

``` cs
async Task aaa()
{
    await Task.Run(() => "First thing done")
        .ContinueWith(async task => await Task.Run(() => $"{task.Result} _First thing done")).Unwrap()
        .ContinueWith(async task => await Task.Run(() => $"{task.Result}_Third thing done"));
}
_ = aaa();

```

Unwrapメソッドは.NetFrameworkの4.0から対応している模様。  

[TaskExtensions.Unwrap メソッド](https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.tasks.taskextensions.unwrap?view=net-7.0)  
[A basic use-case for Task Unwrap](https://pragmateek.com/a-basic-use-case-for-task-unwrap/)

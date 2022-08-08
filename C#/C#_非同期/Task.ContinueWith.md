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
        // 警告文があれば、ダイアログを表示しつつ、falseを返却する。
        return string.IsNullOrEmpty(msg)
            || await MessageDialogUtil.ShowWarningAsync(Messenger, msg).ContinueWith(f => false);
    }
```

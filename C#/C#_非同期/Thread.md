# Thread

---

## なぜThread.Sleepはだめなのか

Thread.Sleep は、メインスレッドを止める。  
スレッドがブロックされ、事実上のフリーズ状態となる。  
だから、この非同期処理が、メインと同じスレッドを使っているとしたら、メイン側のスレッドも停止します。  

``` C#
// Thread.Sleepの場合 | Task.Delayの場合
// step 1             | step 1
// getMessage!        | getMessage!
// finished!          | step 3
// message:hello      | finished!
// step 2             | message:hello
// step 3             | step 2
{
    executeAsync();
    Console.WriteLine("step 3");
    Console.ReadLine();
}

static async Task<string> getMessageAsync(string message)
{
    Console.WriteLine("getMessage!");
    //System.Threading.Thread.Sleep(3000);
    await Task.Delay(3000);
    Console.WriteLine("finished!");
    return "message:" + message;
}
static async void executeAsync()
{
    Console.WriteLine("step 1");
    var result = await getMessageAsync("hello");
    Console.WriteLine(result);
    Console.WriteLine("step 2");
}
```

[C# で Thread.Sleep はあきまへん](https://qiita.com/TsuyoshiUshio@github/items/e9404651c9e48f1b8443)  

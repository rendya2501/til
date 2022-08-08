# Async・Await

---

## await

非同期処理しているタスクの完了を待つ場合はawaitを使います。  
指定した Task の完了を待ち、その結果を取りだす。  

※awaitはTask型に対して使用する。  

``` C# : 基本
await RunTaskBAsync(); // タスクBに戻り値がない(void)場合

var returnValue = await RunTaskBAsync(); // タスクBに戻り値がある場合
```

``` C# : タスクBが正常終了した後、その戻り値を使って続きの処理をする場合
var result = await RunTaskBAsync();
Console.WriteLine(result); // 続きの処理
```

``` C# : タスクBが異常終了、またはキャンセルされた場合のエラーハンドリング(try/catch)
try
{
  await RunTaskBAsync();
}
catch (TaskCanceledException ex)
{
  // キャンセルされた場合の例外処理
}
catch (Exception ex)
{
  // 異常終了した場合の例外処理
}
```

``` C# : 必ずしもメソッドの実行時にawaitを使う必要はなく、待機のタイミングを遅らせることもできます。
Task<int> taskB = RunTaskBAsync(); // awaitをつけていないのでTask<int>型を受け取る

// 何か別の処理 (この間もタスクBは非同期で実行されている)

int result = await taskB; // ここでタスクBの完了を待つ
```

---

## async

awaitを使う場合、下記ルールに則って実装します。  

1. メソッドにasyncキーワードを付与 (文法)  
   - `非同期メソッド` となる  
2. 戻り型としてTask型を利用 (文法)  
   - returnする場合: Task<戻り値の型>  
   - returnしない場合: Task  
3. メソッド名の語尾にAsyncをつける (推奨)  
   - 呼び出す側が非同期処理であることを把握しやすくするため  

``` C#
public async Task<int> RunTaskAAsync()
{
  int result = await RunTaskBAsync();
  return result + 1; // taskBの戻り値を使った計算例
}
```

上記サンプルコードでのポイントは「戻り型はTask\<int>型なのに、実際にreturnしているのはint型である」という点です。  
実は asyncキーワードをつけると、戻り値をTask型で自動的にラップして返してくれるようになります。  
これで呼び出し元はTask型のオブジェクトを使って非同期処理のハンドリングができますし、自分でTask型を生成しなくてよい。  

以上のポイントを踏まえ、 タスクA -> タスクB -> タスクC の非同期呼び出しを実装すると下記になります。  

``` C#
public async Task<int> RunTaskAAsync()
{
    var result = await RunTaskBAsync(); // taskBの完了を待つ
    return result + 1;
}
private async Task<int> RunTaskBAsync()
{
    await RunTaskCAsync(); // taskCの完了を待つ
    return 1 + 2 + 3;
}
private async Task RunTaskCAsync()
{
    await Task.Delay(500); // 0.5秒待機
}
```

---

## AsyncをAwaitするのと同期は同じか？

ibさんからAsyncをAwaitするのと、普通の同期処理は同じか？という質問を受けた。  
厳密には違うだろうが、どう違うのか、考えてみればわからないのでまとめることにした。  

同期処理における時間のかかる処理は白くなって動かせなくなる。  
しかし、AsyncAwaitは普通に動かせる。  
同期処理における待機は、UIスレッドそのものを止めるので、画面が固まってしまうが、AsyncAwaitはUIスレッド以外で新しくスレッド作ってそちらで実行するので、画面は固まらない。  
でも、処理を実行しているスレッドでは、実質同期的な停止と同じなのだろう。  

``` C#
    // 画面固まる。2秒経過したら画面動かせるようになって、メッセージボックスが表示される。
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Task.Delay(2000).GetAwaiter().GetResult();
        MessageBox.Show("owata1");
    }

    // 画面固まらない。2秒中も操作可能で2秒経過したらメッセージボックスが表示される。
    private async void Button_Click_1(object sender, RoutedEventArgs e)
    {
        await Task.Delay(2000);
        MessageBox.Show("owata2");
    }
```

async await 同期 同じ c#  
[[C#]await利用時の同期コンテキストと実行スレッドの動きについてコードを動かして見ていく](https://qiita.com/Kosei-Yoshida/items/7afe6c2f6158f36f50b1)  
[【C#】ASYNC/AWAITで同期メソッドから非同期メソッドを呼ぶ方法](https://nryblog.work/call-sync-to-async-method/)  

何回もボタンを押した後で、〇〇のスレッドが終了しましたってのはそういう意味なのかもしれない？
そのスレッドの処理が終わったことを通知するアナウンスなんだろうな。

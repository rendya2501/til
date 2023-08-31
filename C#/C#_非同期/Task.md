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

## Taskのキャンセルで例外を発生させない方法はないのか？

頑張ればありそうだけど、基本的にトークンを使った方法が一番良い模様。  

c# task cancel no exception  

[Why Do You Need a Cancellation Token in C# for Tasks?](https://hackernoon.com/why-do-you-need-a-cancellation-token-in-c-for-tasks)  

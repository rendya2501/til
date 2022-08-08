# 非同期系メモ

[【C#】Task初心者のTask初心者によるTask初心者の為のTask入門](https://qiita.com/OXamarin/items/eddc9f7f01b691631887)  
[Taskを極めろ！async/await完全攻略](https://qiita.com/acple@github/items/8f63aacb13de9954c5da)  

TaskはUIスレッドを止めないための仕組み。  
メインスレッド一本で処理するのが同期処理。  
メインスレッド以外で処理するのが非同期処理。  
メインスレッドには、必然的にUIスレッドが来るので、同期処理で重い処理を走らせたら画面が固まるのはこのため。  

---

## 同時に複数の非同期処理を扱う

例えば、タスクBとタスクCを同時に処理して、両方が完了した後にタスクAの続きを処理したい場合があります。  
そういった場合はTask.WhenAllを使用します。  
Task.WhenAllは、「複数のタスクが全て完了したかどうかを確認できるTask」を返します。  
Task型なのでawaitできます。  

``` C#
public async Task<int> RunTaskAAsync()
{
  Task<int> taskB = RunTaskBAsync();
  Task<int> taskC = RunTaskCAsync();
  
  int[] results = await Task.WhenAll(taskB, taskC); // 両方完了後、戻り値が配列として返る
  
  return results[0] + results[1]; // タスクBとタスクCの結果を加算
}

private async Task<int> RunTaskBAsync()
{
  await Task.Delay(100); // 0.1秒待機
  return 1 + 2 + 3;
}

private async Task<int> RunTaskCAsync()
{
  await Task.Delay(200); // 0.2秒待機
  return 4 + 5 + 6;
}
```

---

## 既存の同期処理を非同期化する

既存の同期メソッドを、呼び出し側から非同期扱いすることができます。  
Task.Runを使うと処理をTaskでラップすることができます。  

``` C#
public async Task<int> RunTaskAAsync()
{
  var result = await Task.Run(RunTaskB); // Task<int>化されるのでawaitできる
  return result + 1;
}

private int RunTaskB() // 同期メソッド
{
  return 1 + 2 + 3;
}
```

## 戻り値があるTaskの並列実行と結果の扱い方

``` C#
    Task<int> t1 = Task.Run(() => 1);
    Task<int> t2 = Task.Run(() => 2);
    Task<(int, int)> t3 = Task.Run(() => (3, 3));

    Task.WaitAll(t1, t2, t3);

    int result1 = t1.Result;
    int result2 = t2.Result;
    (int, int) result3 = t3.Result;

    // Actionを返すTaskの定義方法
    Task<Action> t1 = new Task<Action>(() =>
    {
        return () => { _ = 1; };
    });
```

---

## 非同期処理の完了を待たない

非同期処理は必ずしも待つ必要はありません。投げっぱなしにしたいケースがあります。  

そういった場合は単純に待たなければ良い、つまりawaitを使わなければ良いです。  

``` C#
public int RunTaskA()
{
    RunTaskBAsync(); // awaitを使わない

    var result = 1 + 2 + 3; // タスクBの完了を待たずに処理を続ける
    return result; // タスクBの完了を待たずにreturnする
}
```

この場合のポイントは下記になります。  

- タスクBの完了を待たずに処理を続けることができる  
- タスクBが失敗したとしてもタスクAには影響はない  

ちなみにこのように実装するとVisual Studio審判長から以下のようなイエローカードが提示されます。  

>この呼び出しを待たないため、現在のメソッドの実行は、呼び出しが完了するまで続行します。呼び出しの結果に 'await' 演算子を適用することを検討してください。  

わかっていて敢えてawaitをつけていないことを審判に示すためには下記のように実装します。  

``` C#
var _ = RunTaskBAsync(); // taskを利用しないことを明示
```

---

## 非同期メソッドを同期メソッドから呼び出す

非同期処理をする場合、基本的にはasync/awaitパターンが推奨されるのですが、既存実装の改修などでは戻り値をTask型に変更できないケースも多いです。  

そういった場合はこれらを利用します。  

- Task型: task.Waitメソッド  
- Task\<T>型: task.Resultプロパティ  

``` C#
public int RunTaskA()
{
    int result = RunTaskBAsync().Result; // 完了の待機と戻り値の取得
    return result + 1;
}
```

!. task.Wait task.Resultを使うとデッドロックが発生する可能性があります。どうしてもという場合のみ気をつけて使いましょう。  
参考: [async/awaitについての備忘録 - async/await, Taskのタブー②](https://qiita.com/mounntainn/items/3f39e0c57412c48508bf#asyncawait-task%E3%81%AE%E3%82%BF%E3%83%96%E3%83%BC-1)  

---

## asyncを使う必要がないケース

非同期メソッドを実装するときには必ずしもasyncを使う必要はありません。  
asyncキーワードは自動的にTaskを生成してくれますが、逆に言えば自力でTaskオブジェクトをreturnすることができればasyncに頼る必要はありません。  

そのようなケースは主に2つあります。  

- 他の非同期メソッドの戻り値をそのままreturnできる場合  
- タスクAがタスクBを呼び出し、その結果をそのままreturnする場合  

``` C#
public async Task<int> RunTaskAAsync()
{
    return await RunTaskBAsync(); // int型をreturnしてasyncにラップしてもらう
}

private async Task<int> RunTaskBAsync()
{
    await Task.Delay(1000); // 1秒待機
    return 1 + 2 + 3;
}
```

この場合、async/awaitを使わずにこのように記載することもできます。  

``` C#
public Task<int> RunTaskAAsync()
{
    return RunTaskBAsync(); // Task<int>型をreturnする
}

private async Task<int> RunTaskBAsync()
{
    await Task.Delay(1000); // 1秒待機
    return 1 + 2 + 3;
}
```

後者のように書くメリットは、前者だとawaitでTaskを剥がした後にasyncでまたTaskにラップされるので、その余計なステップを排除できることでしょうか(そこまで気にしなくていい気もするけど)。  

---

## 自力でTaskを生成する場合

Taskを自力で生成する場合の代表例は、既存同期メソッドの非同期化です。  

``` C#
public Task<int> RunTaskAAsync()
{
  Task<int> taskB = Task.Run(RunTaskB);
  return taskB;
}

private int RunTaskB() // 同期メソッド
{
  return 1 + 2 + 3;
}
```

他には、「戻り型はTask型だが処理は非同期である必要がない」という場合があります。  
具体的には下記のような場合です。  

- 元々は非同期メソッドだったものが改修により非同期である必要がなくなった  
- interfaceの戻り型の定義がTaskになっているが、それを実装した際に非同期処理がなかった  

こういった場合にはTask.CompletedTaskでTask型のオブジェクト、Task.FromResultでTask\<T>型のオブジェクトを生成できます。  

``` C#
public Task RunTask1Async()
{
    // 同期処理
        
    return Task.CompletedTask; // "正常終了"を表すTaskオブジェクト
}

public Task<int> RunTask2Async()
{
    // 同期的な計算処理
    var result = 1 + 2 + 3;
    return Task.FromResult(result); // 値をTaskでラップして返す
}
```

---

## 非同期処理のキャンセル

あるタスクを非同期実行した後、そのタスクをキャンセルしたい場合があります。  
例えば「サーバーに通信したはいいものの応答が遅すぎるので通信を切りたい」など。  

そういった場合は**CancellationToken**を使います。  
CancellationTokenは呼び出し元から非同期メソッドにキャンセル依頼をするためのものです。  
これは事前に呼び出し元で生成して非同期メソッドに渡しておく必要があります。  

``` C#
var cts = new CancellationTokenSource();
var taskB = RunTaskBAsync(cts.Token); // CancellationTokenを渡す

// なんらかの処理

cts.Cancel(); // タスクBにキャンセルを依頼
```

ただし、このキャンセルの仕組みを利用するためには当然ですが非同期メソッド側が下記のようにキャンセルに対応していないといけません。  
自作する場合には気をつけましょう。  

- 引数でCancellationTokenを受け取れるようになっている  
- CancellationTokenを監視して、キャンセル依頼が来たときに反応できるようになっている  
- キャンセル処理をしてTaskCanceledExceptionをthrowできるようになっている  

---

## async void

async void ってなんだ？  
→  
イベントのためにしょうがなくある構文。  
基本的に非同期処理は投げっぱなしになる模様。  

[asyncの落とし穴Part3, async voidを避けるべき100億の理由](https://neue.cc/2013/10/10_429.html)  

>自分で書く場合は、必ずasync Taskで書くべき、というのは非同期のベストプラクティスで散々言われていることなのですけれど、理由としては、まず、**voidだと、終了を待てないから**。  
>voidだと、その中の処理が軽かろうと重かろうと、終了を感知できない。  
>例外が発生しても分からない。投げっぱなし。  
>これがTaskになっていれば、awaitで終了待ちできる。例外を受け取ることができる。  
>await Task.WhenAllで複数同時に走らせたのを待つことができる。  
>はい、async Taskで書かない理由のほうがない。  
>んじゃあ何でasync voidが存在するかというと、イベントがvoidだから。  
>はい。button_clickとか非同期対応させるにはvoidしかない。それだけです。  
>なので、自分で書く時は必ずasync Task。async voidにするのはイベントだけ。これ絶対。  

[C#_戻り値が void と Task の違い](https://koshinran.hateblo.jp/entry/2018/05/14/101108)  

>戻り値が void だと await がついた処理がいつ完了するかわからない。  

■await  
Task 完了まで待つ、という意味なので同期的。  

await しない場合、「誰かこの仕事実行して」と命令を投げるだけなのでタスク実行中に自分は本来の仕事の続きをこなせる。  
戻り値 Task とすることで、その仕事の進捗状況を把握できる。  

■戻り値 Task  
「手順書に書かれた仕事が全て完了したことを報告する」ことになる。  
非同期メソッドで戻り値を Task にした場合、自動的にメソッドが return した時に完了する Task となる。  

■戻り値 void  
「この Task を開始して」という命令をした後は、その命令したことを忘れる。  
実害は、 void で実行した非同期メソッドの中で例外が発生した場合など、その例外はコード上で見つけることが出来なくなり、アプリケーションを殺す。  

■非同期メソッドで戻り値 Task  

``` C#
    private async Task Hoge()    // 順次動作
    {
        await Task.Run(() => 処理A);
        //処理 A を実行する、というタスクを開始して完了するまで待機。
    }
```

↑ return は？  
非同期メソッドで戻り値が Task の時、そのメソッドが return した時に完了する Task となる。  
戻り値 Task と指定しても、返す必要がない ( 返せないもよう )。  
Task\<T> とすると、 return で T 型が返る。  

■非同期メソッドで戻り値 void (基本 NG )  

``` C#
    private async void Hoge()    // 順次動作
    {
        await Task.Run(() => 処理A);
        //処理 A がいつ完了するか不明。完了するまで待機。
    }
```

■同期メソッドで戻り値 void  

``` C#
    private void Hoge()    // 並列動作
    {
        Task.Run(() => 処理A);
        // Task.Run が投げっぱ。処理 A の状態が分からない。
        // 命令を投げるだけなので、 処理 A 実行中に自分自身は本来の仕事の続きができる。
        // 戻り値が Task だと進捗状況を把握できる。
    }
```

■同期メソッドで戻り値 Task  

``` C#
    private Task Hoge()    // 並列動作
    {
        var cs = new List<Fuga>;
        for()
        {
            var c = Task.Run(() => 処理A);
            cs.Add(c);    //処理 A を開始するというタスクを List にまとめる。
        }
        return Task.WhenAll(c);
        //全ての Task が完了した時、完了扱いになる Task を生成。
    }
```

**async void は禁止**  
UI イベントハンドラ―に非同期メソッドを登録するのは void の必要があった。  
UI から直接実行されるイベントの受け皿メソッドのみ、async void を使える。  
それ以外は NG。  

※  
特定のスレッドに依存する処理 ( UI 操作など ) は、  
本来非同期メソッド上で取り扱ってはいけない。( Task の思想ではそうなるはずだった )  

同期メソッドの戻り値 void は、非同期メソッドでは戻り値 Task のこと。  
同期メソッドの戻り値 T は、非同期メソッドでは戻り値 Task\<T> の T のこと。  

## 投げっぱなしの非同期メソッドを使いたい場合、どうすればいいんでしょう？

async Task メソッドをそのまま実行する。  

``` C#
public async Task<ActionResult> Index()
{
    // 警告抑制のため破棄で受ける
    _ = ToaruAsyncMethod();
    return View();
}

public async Task ToaruAsyncMethod()
{
    await Task.Delay(TimeSpan.FromSeconds(1));
    Debug.WriteLine("hoge");
}
```

---

## async void と _ = Task.Run は同じか？

[c# async voidでawait Task.Run()失敗の巻](https://qiita.com/twentyfourhours/items/3451f39567239f951a1a)  

仕事投げっぱなしで終了を検知しないという意味では同じ。  
async void は終了を検知できないけど、 Task.Runは検知できる。  
だけど、破棄することであえて検知しない→投げっぱなしにすることも可能で、その状態はasync void を実行したのと同じような状態とみなせる。  

``` C#
        // Start 0
        // Start 1
        // Start 2
        // End 1
        // End 0
        // End 2
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                await Task.Run(() => DelayMethodByAsyncVoid(i));
            }
        }
        async void DelayMethodByAsyncVoid(int id)
        {
            Console.WriteLine("Start " + id);
            await Task.Delay(2000);
            Console.WriteLine("End " + id);
        }

        // Start 0
        // Start 1
        // Start 2
        // End 2
        // End 1
        // End 0
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                _ = DelayMethodByAsyncTask(i);
            }
        }

        // こいつだけは明らかに動作が違うのでやるな
        // Start 3
        // Start 3
        // Start 3
        // End 3
        // End 3
        // End 3
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                _ = Task.Run(() => DelayMethodByAsyncTask(i));
            }
        }

        async Task DelayMethodByAsyncTask(int id)
        {
            Console.WriteLine("Start " + id);
            await Task.Delay(2000);
            Console.WriteLine("End " + id);
        }
```

---

## 非同期プログラミングのベストプラクティス

[非同期プログラミングのベストプラクティス](https://docs.microsoft.com/ja-jp/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)  

- async void を避ける  
- すべて非同期にする  
- コンテキストを構成する  

---

## Taskのラムダ式の定義の仕方

async/await を使った非同期ラムダ式を変数に代入する方法とも言うか。  
いつもの癖でAction型に入れようとして少し苦戦したので備忘録として残すことにした。  

TaskはTask型を絶対に返すので、Actionは使うことができない。  
最低でも`Func<Task>`が必要。  
戻り値もあるなら`Func<戻り値、Task>`が必要。  

``` C# : 単純なTaskのデリゲート定義
    Func<Task> AsyncFunc = async () =>
    {
        await Task.Delay(1);
    };
```

```C# : 単純なTaskのローカル関数定義
    async Task AsyncFunc = async () => 
    {
        await Task.Delay(1);
    }
```

``` C# : 戻り値があるTask型のデリゲート定義
    Func<ListReplyContext, Task> callback = async res =>
    {
        return new ListReplyContext();
    }
```

``` C# : 戻り値があるTask型のローカル関数定義
    async Task<ListReplyContext> callback ()
    {
        return new ListReplyContext();
    }
```

<https://qiita.com/go_astrayer/items/352c34b8db72cf2f6ca5>  
[async/await を使った非同期ラムダ式を変数に代入する方法](https://qiita.com/go_astrayer/items/352c34b8db72cf2f6ca5)  

---

## async voidのラムダ式

<https://stackoverflow.com/questions/61827597/async-void-lambda-expressions>  

async void ○○ await △△ みたいな非同期処理を1行で書けないか探したが、全然そんなこと書いてるところがない。  
「async void lambda c#」で調べてようやくそれっぽいところにたどり着いたが、本当にあってるのかはわからない。  

2022/05/19 追記  
async voidはTask.Run()を投げっぱなしにすることで実現できる。  

async voidは仕事の完了を観測できない。  
つまり処理の投げっぱなしを意味する。  
でもって、Taskは絶対にTask型を返すので、ラムダ式として定義するなら最低でも`Func<Task>`と定義する必要がある。  
なので、厳密にasync voidはラムダ式で定義できず、Task.Run()で特にawaitも.Resultもしなければasync voidと同じ状況を再現できる。  

だからいくら探しても思うような記事にめぐり合わなかった。  
そもそもTaskに対する理解が足りていなかった証拠だろう。  

``` C#
    private async void Hoge()
    {
        await Task.Delay(1000);
    }
    private void Main()
    {
        // async void HogeはTask.Runのように書いても動くけど、厳密には少し違うみたい。
        Hoge();
        // これは正確にはWait1000みたいな意味合いらしい。
        Task.Run(async () => await Task.Delay(1000));
    }
    private async Task Wait1000() {
        await Task.Delay(1000);
    }
    Task.Run(Wait1000);
```

---

## 非同期メソッドの並列実行

<https://docs.microsoft.com/ja-jp/dotnet/csharp/programming-guide/concepts/async/>  

非同期メソッドを並列処理できそうな箇所があったので、Task.WaitAllみたいな並列処理をAsyncAwaitでも実現できないか調べてみた。  
天下のマイクロソフトに完璧なサンプルがあったのでそれを拝借する。  
因みに実現できたのだが、そうしたら非同期中のクルクルがなくなってしまったのでやめた。  

``` C#
static async Task Sample1()
{
    var eggsTask = Task.Run(async () =>
    {
        Console.WriteLine("すごく重い処理その1(´・ω・`)始まり");
        await Task.Delay(3000);
        Console.WriteLine("すごく重い処理その1(´・ω・`)終わり");
    });
    var baconTask = Task.Run(async () =>
    {
        Console.WriteLine("すごく重い処理その2(´・ω・`)始まり");
        await Task.Delay(5000);
        Console.WriteLine("すごく重い処理その2(´・ω・`)終わり");
    });
    // 普通にTask.WhenAllに各非同期メソッドを登録すればいいみたい。
    await Task.WhenAll(eggsTask, baconTask);
    Console.WriteLine("終わり");

    // Listに入れて引き回す事もできる。
    // 処理を並列に実行しつつ、各処理固有の終了処理を記述したい時はこっちかもね。
    var breakfastTasks = new List<Task> { eggsTask, baconTask };
    while (breakfastTasks.Count > 0)
    {
        Task finishedTask = await Task.WhenAny(breakfastTasks);
        if (finishedTask == eggsTask)
        {
            Console.WriteLine("eggs are ready");
        }
        else if (finishedTask == baconTask)
        {
            Console.WriteLine("bacon is ready");
        }
        breakfastTasks.Remove(finishedTask);
    }
    Console.WriteLine("終わり");
}
```

---

## なぜThread.Sleepはだめなのか

[C# で Thread.Sleep はあきまへん](https://qiita.com/TsuyoshiUshio@github/items/e9404651c9e48f1b8443)  

理由は簡単で、Thread.Sleep Method は、スレッドを止めるメソッドだから。  
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

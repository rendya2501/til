# Async Void

---

## async void とは？

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

<!--  -->
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
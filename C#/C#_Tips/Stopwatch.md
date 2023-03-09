# StopWatch

## 基本

`Stopwatch.StartNew`はインスタンスの生成とスタートを同時に行ってくれるもの。  
これがあればこんな風に書ける。  

``` cs
// 処理1
var stopWatch = Stopwatch.StartNew();
// <処理1の内容をここに記述>
stopWatch.Stop();
Console.WriteLine(stopWatch.Elapsed);

// 処理2
stopWatch = Stopwatch.StartNew();
// <処理2の内容をここに記述>
stopWatch.Stop();
Console.WriteLine(stopWatch.Elapsed);
```

[処理時間を正確に計測するには？［2.0のみ、C#、VB］ - ＠IT](https://atmarkit.itmedia.co.jp/fdotnet/dotnettips/412stopwatch/stopwatch.html)  

``` cs
Stopwatch sw = new Stopwatch();
sw.Start();
System.Threading.Thread.Sleep(1000);
sw.Stop();

// ミリ秒単位で出力
Console.WriteLine(sw.ElapsedMilliseconds); // 出力例： 998

// TimeSpan構造体で書式付き表示
Console.WriteLine(sw.Elapsed); // 出力例： 00:00:00.9984668

// 高分解能なタイマが利用可能か
Console.WriteLine(Stopwatch.IsHighResolution); // 出力例： True

// タイマ刻み回数
Console.WriteLine(sw.ElapsedTicks); // 出力例： 2988141812

// タイマの1秒あたりの刻み回数
Console.WriteLine(Stopwatch.Frequency); // 出力例： 2992730000

// より詳細な秒数
double sec = (double)sw.ElapsedTicks / (double)Stopwatch.Frequency;
Console.WriteLine(sec); // 出力例：0.998466888760429
```

---

## 備忘録

`StartNew`を知らなかった頃は、愚直に`Stopwatch`をnewしてResetして再利用していたわけだが、今となってはいい思い出なので備忘録として残す。  

``` C#
static void Benchmark()
{
    var stopWatch = new System.Diagnostics.Stopwatch();

    // 処理1
    stopWatch.Start();

    stopWatch.Stop();
    stopWatch.Reset();


    // 処理2
    stopWatch.Start();

    stopWatch.Stop();
    stopWatch.Reset();
}
```

---

## ベンチマークアノテーション

ベンチマークアノテーションを付けたメソッドの時間を計測してくれるものらしいが、使うまでがややこしそうなので参考程度に。  

[BenchmarkDotNetを使ってみる｡](https://qiita.com/Tokeiya/items/30d8a76163622a4b5be1)  

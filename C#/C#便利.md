# 便利コードまとめ

---

## 処理速度計測

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

## イミディエイト クリップボード コピー

`Clipboard.SetText(コピーしたい文字列)`  

[クリップボードに文字列をコピーする、クリップボードから文字列を取得する](https://dobon.net/vb/dotnet/string/clipboard.html)  
Clipboard.SetTextメソッドを使えば、簡単に文字列をクリップボードにコピーできます。  

VB6で長いSQLを取得する時にお世話になったでそ。  
最新の環境でも普通に使えたのでメモ。  
これでStringBuilderをToStringしたときに、長ったらしく横に展開されるあれも楽に取ることができる。  
内容的にVisualStudio側に置いてもいいのだが、とりあえずC#のほうに置いておく。  

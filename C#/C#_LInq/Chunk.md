# Chunk

## 連番を指定した数の塊に分解する方法

間隔3と指定した場合、
{
    {0,1,2},
    {3,4,5},
    {6,7,8}
}

間隔5を指定した場合、
{
    {0,1,2,3,4},
    {5,6,7,8,9},
    {10,11,12,13,14},
}

年齢を範囲で区切る処理で必要になったのでまとめ。  
みたいな一連のリストを生成するスマートな処理はないものか探した。

最後に考えたのはForで愚直にやる方法。  
スマートじゃないし、直観的でもないけど、十分動く。でもって速度的には一番早い。  

``` C#
var kankaku = 5;
var from = 0;
// 間隔分インクリメントされる。
for (var to = kankaku - 1; to < 150; to += kankaku)
{
    // ここの間は0,1,2,3,4  5,6,7,8,9 になってる。

    // fromは6,10,16みたいな感じで常にto + 1の値にする。
    from = to + 1;
}
```

探したら使えそうなのがたくさんあった。  

[【C#】LINQ でコレクションをN個ずつの要素に分割する](https://qiita.com/Nossa/items/db9bff2390291432d138)  
[連続する数値でグループ分けする](https://noriok.hatenadiary.jp/entry/2015/06/14/122043)  
[LINQでn個ずつグルーピング](https://ichiroku11.hatenablog.jp/entry/2015/04/16/230309)  
[C#,VB LINQでコレクションをチャンク(N個ずつ)に分割](https://webbibouroku.com/Blog/Article/chunk-linq)  

1から100までの連番を生成して、それを指定した要素数の塊にわけて、ループして最小値と最大値を取れば実現できるという寸法。  

``` C#
    // N 個ずつの N
    var chunkSize = 5;
    var chunks = Enumerable.Range(0, 100)
        .Select((v, i) => (v, i))
        .GroupBy(x => x.i / chunkSize)
        .Select(g => g.Select(x => x.v));
    // 動作確認
    foreach (var chunk in chunks)
    {
        foreach (var item in chunk)
            Console.Write($"{item} ");
        Console.WriteLine();
    }

    // fromtoとnullのバージョン
    List<(int, int)?> chunks2 = Enumerable.Range(0, 100)
        .Select((v, i) => (v, i))
        .GroupBy(x => x.i / 5)
        .Select(g => ((int, int)?)(g.Min(m => m.v), g.Max(m => m.v)))
        .ToList();
    chunks2.Insert(0, null);
```

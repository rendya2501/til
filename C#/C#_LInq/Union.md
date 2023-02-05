# Union

---

## Concatとの違い

- Unionは重複を削除する。  
- Concatは重複を許可する。  

[【C#】LINQの集合演算まとめ｜プログラミング暮らし](https://pg-life.net/csharp/linq-setoperation/#:~:text=%E3%81%A6%E8%BF%94%E3%81%97%E3%81%BE%E3%81%99%E3%80%82-,Union%20%E3%81%A8%20Concat%20%E3%81%AE%E9%81%95%E3%81%84,%E9%87%8D%E8%A4%87%E3%82%92%E8%A8%B1%E5%8F%AF%E3%81%97%E3%81%BE%E3%81%99%E3%80%82)  

---

## ユースケース

AリストとBリストの番号を統合して重複を排除した合計が1より大きいか？みたいな判定する時に使ったのでメモ。  

``` C#
if (TestList1.Select(s => s.TestNo).Union(TestList2.Select(s => s.TestNo)).Count(w => !string.IsNullOrEmpty(w)) > 1)
```

[要素が重複しないようにして、複数の配列（またはコレクション）をマージする（和集合を取得する）](https://dobon.net/vb/dotnet/programing/arrayunion.html)  

---

## AddとUnion

LinqのAddはvoidなので、チェーンして書くことができない。  
だけど、Unionを工夫して使うと全部繋げて書けるよっていう例  
速度は保証できないので、完全に好みであるが、備忘録として残しておく  

■ **愚直に実装したパターン**

途中でAddさせたいならいったんチェーンを切らないといけないし、ToList化もしないといけない。  

``` C#
var framePlayerList = TestModel
    .GetList(condition)
    .Where(w => w.TestNo != targetPlayer.TestNo)
    .ToList();
// チェックインするプレーヤーを追加する
framePlayerList.Add(targetPlayer);
// 今回のチェックインでその枠に存在するプレーヤー全員がチェックイン済みになるなら組確定とする。
reservationFrame.ConfirmFlag = framePlayerList.All(a => a.CheckinFlag == true);
```

■ **UNIONで疑似的にADDしたパターン**  

繋げるべきデータをUnion内で作ってしまえば、データの追加が可能  

``` cs
reservationFrame.ConfirmFlag = TestModel
    .GetList(condition)
    .Where(w => w.TestNo != targetPlayer.TestNo)
    // そのプレーヤーNoを除外した後、Listを作成してUNIONすることで疑似的なADDが可能
    .Union(new List<TestClass>() { targetPlayer })
    .All(a => a.CheckinFlag == true);
```

■ **ベンチマーク**

1000万回ループさせた場合、とEnumerable.Rangeで1000の空リスト作ってUnionした場合、30万:100万なので3倍ほど遅いと見なせる。  

``` C#
static void Benchmark()
{
    var stopWatch = new System.Diagnostics.Stopwatch();
    var aa = new List<int>();

    // 処理1 約20~30万tick
    stopWatch.Start();
    for (int i = 0; i < 10000000; i++)
    {
        aa.Add(i);
    }
    stopWatch.Stop();
    stopWatch.Reset();

    aa.Clear();

    // 処理2 約100万tick
    stopWatch.Start();
    aa = aa.Union(Enumerable.Range(0, 10000000)).ToList();
    stopWatch.Stop();
    stopWatch.Reset();
}
```

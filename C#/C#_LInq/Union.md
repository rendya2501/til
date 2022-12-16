
# Union

---

## ユースケース

AリストとBリストの番号を統合して重複を排除した合計が1より大きいか？みたいな判定する時に使ったのでメモ。  

``` C#
if (TestList1.Select(s => s.TestNo).Union(TestList2.Select(s => s.TestNo)).Distinct().Count(w => !string.IsNullOrEmpty(w)) > 1)
```

[要素が重複しないようにして、複数の配列（またはコレクション）をマージする（和集合を取得する）](https://dobon.net/vb/dotnet/programing/arrayunion.html)  

---

## AddとUnion

LinqのAddはvoidなので、チェーンして書くことができない。  
だけど、Unionを工夫して使うと全部繋げて書けるよっていう例  
速度は保証できないので、完全に好みであるが、備忘録として残しておく  

``` C#
// 本来のパターン
// 途中でAddさせたいならいったんチェーンを切らないといけないし、ToList化もしないといけない。
var framePlayerList = TestModel
    .GetList(condition)
    .Where(w => w.TestNo != targetPlayer.TestNo)
    .ToList();
// チェックインするプレーヤーを追加する
framePlayerList.Add(targetPlayer);
// 今回のチェックインでその枠に存在するプレーヤー全員がチェックイン済みになるなら組確定とする。
reservationFrame.ConfirmFlag = framePlayerList.All(a => a.CheckinFlag == true);


// UNIONで疑似的にADDしたパターン
// 繋げるべきデータをUnion内で作ってしまえば、データの追加が可能というわけ
reservationFrame.ConfirmFlag = TestModel
    .GetList(condition)
    .Where(w => w.TestNo != targetPlayer.TestNo)
    // そのプレーヤーNoを除外した後、Listを作成してUNIONすることで疑似的なADDが可能というわけ
    .Union(new List<TestClass>() { targetPlayer })
    .All(a => a.CheckinFlag == true);


// ごめん。このサンプルだったらこれで済んだわ。
// 比較したい要素はフラグだけだから、そのプレーヤーが既に存在するなら、
// そのフラグだけ最新のプレーヤー情報に書き換えればいいだけだった。
var framePlayerList = TestModel
    .GetList(condition)
    .All(a => {
        if (a.TestNo == targetPlayer.TestNo) {
            a.CheckinFlag = targetPlayer.CheckinFlag;
        }
        return a.CheckinFlag == true;
    });

var tupleList = new List<(int Index, bool flag)>
    {
        (1, false),
        (2, true),
        (3, true),
    }
    .All(a => {
        if (a.Index == 1)
        {
            a.flag=true;
        }
        return a.flag == true;
    });
```

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

# Enumerable.Range

## 固定レコード出力

``` cs
var tupleList = new List<(int ClsCode, string ClsName, int Score, string StudentCode, string StudentName)>
{
    (1, "国語", 90,"0010","田中 一郎"),
    (2, "数学", 80,"0010","田中 一郎"),
    (3, "英語", 70,"0010","田中 一郎"),
    (1, "国語", 60,"0011","鈴木 二郎"),
    (2, "数学", 50,"0011","鈴木 二郎"),
    (3, "英語", 80,"0011","鈴木 二郎"),
    (1, "国語", 70,"0012","佐藤 三郎"),
    (2, "数学", 80,"0012","佐藤 三郎"),
    (3, "英語", 90,"0012","佐藤 三郎"),
};
```

上記のようなデータがあるとする。  
これをGroupByして合計を出力したい。  
帳票に7行固定で出力するため、要素数は7固定。  
存在しないレコードにはnullが入ってほしい。  

目標とする出力値は以下の通り。  

``` txt
[0]: { ClsCode = 1, ClsName = "国語", TotalScore = 220 }
[1]: { ClsCode = 2, ClsName = "数学", TotalScore = 210 }
[2]: { ClsCode = 3, ClsName = "英語", TotalScore = 240 }
[3]: null
[4]: null
[5]: null
[6]: null
```

目標値を出力するために愚直にやるならこうなる。  
しかし、1文で完結できていない。  
なんとか全てチェーンさせたい。  

``` cs
// groupbyで集約する
var grouped = tupleList.GroupBy(g => new { g.ClsCode, g.ClsName });

// Range(1,7)で7行固定でループ。
// Select内部でWhereして合算。
var result = Enumerable
    .Range(1, 7)
    .Select(i =>
    {
        var item = grouped.FirstOrDefault(f => f.Key.ClsCode == i);
        return new
        {
            item?.Key.ClsCode,
            item?.Key.ClsName,
            TotalScore = item?.Sum(s => s.Score) ?? 0,
        };
    })
    .ToList();
```

解決法として、キーコード 1~7に対してLEFT OUTER JOINする事でいけた。  
しかし、nullのレコードが消えてしまったりしたので成功した例、失敗した例を全て載せる。  

■**成功した例**  

理想形1

``` cs
var gg = Enumerable
    .Range(1, 7)
    .GroupJoin(
        // グルーピングする
        tupleList.GroupBy(g => new { g.ClsCode, g.ClsName }),
        i => i,
        grouped => grouped?.Key.ClsCode,
        // グルーピングされている状態なので、コードは一意でその中に要素が入ってくる
        (i, grouped) => grouped.FirstOrDefault()
    )
    // firstOrDefaultすることで、コード1に対する要素群、コード2に対する要素群、・・・という形でループできる。
    .Select(s => new
    {
        s?.Key.ClsCode,
        s?.Key.ClsName,
        TotalScore = s?.Sum(s => s.Score) ?? 0,
    })
    .ToList();
```

理想形2  
最初にうまく行った例だが、selectを2回も書くなど、冗長過ぎるので使うことはないだろう。  
そもそも色々わかっていない時のモノなので仕方なし。  

``` cs
var dd = Enumerable
    .Range(1, 7)
    .GroupJoin(
        tupleList
            .GroupBy(g => new { g.ClsCode, g.ClsName })
            .Select(s => new { s.Key.ClsCode, s.Key.ClsName, TotalScore = s.Sum(sum => sum.Score) }),
        i => i,
        grouped => grouped.ClsCode,
        (i, grouped) =>
        {
            var item = grouped.FirstOrDefault();
            return new
            {
                item?.ClsCode,
                item?.ClsName,
                TotalScore = item?.TotalScore ?? 0,
            };
        }
    )
    .ToList();
```

■**失敗例**

GroupJoinすると階層が深くなってしまう。  
それを解決するためにSelectManyを使ってみたが、そうするとnullのレコードが消えてしまうのでうまく行かなかった。  
SelectManyした途端、nullのレコードがなくなる。  
それは後半でわかることだが、考えてみれば当たり前の事であった。  

``` cs
var fail_pattern1 = Enumerable
    .Range(1, 7)
    .GroupJoin(
        tupleList
            .GroupBy(g => new { g.ClsCode, g.ClsName })
            .Select(s => new { s.Key.ClsCode, s.Key.ClsName, TotalScore = s.Sum(sum => sum.Score) }),
        i => i,
        grouped => grouped.ClsCode,
        (i, grouped) => grouped
    )
    .SelectMany(s => s)
    .ToList();
// GroupJoinまでの結果
// [0] [IEnumerable]: Key = 1
//   [0]: { ClsCode = 1, ClsName = "国語", TotalScore = 220 }
// [1] [IEnumerable]: Key = 2
//   [0]: { ClsCode = 2, ClsName = "数学", TotalScore = 210 }
// [2] [IEnumerable]: Key = 3
//   [0]: { ClsCode = 3, ClsName = "英語", TotalScore = 240 }
// [3] [IEnumerable]: Count = 0
// [4] [IEnumerable]: Count = 0
// [5] [IEnumerable]: Count = 0
// [6] [IEnumerable]: Count = 0


var fail_pattern2 = Enumerable
    .Range(1, 7)
    .GroupJoin(
        tupleList.GroupBy(g => new { g.ClsCode, g.ClsName }),
        i => i,
        grouped => grouped.Key.ClsCode,
        (i, grouped) => grouped.Select(s => new { s.Key.ClsCode, s.Key.ClsName, TotalScore = s.Sum(sum => sum.Score) })
    )
    .SelectMany(s => s)
    .ToList();
// GroupJoinまでの結果
// [0] [List]: Count = 1
//   [0]: { ClsCode = 1, ClsName = "国語", TotalScore = 220 }
// [1] [List]: Count = 1
//   [0]: { ClsCode = 2, ClsName = "数学", TotalScore = 210 }
// [2] [List]: Count = 1
//   [0]: { ClsCode = 3, ClsName = "英語", TotalScore = 240 }
// [3] [List]: Count = 0
// [4] [List]: Count = 0
// [5] [List]: Count = 0
// [6] [List]: Count = 0


// fail_pattern1,2のGroupJoinの後.SelectMany(s => s)した場合の結果
// [0]: { ClsCode = 1, ClsName = "国語", TotalScore = 220 }
// [1]: { ClsCode = 2, ClsName = "数学", TotalScore = 210 }
// [2]: { ClsCode = 3, ClsName = "英語", TotalScore = 240 }
```

ネストも消えるがnullのレコードも消える。  
count=0は何もないので、そのまま階層をおろすと死ぬ。  
count=0をnullに変換するように記述すればいけると思うが、SelectManyのsはIEnumerableなのでselectとか色々しないと使えない。  
つまり、面倒くさいので、SelectMany案は使い物にならないということ。  

以下、実験用のコード  
ゴミ  

``` cs
var ee = Enumerable
    .Range(1, 7)
    .GroupJoin(
        tupleList
            .GroupBy(g => new { g.ClsCode, g.ClsName })
            .Select(s => new { s.Key.ClsCode, s.Key.ClsName, TotalScore = s.Sum(sum => sum.Score) }),
        i => i,
        grouped => grouped.ClsCode,
        (i, grouped) => grouped.Select(s =>
        new
        {
            s?.ClsCode,
            s?.ClsName,
            s?.TotalScore
        })
    )
    .ToList();
// [0] [IEnumerable]: Count = 1
// [1] [IEnumerable]: Count = 1
// [2] [IEnumerable]: Count = 1
// [3] [IEnumerable]: Count = 0
// [4] [IEnumerable]: Count = 0
// [5] [IEnumerable]: Count = 0
// [6] [IEnumerable]: Count = 0
```

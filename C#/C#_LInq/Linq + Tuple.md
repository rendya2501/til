# Linq + Tuple

---

## タプル初期化奮闘記

配列を宣言してFor文で回して値をいれるやり方をもっと洗練できないか試行錯誤したのでまとめる。  
すぐ使い捨てる変数に参照型は使いたくなかったので、ValueTuple一択だったが、どうやってLinqだけで初期化できるかが分かりそうで分からなかった。  
最初はLinq.Repeatを使ったが、初期化に無駄があったのでLinq.Rangeを使うことで解決できた。  

``` C# : 改善したい形
// 愚直に配列を宣言してそれからFor文に入る。
// 変数を2つも用意しないといけないし、New一回で初期化が終わってくれないのでどうしても野暮ったく感じてしまう。
// 最も原始的な方法だから分かりやすいんだけど古臭い。
var hogeArray = new HogeClass[10];
for (int i = 0; i <= hogeArray.Length - 1; i++)
{
    var tmp = FugaList.FirstOrDefault(f => f.Code == i);
    hogeArray[i] = new HogeClass()
    {
        Name = tmp?.Namze,
        Amount = tmp?.Amount ?? decimal.Zero
    };
}
```

``` C# : 完成案
// RangeをFor文に見立て、indexをSelectで取得。
// Select内でタプルを初期化して最後にToList()することで目的を達成できた。
var aaa = Enumerable
    .Range(0, 5)
    .Select(i =>
    {
        var item = FugaList.FirstOrDefault(f => f.Code == i + 1);
        return (
            Name: item?.Name,
            Disp: item?.Amount ?? decimal.Zero,
            Calc: item?.HogeType switch
            {
                HogeType.NotCalc => 0,
                HogeType.Plus => item.Amount ?? decimal.Zero,
                HogeType.Minus => -item.Amount ?? decimal.Zero,
                _ => 0,
            }
        );
    }).ToList();
```

``` C# : 最初に思いついた案(ボツ)
// 繰り返し処理ならRepeatということでやってみたやつ。
// Repeat案だとRepeatの中でまず初期化しないといけないし、そのあとで値を入れなおすので明らかに無駄。
var bbb = Enumerable
    .Repeat<(string Name, decimal Disp, decimal Calc)>(("", 0, 0), 5)
    .Select((s, i) =>
    {
        var item = FugaList.FirstOrDefault(f => f.Code == i + 1);
        s.Name = item?.Name;
        s.Disp = item?.Amount ?? decimal.Zero;
        s.Calc = item?.HogeType switch
        {
            HogeType.NotCalc => 0,
            HogeType.Plus => item.Amount ?? decimal.Zero,
            HogeType.Minus => -item.Amount ?? decimal.Zero,
            _ => 0,
        };
        return s;
    }).ToList();
```

---

## LinqでValueTapleを作る方法

<https://stackoverflow.com/questions/33545249/create-a-tuple-in-a-linq-select>

``` C#
codes = codesRepo.SearchFor(predicate)
    .Select(c => new { c.Id, c.Flag }) // anonymous type
    .AsEnumerable()
    .Select(c => (Id: c.Id, Flag: c.Flag)) // ValueTuple
    .ToList();
```

``` C# : ド安定
    var tupleList = new List<(int Index, string Name)>
    {
        (1, "cow"),
        (2, "chickens"),
        (3, "airplane"),
    };
```

``` C# : ValueTupleのListその1
    var tt = new List<(int, string)>
    {
        (3, "first"),
        (6, "second")
    };
```

``` C# : ValueTupleのListその2
    List<(int example, string descrpt)> list = Enumerable
        .Range(0, 10)
        .Select(i => (example: i, descrpt: $"{i}"))
        .ToList();
```

``` C# : 配列 もいけるよ
    var tupleList = new (int Index, string Name)[]
    {
        (1, "cow"),
        (5, "chickens"),
        (1, "airplane")
    };
```

---

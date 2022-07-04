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
var cash = new TFr_FrontCashDetail[10];
for (int i = 0; i <= cash.Length - 1; i++)
{
    var tmp = frontCashDetail.FirstOrDefault(f => f.DailyReportItemCD == i);
    cash[i] = new TFr_FrontCashDetail()
    {
        DailyReportItemName = tmp?.DailyReportItemName,
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
        var item = balanceDetailList.FirstOrDefault(f => f.CalcItemCD == i + 1);
        return (
            Name: item?.CalcItemName,
            Disp: item?.Amount ?? decimal.Zero,
            Calc: item?.CashExcessOrDeficiencyCalcType switch
            {
                CalculationType.NotCalc => 0,
                CalculationType.Plus => item.Amount ?? decimal.Zero,
                CalculationType.Minus => -item.Amount ?? decimal.Zero,
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
        var item = balanceDetailList.FirstOrDefault(f => f.CalcItemCD == i + 1);
        s.Name = item?.CalcItemName;
        s.Disp = item?.Amount ?? decimal.Zero;
        s.Calc = item?.CashExcessOrDeficiencyCalcType switch
        {
            CalculationType.NotCalc => 0,
            CalculationType.Plus => item.Amount ?? decimal.Zero,
            CalculationType.Minus => -item.Amount ?? decimal.Zero,
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

## ValueTupleをFirstOrDefaultしてnullを判定したい

Equalsメソッド + default句 で実現できた。  
ValueTupleはFirstOrDefaultしてもnullにならない。  

`(int a, int b)?` みたいにやってもいいけど、とりあえず、defaultの形をまとめる。  

<https://twitter.com/kyubuns/status/1379265780240457729>  
[How to null check c# 7 tuple in LINQ query?](https://stackoverflow.com/questions/44307657/how-to-null-check-c-sharp-7-tuple-in-linq-query)  

``` C#
    var tupleList = new List<(int a, int b, int c)>()
    {
        (1, 1, 2),
        (1, 2, 3),
        (2, 2, 4)
    };
    var result = tupleList.FirstOrDefault(f => f.a == 4);

    // タプルのnull判定は Equalsメソッド + default
    if (result.Equals(default(ValueTuple<int,int,int>)))
    {
        Console.WriteLine("Missing!"); 
    }

    // default構文は省略可能(.NetFramework4.8でも可能であることを確認)
    if (Keys.Equals(default())
    {
        Console.WriteLine("Missing!"); 
    }
```

``` C# : null許容型
    var aa = new LinkedList<int>();
    // ?をつけてnull許容型にする
    var tupleList = new List<(int a, int b, int c)?>()
    {
        (1, 1, 2),
        (1, 2, 3),
        (2, 2, 4)
    };
    // null許容型にした場合 ?. のnull合体演算子でなければエラーになる
    // この時の結果はnullとなる。
    var result = tupleList.FirstOrDefault(f => f?.a == 4);

    // nullなので、普通に判定可能
    if (result == null)
    {
        Console.WriteLine("Missing!");
    }
    // こちらでも判定可能。null.Equalsだと思うけどエラーにならない。謎。
    if (result.Equals(default))
    {
        Console.WriteLine("Missing!");
    }

```

# Linq + Tuple

---

## タプル初期化奮闘記

配列を宣言してFor文で回して値をいれるやり方をもっと洗練できないか試行錯誤したのでまとめる。  
すぐ使い捨てる変数に参照型は使いたくなかったので、ValueTuple一択だったが、どうやってLinqだけで初期化できるかが分かりそうで分からなかった。  
最初はLinq.Repeatを使ったが、初期化に無駄があったのでLinq.Rangeを使うことで解決できた。  

■**改善したい形**  

愚直に配列を宣言してそれからFor文に入る。  
変数を2つも用意しないといけないし、New一回で初期化が終わってくれないのでどうしても野暮ったく感じてしまう。  
最も原始的な方法だから分かりやすいんだけど古臭い。  

``` cs
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

■**完成案**  

RangeをFor文に見立て、indexをSelectで取得。  
Select内でタプルを初期化して最後にToList()することで目的を達成できた。  

``` cs
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

■**最初に思いついた案(ボツ)**  

繰り返し処理ならRepeatということでやってみたやつ。  
Repeat案だとRepeatの中でまず初期化しないといけないし、そのあとで値を入れなおすので明らかに無駄。  

``` cs
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

StackOverFlowの案

``` cs
codes = codesRepo.SearchFor(predicate)
    .Select(c => new { c.Id, c.Flag }) // anonymous type
    .AsEnumerable()
    .Select(c => (Id: c.Id, Flag: c.Flag)) // ValueTuple
    .ToList();
```

ド安定

``` cs
    var tupleList = new List<(int Index, string Name)>
    {
        (1, "cow"),
        (2, "chickens"),
        (3, "airplane"),
    };
```

ValueTupleのListその1

``` cs
    var tt = new List<(int, string)>
    {
        (3, "first"),
        (6, "second")
    };
```

ValueTupleのListその2

``` cs
    List<(int example, string descrpt)> list = Enumerable
        .Range(0, 10)
        .Select(i => (example: i, descrpt: $"{i}"))
        .ToList();
```

配列 もいけるよ

``` cs
    var tupleList = new (int Index, string Name)[]
    {
        (1, "cow"),
        (5, "chickens"),
        (1, "airplane")
    };
```

- 参考  
  - [Create a Tuple in a Linq Select](https://stackoverflow.com/questions/33545249/create-a-tuple-in-a-linq-select)  

---

## Dapperでタプルで1件もない場合

普通にCount = 0となるだけ。  
最初が0件なら以降のクエリも実行されない。  
なのでエラーになることはない。  

ValueTupleは値型なので、nullは存在しない。  
その状態で1件もなかった場合、Tupleはどうなるのか、ハッキリわからなかったので検証してみた。  

nullと1件もない状態というのはそもそもが違ったわけでした。  

``` cs
string constr = @"Server=;Database=;User ID=;Password=;Trust Server Certificate=true";
using SqlConnection connection = new SqlConnection(constr);
string query = "SELECT Code,Name,Amount FROM TestTable WHERE Code = @code";
// 0件
var result = connection.Query<(int Code, string Name, decimal Amount)>(query, new { Code = "aaa" }).ToList();
// この状態で処理を進めても0件なので処理は実行されず、エラーにならない。
// aaaの結果も0件のまま。
var aaa = result
    .GroupBy(g => new { g.DepositClsCD, g.DepositClsName })
    .OrderBy(o => o.Key.DepositClsCD)
    .Select(s => new
    {
        s.Key.DepositClsCD,
        s.Key.DepositClsName,
        Count = s.Count(),
        Amount = s.Sum(s => s.Amount)
    })
    .ToList();
```

TupleのListを作成した地点で0件。  
その状態で処理を進めても何もされない。  
なのでTupleのDefault状態に関して意識する必要はない。  

``` cs
// 作成した直後は0件
IEnumerable<(int, string, int)> aa = new List<(int, string, int)>();
// その状態でgroupbyしても0件
var bb = aa.GroupBy(g => new { g.Item1, g.Item2 }).Select(s => new { Item1 = s.Key.Item2, Item2 = s.Sum(ss => ss.Item3) });
// 最後にselectしても0件
var cc = bb.Select(s => new { s.Item1, s.Item2 }).ToList();
```

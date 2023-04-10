# Aggregate

---

## シンプルな例

``` cs
// 整数のリストを作成
List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };

// Aggregateメソッドを使ってリスト内のすべての数値を乗算
// 1. 最初の要素（1）をaccumulatorに設定
// 2. accumulator（1）と次の要素（2）で関数を適用（1 * 2 = 2）、新しいaccumulator（2）に設定
// 3. accumulator（2）と次の要素（3）で関数を適用（2 * 3 = 6）、新しいaccumulator（6）に設定
// 4. accumulator（6）と次の要素（4）で関数を適用（6 * 4 = 24）、新しいaccumulator（24）に設定
// 5. accumulator（24）と次の要素（5）で関数を適用（24 * 5 = 120）、新しいaccumulator（120）に設定
int product = numbers.Aggregate((accumulator, currentValue) => accumulator * currentValue);

// リスト内の数値の積: 120
Console.WriteLine($"リスト内の数値の積: {product}");
```

- 第1引数（accumulator）: これまでの演算の結果が保持されます。最初はシーケンスの最初の要素が設定され、その後は各ステップで関数が適用された結果がアキュムレータとして保持されます。  
- 第2引数（currentValue）: 現在処理されているシーケンス内の要素です。各ステップで次の要素が適用されます。  

Aggregateメソッドでは、シーケンス内の要素に対して関数が累積的に適用され、最終的な結果が得られます。  
この例では、accumulatorにはこれまでの演算の結果が保持され、currentValueにはリスト内の現在の要素が設定されます。  
関数が適用されるたびに、accumulatorは更新され、次のステップで使用されます。  

---

## 公式サンプル

マイクロソフト公式サンプル。  
Aggregateメソッドを使用して、与えられた文字列の配列（fruits）の中で、最も長い文字列を見つけて大文字に変換して表示する。  

``` cs
string[] fruits = { "apple", "mango", "orange", "passionfruit", "grape" };

// 配列内のいずれかの文字列が「banana」より長いかどうかを判断します。
string longestName = fruits.Aggregate(
    // 1. シード値（"banana"）: 最初のアキュムレータとして設定されます。
    "banana",
    // 2. 関数: 現在の最長文字列（longest）と配列内の次の文字列（next）を比較し、次の文字列がより長い場合は次の文字列を返し、そうでない場合は現在の最長文字列を返します。
    (longest, next) =>
    {
        // 1. longest = "banana" : next = "apple"
        // 2. longest = "banana" : next = "mango"
        // 3. longest = "banana" : next = "orange"
        // 4. longest = "banana" : next = "passionfruit"
        // 5. longest = "passionfruit" : next = "grape"
        return next.Length > longest.Length ? next : longest;
    },
    // 3. 結果セレクター: 最終的に見つかった最長の文字列を大文字に変換します。
    fruit =>
    {
        return fruit.ToUpper();
    }
);

// The fruit with the longest name is PASSIONFRUIT.
Console.WriteLine($"The fruit with the longest name is {longestName}.");
```

---

## StringBuilderとの併用

`StringBuilder`は基本的にはAddしていくので、foreachでループすることが一般的だと思われる。  
しかしその場合、1行で記述することができない。  
`Aggregate`を使用した場合は1行で記述することが可能となる。  

``` cs
List<string> items = Enumerable.Range(1, 1000000).Select(x => x.ToString()).ToList();

// foreach ループを使用した場合
StringBuilder sbForeach = new StringBuilder();
foreach (string item in items)
{
    sbForeach.AppendLine(item);
}

// Aggregate メソッドを使用した場合
StringBuilder sbAggregate = items.Aggregate(new StringBuilder(), (sb, item) => sb.AppendLine(item));

Console.WriteLine(sbAggregate.ToString());
```

---

## Aggregate vs foreach

StringBuilderの生成に置ける速度の比較を行う。  
100万回のループではforeachのほうが早い。  

- 検証環境
  - windows 10  
  - .net6

``` cs
using System.Diagnostics;
using System.Text;

List<string> items = Enumerable.Range(1, 1000000).Select(x => x.ToString()).ToList();

// foreach ループを使用した場合
Stopwatch stopwatch = Stopwatch.StartNew();
StringBuilder sbForeach = new StringBuilder();
foreach (string item in items)
{
    sbForeach.AppendLine(item);
}
stopwatch.Stop();
Console.WriteLine($"foreach: {stopwatch.ElapsedMilliseconds} ms");

// Aggregate メソッドを使用した場合
stopwatch.Restart();
StringBuilder sbAggregate = items.Aggregate(new StringBuilder(), (sb, item) => sb.AppendLine(item));
stopwatch.Stop();
Console.WriteLine($"Aggregate: {stopwatch.ElapsedMilliseconds} ms");

// 1回目
// foreach: 39 ms
// Aggregate: 53 ms

// 2回目
// foreach: 39 ms
// Aggregate: 90 ms

// 3回目
// foreach: 40 ms
// Aggregate: 46 ms
```

なぜか1000万回になるとAggregateのほうが早くなった。  
不思議。  

``` cs
List<string> items = Enumerable.Range(1, 10000000).Select(x => x.ToString()).ToList();

// コードは同じ

// 1回目
// foreach: 434 ms
// Aggregate: 353 ms

// 2回目
// foreach: 434 ms
// Aggregate: 342 ms

// 3回目
// foreach: 425 ms
// Aggregate: 336 ms
```

---

## 結合の速度比較

次はStringBuilderの生成ではなく、前後の文字列との結合の場合はどうなるか調査。  
この場合は基本的に`string.Join`の圧勝となった。  
それ以外はほぼ誤差なので、好きなの使えばいいんじゃないかなって感じ。  

``` cs
using System.Diagnostics;
using System.Text;

var data = new[] { "a", "b", "c", "d", "e" };
var iterations = 10000000;
var stopwatch = new Stopwatch();

// Method 1 : string.Join
stopwatch.Start();
for (int i = 0; i < iterations; i++)
{
    var result = string.Join(",", data);
}
stopwatch.Stop();
Console.WriteLine($"Method 1: {stopwatch.ElapsedMilliseconds} ms");
stopwatch.Reset();

// Method 2 : Aggregate + string
stopwatch.Start();
for (int i = 0; i < iterations; i++)
{
    var result = data.Aggregate((x, y) => $"{x},{y}");
}
stopwatch.Stop();
Console.WriteLine($"Method 2: {stopwatch.ElapsedMilliseconds} ms");
stopwatch.Reset();

// Method 3 : Aggregate + StringBuilder + if
stopwatch.Start();
for (int i = 0; i < iterations; i++)
{
    var result = data.Aggregate(
        new StringBuilder(),
        (current, next) => current.Append(current.Length == 0 ? "" : ", ").Append(next)
    )
    .ToString();
}
stopwatch.Stop();
Console.WriteLine($"Method 3: {stopwatch.ElapsedMilliseconds} ms");
stopwatch.Reset();

// Method 4 : Aggregate + StringBuilder + Remove
stopwatch.Start();
for (int i = 0; i < iterations; i++)
{
    var result = data.Aggregate(
        new StringBuilder(),
        (current, next) => current.Append(", ").Append(next),
        res => res.Remove(0,2)
    )
    .ToString();
}
stopwatch.Stop();
Console.WriteLine($"Method 4: {stopwatch.ElapsedMilliseconds} ms");


// 1回目
// Method 1: 550 ms
// Method 2: 1464 ms
// Method 3: 1384 ms
// Method 4: 1508 ms

// 2回目
// Method 1: 532 ms
// Method 2: 1516 ms
// Method 3: 1386 ms
// Method 4: 1560 ms

// 3回目
// Method 1: 555 ms
// Method 2: 1870 ms
// Method 3: 1807 ms
// Method 4: 1631 ms
```

---

``` cs
var innerQuery = new StringBuilder();
foreach (var (item, j) in customerReportDetail.OrderBy(o => o.DataFieldSeqNo).Select((s, i) => (s, i)))
{
   // 先頭に演算子をつけないようにするための判定
   if (j != 0)
   {
       innerQuery.Append("+");
   }
   // プリペアードステートメントのリプレース
   innerQuery.Append(
       _QueryList[item.SettingCls][item.SettingItem].ToString()
           .Replace("@FromCD1", $"{item.FromCD1}")
           .Replace("@ToCD1", $"{item.ToCD1}")
           .Replace("@FromCD2", $"{item.FromCD2}")
           .Replace("@ToCD2", $"{item.ToCD2}")
   );
}
queryAll.Append(innerQuery);
```

``` cs
queryAll.Append(
    customerReportDetail
        .OrderBy(o => o.DataFieldSeqNo)
        .Aggregate(
            new StringBuilder(),
            (accumrate, item) => accumrate
                .Append(accumrate.Length == 0 ? "" : "+")
                .Append(
                    _QueryList[item.SettingCls][item.SettingItem].ToString()
                        .Replace("@FromCD1", $"{item.FromCD1}")
                        .Replace("@ToCD1", $"{item.ToCD1}")
                        .Replace("@FromCD2", $"{item.FromCD2}")
                        .Replace("@ToCD2", $"{item.ToCD2}")
                )
        )
);

queryAll.Append(
    customerReportDetail
        .OrderBy(o => o.DataFieldSeqNo)
        .Aggregate(
            new StringBuilder(),
            (accumrate, item) => accumrate
                .Append("+")
                .Append(
                    _QueryList[item.SettingCls][item.SettingItem].ToString()
                        .Replace("@FromCD1", $"{item.FromCD1}")
                        .Replace("@ToCD1", $"{item.ToCD1}")
                        .Replace("@FromCD2", $"{item.FromCD2}")
                        .Replace("@ToCD2", $"{item.ToCD2}")
                ),
            res => res.Remove(0, 1)
        )
);
```

---

``` cs
int iterations = 100;
var data = new List<string> { "A", "B", "C", "D", "E" };
var data_ = new List<StringBuilder> { 
    new StringBuilder("A"), 
    new StringBuilder("B"),
    new StringBuilder("C"),
    new StringBuilder("D"),
    new StringBuilder("E")
};
var stopwatch = new Stopwatch();

stopwatch.Start();
for (int i = 0; i < iterations; i++)
{
    string queryResult = GenerateQueryWithForeach(data);
}
stopwatch.Stop();
Console.WriteLine($"Foreach method took {stopwatch.ElapsedMilliseconds} milliseconds.");

stopwatch.Restart();
for (int i = 0; i < iterations; i++)
{
    string queryResult = GenerateQueryWithAggregate(data_);
}
stopwatch.Stop();
Console.WriteLine($"Aggregate method took {stopwatch.ElapsedMilliseconds} milliseconds.");


static string GenerateQueryWithForeach(List<string> data)
{
    var queryAll = new StringBuilder();

    foreach (var item in data)
    {
        if (queryAll.Length > 0)
        {
            queryAll.AppendLine("UNION ALL");
        }

        queryAll.AppendLine("SELECT");
        queryAll.AppendLine($"    '{item}'");
        queryAll.AppendLine("    ,(");
        queryAll.Append("..."); // Add your actual query logic here
        queryAll.AppendLine("    )");
    }

    return queryAll.ToString();
}

static string GenerateQueryWithAggregate(List<StringBuilder> data)
{
    return data
        // .Select((item, index) =>
        //     (index != 0 ? "UNION ALL\n" : "") +
        //     $"SELECT\n    '{item}'\n    ,(\n...    )\n" // Add your actual query logic here
        // )
        .Aggregate(new StringBuilder("UNION ALL"),(a, b) => a.AppendLine("SELECT").Append(b))
        .ToString();
}


SELECT 'A'
UNION ALL
SELECT 'B'
UNION ALL
SELECT 'C'
UNION ALL
SELECT 'D'
UNION ALL
SELECT 'E'
```

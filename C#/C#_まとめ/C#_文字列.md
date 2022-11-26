# 文字列関係まとめ

---

## BETWEEN(string.CompareTo)

文字列のBETWEEN比較をするのに便利そうだなと思ってまとめ。  
とりあえずサンプルのように大小関係を取ればBETWEENになる。  

``` C#
// a.CompareTo(b)とした時
// aがbより小さい : -1 : ("9000").CompareTo("9001"),
// aとbが同じ     :  0 : ("9000").CompareTo("9000"),
// aがbより大きい :  1 : ("9000").CompareTo("8999"),
// 8999 9000 9001 ~~~ 9998 9999 10000
//  -1    0    1        1    0    -1
{
    if (NumberFrom.CompareTo(number) <= 0 && NumberTo.CompareTo(number) >= 0)
    {
    }
}
```

---

## プレースホルダー(文字列補完)

基本は`{インデックス}`で置き換え場所を指定する事。  
中かっこ自体を表示する方法も地味にわからなかったが、それは`{{`でエスケープすればよかった。  

``` C# 5.0以前
string stationaries = "Pen";
string fruits = "Pineapple Apple";
// 表示するインデックスが多すぎると、わけわからなくなる。
MessageBox.Show(string.Format("PPAPとは{0} {1} {2}の略である。", stationaries, fruits, stationaries));
// string.Formatを噛まさなくても、Console.Write直でいける。
Console.Write("PPAPとは{0} {1} {2}の略である。", stationaries, fruits, stationaries);
```

``` C# 6.0以降
string stationaries = "Pen";
string fruits = "Pineapple Apple";
// ${}で補完を行う。業務で使ってる形がこれ。こちらのほうが直観的でよい。
// これができるならインデックスで指定する方法は使わなくていいだろう。
// もちろんConsole.Writeでも同じように記述可能。
MessageBox.Show($"PPAPとは{stationaries} {fruits} {stationaries}の略である。");
```

``` C# {}を文字列内に含める
// {{、または}}とする。
string ppap = "Pen Pineapple Apple Pen ";
MessageBox.Show($"{{{ppap}}}");
// 結果は{Pen Pineapple Apple Pen}と表示されます。
```

[【C#】特殊文字「${ }」は文字列補間](https://buralog.jp/csharp-string-interpolation/)  

---

## プレースホルダー中におけるToStringFormatの指定

[【C#6.0～】文字列補間（$を使った文字列書式設定）](https://imagingsolution.net/program/string_interpolation/)  

いつぞや、いつも`.ToString("N0")`ってフォーマット書くところ`:N0`で書けることを発見したのでまとめ。  
プレースホルダー中でのフォーマットの指定はコロン指定ができるができるらしい。便利。  
C#Ver6からの機能みたい。結構実装されてから経っているのね。  

因みにToStringするときに文字列で指定するこれは、「書式設定」というらしい。  

``` C#
// SettlementAmount.ToString("N0") → SettlementAmount:N0
$"支払額{SettlementAmount:N0}円を人数{TargetPlayerCount}人で均等に割り付けます。{Environment.NewLine}よろしいですか？"
```

---

## プレースホルダーな文字列を渡さなくても問題ない

Format関数の第2引数以降の文字列補完があっても、プレースホルダーなしの文字列を渡してもエラーにはならない。  

``` cs
var str = "{0}『{1}』のデータは存在しません。";
System.Console.WriteLine(string.Format(str,"1","2"));
// 1『2』のデータは存在しません。

str = "aaaaa";
System.Console.WriteLine(string.Format(str,"1","2"));
// aaaa
```

---

## 文字列先頭の`@`の意味

文字列先頭の`@`ってなんだっけ？ということでまとめ。  

結論から言うと先頭に`@`があると`\`をまとめてエスケープしてくれる。  
ないと、全部エスケープしないといけない。

``` C#
// 全部エスケープしないとエラーになる。
string constr = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\CSharpSample1\\LocalDB\\SampleDatabase.mdf;";
// @をつけるだけで全部エスケープされるのでエラーにならない。
string constr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\CSharpSample1\LocalDB\SampleDatabase.mdf;";
```

[C#で文字列の￥をエスケープする手間を省く](https://water2litter.net/rye/post/c_str_escape/)  

C#の文字列は２種類ある。  

### 標準リテラル文字列

エスケープ文字を埋め込む必要がある場合。  
0個以上の文字を２重引用符（ダブルクォーテーション）で囲んで指定する。  
例: "Hello\n"  

### 逐次的リテラル文字列

文字列テキストに¥記号が含まれる場合。
@文字、２重引用符（ダブルクォーテーション）、0個以上の文字、閉じる用の２重引用符で指定する。  
文字列テキスト内に２重引用符が含まれる場合は、その２重引用符に２重引用符を付ける。  
例: @"Hello"  

### 逐次的リテラル文字列を試してみた

``` C#
    // 標準リテラル文字列
    string Text1 = "千早振る神代もきかず竜田川\n唐紅に水くくるとは"; 
    // 千早振る神代もきかず竜田川
    // 唐紅に水くくるとは

     // 逐次的リテラル文字列
    string Text2 = @"千早振る神代もきかず竜田川\n唐紅に水くくるとは";
    // 千早振る神代もきかず竜田川\n唐紅に水くくるとは

    // 改行を含む逐次的リテラル文字列
    string Text3 = @"千早振る神代もきかず竜田川
    唐紅に水くくるとは"; 
    // 千早振る神代もきかず竜田川
    // 唐紅に水くくるとは

    // ２重引用符を含む逐次的リテラル文字列
    string Text4 = @"千早振る神代もきかず""竜田川"" 唐紅に水くくるとは"; 
    // 千早振る神代もきかず"竜田川" 唐紅に水くくるとは
```

---

## Like検索(曖昧検索)

[LINQ：文字列コレクションで「LIKE検索」（部分一致検索）をするには？［C#、VB］](https://atmarkit.itmedia.co.jp/ait/articles/1412/02/news129.html)  

``` C#
    // 曖昧検索 LIKE '%ぶた%'
    var 前後パーセント = sampleData.Where(item => item.Contains("ぶた"));
    WriteItems("LIKE '%ぶた%'", 前後パーセント);
    // → LIKE '%ぶた%': ぶた, こぶた, ぶたまん, ねぶたまつり
    
    // 前方一致 LIKE 'ぶた%'
    var 後パーセント = sampleData.Where(item => item.StartsWith("ぶた"));
    WriteItems("LIKE 'ぶた%'", 後パーセント);
    // → LIKE 'ぶた%': ぶた, ぶたまん

    // 後方一致 LIKE '%ぶた'
    var 前パーセント = sampleData.Where(item => item.EndsWith("ぶた"));
    WriteItems("LIKE '%ぶた'", 前パーセント);
    // → LIKE '%ぶた': ぶた, こぶた
```

---

## StringBuilderで先頭にWHEREを追加する方法

``` C#
    var whereQuery = new StringBuilder();
    if (!string.IsNullOrEmpty(testNumber))
    {
        whereQuery.AppendLine("AND [TestNumber] = @testNumber");
    }
    if (!string.IsNullOrEmpty(testID))
    {
        whereQuery.AppendLine("AND [TestID] = @testID");
    }
    if (testFlag)
    {
        whereQuery.AppendLine("AND [testFlag] <> @testFlag");
    }
    // 先頭のAND消してWHEREにする。
    whereQuery.Remove(0, 3).Insert(0, "WHERE");
```

---

## 文字列のインクリメント

[【C#】文字列内の末尾の数値をインクリメントするサンプル](https://baba-s.hatenablog.com/entry/2020/05/06/001800)  
[c# - 文字と数字の両方で文字列をインクリメントします](https://tagsqa.com/detail/45665)  

正規表現で数字を取得。  
intに変換してインクリメントして文字列に直す。
もしくは結合しなおすことでインクリメントを実現する。  

usingはこれ。  
`uging System.Text.RegularExpressions;`  

``` C# : どういう原理かわからんが、一番スマートかも
// "MD123" → "MD124"
// "123MD" → "124MD"
//  "7000" → "7001"
var newString = Regex.Replace(
    input,
    "\\d+",
    m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length))
);
```

``` C# : 拡張メソッドにした
// こんな芸当が可能
string str = "MD123".Increment();

//拡張メソッド定義
public static class StringExtension
{
    public static string Increment(this string input)
    {
        return Regex.Replace(
            input,
            "\\d+",
            m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length))
        );
    }
}
```

上のさえできれば後のやつを使う意味はないけど、備忘録として残す

``` C# : 案1
// "MD123" → "MD124"
// "123MD" →    Err
//  "7000" →  "7001"
string input = "MD123";

var valueString = new Regex("([0-9]*$)").Match(input).Value;
var value = int.Parse(valueString) + 1;
var digits = Math.Min(value == 0 ? 1 : (int)Math.Log10(value) + 1, valueString.Length);
var result = input.Remove(input.Length - digits, digits) + value;

// 関数としてまとめた例
var accountNo = new Func<string, string>((input) =>
{
    var valueString = new Regex("([0-9]*$)").Match(input).Value;
    var value = int.Parse(valueString) + 1;
    var digits = Math.Min(value == 0 ? 1 : (int)Math.Log10(value) + 1, valueString.Length);
    return input.Remove(input.Length - digits, digits) + value;
})(maxAccountNo);
```

``` C# : 案2
// "MD123" → "MD124"
// "123MD" →    Err
//  "7000" →    Err

var match = Regex.Match("MD123", @"^([^0-9]+)([0-9]+)$");
// [0] : MD123
// [1] : MD
// [2] : 123
var num = int.Parse(match.Groups[2].Value);
// MD + 124
var after = match.Groups[1].Value + (num + 1);
```

---

## C#で0埋めする方法

[C#で0埋めする方法](https://santerabyte.com/c-sharp-zero-padding/)  

0埋めする場合は「D」のあとに桁数を指定するとその桁数になるように0が追加された文字列が返されます。  

``` C#
int num = 123;
// str = "00123"
string str = num.ToString("D5");
```

---

## 文字列補完式の中で三項演算子を使う

文字列補完式`{}`の中で三項演算をしつつ、なおかつ文字列を追加したい場合の書き方が分からなかったが、結果的にできたのでまとめる。  
単純に三項演算子を括弧で囲えばよかった。  
三項演算子の部分を1つの処理と見立てる意味では括弧で囲うのは自然なことなのかもしれない。  

[C# における文字列補間](https://docs.microsoft.com/ja-jp/dotnet/csharp/tutorials/string-interpolation)  

``` C#
// 愚直にやるならこう
var aa = "【" + (!string.IsNullOrEmpty(customer.Name) ? customer.Name : "席No" + customer.SeatNo) + "】様";

// 三項演算子使いたいならこう {( 処理内容 )}
var aa = $"【{(!string.IsNullOrEmpty(customer.Name) ? customer.Name : "席No" + customer.SeatNo)}】様";
```

---

## ToString()の書式設定

[書式を指定して数値を文字列に変換する](https://dobon.net/vb/dotnet/string/inttostring.html)  

Enum.ToString("D")で数値に変換できるのは知っていたけど、まとめていなかったのでまとめる。  

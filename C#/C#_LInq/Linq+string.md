# Linq文字列関係まとめ

---

## Linqによる文字列連結

1. string.Join  
2. Linq.Aggregate  

主にAggregateとString.joinを使う方法があるけれど、String.joinを使うことをおすすめする。  
理由は速度だそうだ。  

■ **string.Join方式**

これ使えばよろしい。

``` cs
var data = new[] { "a", "b", "c" };
Console.WriteLine(string.Join(",", data));
// 出力は「a,b,c」
```

■ **Linq.Aggregate方式**

"a"  
"a" + "," + "b"  
"a,b" + "," + "c"  

のような文字列の結合となるため結合文字数が増えると大きなパフォーマンス低下が生じる。  

``` cs
var data = new[] { "a", "b", "c" };
Console.WriteLine(data.Aggregate((x, y) => $"{x},{y}"));
```

■ **おまけ Aggregate+StringBuilder方式**

チェーンしすぎてわけわからんので、自己満足以外で使うことはないだろう。

```cs
var data = new[] { "a", "b", "c" };
var res = words.Aggregate(
        new StringBuilder(), 
        (current, next) => current.Append(current.Length == 0? "" : ", ").Append(next)
    )
    .ToString();
Console.WriteLine(res);
```

[なんか外国のよく見るところ](https://www.it-swarm-ja.com/ja/c%23/linq%E3%82%92%E4%BD%BF%E7%94%A8%E3%81%97%E3%81%A6%E6%96%87%E5%AD%97%E5%88%97%E3%82%92%E9%80%A3%E7%B5%90%E3%81%99%E3%82%8B/958428705/)  
[複数の文字列をセパレータ文字で区切って結合する方法(LINQ編)](https://www.exceedsystem.net/2020/08/29/how-to-join-multiple-strings-with-delimiter/)  

---

## 実装例1

精算済みの警告を出すためメッセージを生成する例

```C# : 実装例1
var warningMessage = TestModel
    .GetList(condition)
    .Where(w => w.SettlementFlag == true)
    ?.Select(s => s != null
        ? (!string.IsNullOrEmpty(s.TestNo) ? "【" + s.TestNo + "】　" : string.Empty)
            + (!string.IsNullOrEmpty(s.Name) ? "【" + s.Name + "】様" : string.Empty)
            + "は精算済みです。"
        : null
    )
    ?.Aggregate(
        "",
        (a, b) => a + Environment.NewLine + b
    );
```

## 実装例2

``` cs
// これを
if (TestList?.Any() == true)
{
    var context = "文字列";
    foreach (var item in TestList)
    {
        var name = item.Name ?? string.Empty;
        var len = 60 - Encoding.GetEncoding("shift_jis").GetByteCount(name.ToCharArray());
        context += Environment.NewLine + " " + name.PadRight(len) + string.Format("￥{0:#,0}", item.Amount);
    }
    Report = context;
}

// こうできた
Report = TestList?.Any() == true
    ? "文字列" + TestList
        .Select(s =>
        {
            string name = s.Name ?? string.Empty;
            int len = 60 - Encoding.GetEncoding("shift_jis").GetByteCount(name.ToCharArray());
            // 名前 空白 \金額 の構成にする
            return $"{ name.PadRight(len)}￥{s.Amount:#,0}";
        })
        ?.Aggregate("", (a, b) => a + Environment.NewLine + b)
    : string.Empty;
```

---

## Joinを使った作成例

例えば、ある条件で絞った結果の内、区分だけをSelectする。  
その結果が `{ "010", "020", "030", "040", "050" }` のような文字列の区分だったとして、「010」「020」
「030」の区分が存在する場合、それに対応する文言に変換して、エラー文字列を構築したい。  

- 複数ある場合はカンマ区切り
  - 「Type_A,Type_B,Type_C区分が入力されています」  
- 単品の場合はその文字列だけ
  - 「Type_B区分が入力されています」  
- 「010、020、030」がなく、逆に「040、050」があった場合はデフォルトの文言を表示したい。
  - 「区分入力されています」  

そういう文字列を構築する処理をLinqでいい感じに書けないか、ウンウン悩んだ。  
結果的に変換テーブルとJoinでスッキリまとめることができたので、まとめる。  

■ **愚直案**

前提として、区分は専用の定数クラスに定義されているものとする。  
まず愚直にやった場合。  
Whereで対象の区分だけに絞り込んで、それぞれの区分を観測したら文字列をreturnする案。  
長いし、エラー出力の文言をifで分けないといけないし、区分を2回も呼び出している。  
要件は満たすが、到底、満足できるものではない。  

``` C# : 愚直案
// 定数クラス
static class TestType
{
    public static string Type_A => "010";
    public static string Type_B => "020";
    public static string Type_C => "030";
    public static string Type_D => "040";
    public static string Type_E => "050";
}

var testList = new List<string>() { "010", "020", "030", "040", "050" };
var msg = string.Join(
    ",",
    testList
        // 対象の区分だけに絞る
        .Where(w => w == TestType.Type_A || w == TestType.Type_B || w == TestType.Type_C)
        // 文言の順番は変えたくないのでOrderする
        .OrderBy(o => o)
        .Select(s =>
        {
            // 絞った後に更に観測する必要がある
            if (s == TestType.Type_A)
            {
                return "Type_A";
            }
            else if (s == TestType.Type_B)
            {
                return "Type_B";
            }
            // Type_CだけelseにしないとSelectがエラーになってしまうのも納得いかない。
            else
            {
                return "Type_C";
            }
        })
);
// 最初は気が付かなかったが、これは1行で書ける。  
// しかし、愚直にやってた時は、出力部分もifで分けないといけないのも気に食わなかった。
if (!string.IsNullOrEmpty(msg))
{
    throw new Exception(msg + "区分が入力されています");
}
if (testList.Any())
{
    throw new Exception("区分が入力されています");
}
```

■ **変換テーブル検索案**

ifで判定する部分だけでも何とかならないかな？  
と、思って閃いたのが変換テーブルを作る案。  
これなら長大なifの判定部分を1行にすることができる。  
elseの必要もない。  
この地点で割と満足できていた。  

``` C# : 変換テーブル検索案
// 定数クラス
static class TestType
{
    public static string Type_A => "010";
    public static string Type_B => "020";
    public static string Type_C => "030";
    public static string Type_D => "040";
    public static string Type_E => "050";
}

var testList = new List<string>() { "010", "020", "030", "040", "050" };
// 変換テーブルを定義
var conditionPair = new List<(string Type, string Name)>()
{
    (TestType.Type_A,"Type_A"),
    (TestType.Type_B,"Type_B"),
    (TestType.Type_C,"Type_C")
};
var msg = string.Join(
    ",",
    testList
        // やはり順番は整える
        .OrderBy(o => o)
        // 変換テーブルに対して検索をかける。なければnull。
        .Select(s => conditionPair.FirstOrDefault(w => w.Type == s).Name)
        // 最後にnullを排除する
        .Where(w => !string.IsNullOrEmpty(w))
);
// 010,020,030にヒットしなかった場合、whereで弾かれて、msgがnullになる。
// するとtestListには必然的に040,050が残っていると判定できるので、||でチェーンできる。
// msgは何もないのでそのままJoinしても表示に問題はない。
if (!string.IsNullOrEmpty(msg) || testList.Any())
{
    throw new Exception(msg + "区分が入力されています");
}
```

■ **変換テーブル + Join案**

変換テーブルで検索できるなら、変換テーブルとInnerJoinしてしまえば検索もせずに済むのでは？  
というわけで、やってみたらできた。  
出来た時は感動した。ここまで出来たら文句はない。  

``` C# : 変換テーブル + Join案
// 定数クラス
static class TestType
{
    public static string Type_A => "010";
    public static string Type_B => "020";
    public static string Type_C => "030";
    public static string Type_D => "040";
    public static string Type_E => "050";
}

var testList = new List<string>() { "010", "020", "030", "040", "050" };
// 変換テーブルを定義
var conditionPair = new List<(string Type, string Name)>()
{
    (TestType.Type_A,"Type_A"),
    (TestType.Type_B,"Type_B"),
    (TestType.Type_C,"Type_C")
};
// 変換テーブルをJoin
var msg = string.Join(",",conditionPair.Join(testList, c => c.Type, s => s, (c, s) => c.Name));
// InnerJoinなので1件もなければもちろん何もないので、2番目の条件と同じロジックでいける。
if (!string.IsNullOrEmpty(msg) || testList.Any())
{
    throw new Exception(msg + "区分が入力されています");
}
```

---

## n番目の特定文字から後ろだけが欲しい

例えばこういう文字列があったする。  
`DbUp_Journal.Scripts.Dat._202301.202301271317_Fix1_TestTable1_Create.sql`  

最終的にこうしたい。  
`Dat._202301.202301271317_Fix1_TestTable1_Create.sql`  

最初の2単語の`DbUp_Journal.Scripts`の部分はいらない。  
構造的に`.`でスプリットできることが分かる。  

「最初の2つはいらない」という部分は、`Skip`で実現できた。  
Splitして分解したものは`string.Join`で元に戻す。  
切って張ってみたいな感じになるが、まぁいいだろう。  

``` cs
var abs_path = "DbUp_Journal.Scripts.Dat._202301.202301271317_Fix1_TestTable1_Create.sql";
var result = string.Join(".", abs_path.Split(".").Skip(2));
// Dat._202301.202301271317_Fix1_TestTable1_Create.sql
```

---

## 100行以内のものを表示する

100行以下を削除する。

コンソールログで出力するときに、何から何まで出力すると長すぎて先頭の情報が見切れてしまう。  
先頭の数行が確認できれば充分なので、いい感じのところまで表示して、あとはいらないって処理を作りたい。  
「n番目の特定文字から後ろだけが欲しい」の応用で行けた。  

``` cs
string hoge = string.Join(Environment.NewLine, new string[] {"aaaa","bbbb","cccc","dddd","eeee"});
var aaa = string.Join(Environment.NewLine,hoge.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries).Take(3));
Console.WriteLine(aaa);

// aaaa
// bbbb
// cccc
```

``` cs
string hoge = string.Join(Environment.NewLine, new string[] {"aaaa","bbbb","cccc","dddd","eeee"});
var aaa = hoge.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries).Take(2);
foreach (var item in aaa)
    Console.WriteLine(item);

// aaaa
// bbbb
```

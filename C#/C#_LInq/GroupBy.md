# GroupBy

---

## GroupByの基本的な動作

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

パターン1

``` cs
var pattern1 = tupleList
    .GroupBy(
        g => new { g.ClsCode, g.ClsName },
        (k, v) => new
        {
            k.ClsCode,
            k.ClsName,
            TotalScore = v.Sum(s => s.Score)
        }
    )
    .ToList();
```

パターン2

``` cs
var pattern2 = tupleList
    .GroupBy(g => new { g.ClsCode, g.ClsName })
    .Select(s => new
    {
        s.Key.ClsCode,
        s.Key.ClsName,
        TotalScore = s.Sum(sum => sum.Score)
    })
    .ToList();
```

pattern1とpattern2の結果は同じになる。  

``` txt
[0]: { ClsCode = 1, ClsName = "国語", TotalScore = 220 }
[1]: { ClsCode = 2, ClsName = "数学", TotalScore = 210 }
[2]: { ClsCode = 3, ClsName = "英語", TotalScore = 240 }
```

---

## GroupByして単純に足したい場合

``` C#
ConsumptionTaxList = aac
    .GroupBy(g => (g.TaxationType, g.TaxRate))
    .Select(s =>
    {
        var taxSlip = s.FirstOrDefault();
        taxSlip.Price = s.Sum(sum => sum.Price);
        taxSlip.Tax = s.Sum(sum => sum.Tax);
        return taxSlip;
    }).ToList();
```

---

## GroupByした要素の内、先頭のものだけ取得する

やりたいこと  
割り勘の伝票IDを取得して、重複を排除して、それが2件以上あれば、別々で割り勘を実行した人がいるという事なので、呼び出せないようにしたい。  
ついでに、誰と誰が別々に割り勘しているのかをアナウンスしたいので、求めた伝票IDを持っている人の中でそれぞれ先頭の人だけを抜き出したい。  
意外と難しかった。SelectManyとGroupByを組み合わせてうまい事出来たので、まとめる。  

追記：  
SelectManyとGroupByの第1,2引数まで使ったサンプルは中々難しかった。  
第1引数の結果が第2引数のラムダのどちらの引数に入ってくるかも違うのも悩み物だ。  

とりあえずSelectManyで第1引数と第2引数を使った場合の動作はどういうモノになるのかについて。  

[SelectMany の使い方](https://qiita.com/youmts/items/97aab4fb9322746964c6)  
このページのやり方がやばいくらい分かりやすい。  

``` C# : SelectMany で上位と下位をまとめて処理
// 上位の要素（例だとAuthor)と、下位の結果の要素（例だとbookName）をまとめて処理するラムダ式を追加できます。
// １つ目のラムダ式の結果の要素一つ一つに対して、２つ目のラムダ式が呼び出されます。
var bookNames = authors.SelectMany(
    // 第1引数のラムダでそれぞれの著者のbookNameの一覧を取得。
    // 芥川龍之介{羅生門, 蜘蛛の糸, 河童}, 江戸川乱歩{人間椅子, 怪人二十面相}, 川端康成{雪国, 伊豆の踊り子}
    // 一見すると、ここで7冊全てをピックアップしてから2つ目のラムダを実行するように見えるがそうではなく、
    // 1つ目のラムダの結果の1つ1つに対して2つ目のラムダが呼び出される。
    // 順を追うと、まず芥川で3冊。羅生門をピックアップした地点で2つ目のラムダを実行する。以降の本も同様。
    // 芥川の本全てのループが終わったら、次の著者の本1つ1つに対して2つ目のラムダを実行。後は同じ。
    author => author.Books.Select(book => book.Name),
    // 第2引数のラムダはラムダの第2引数に上位の結果が格納された状態で処理を開始する。
    // bookNameに第1引数の結果の要素が入ってくる。
    // authorは芥川{本3冊}の構成で、次の著者のループになるまで値は変わらない。
    // bookNameが{羅生門, 蜘蛛の糸, 河童}の順でやってきて、芥川に関してはは3回処理が実行されることになる。
    (author, bookName) => $"{bookName}/{author.Name}"
);

// 結果
// 羅生門/芥川龍之介, 蜘蛛の糸/芥川龍之介, 河童/芥川龍之介, 人間椅子/江戸川乱歩, 怪人二十面相/江戸川乱歩, 雪国/川端康成, 伊豆の踊り子/川端康成
```

[LINQでグルーピングした最大の要素を取得する](https://shinsuke789.hatenablog.jp/entry/2019/09/14/114009)  
次にGroupByでの第1引数と第2引数がある場合についてまとめ。  

・第1引数：元の値を何でグループ化するかを指定する  
・第2引数：元の値をグループ化したものに対して、キーと値の2つの引数が使え、値に対しての処理を指定する  

``` C# : Key毎にDtが最大の要素を取得するサンプル
    // こういうデータが来たとする。
    // Key = 1, Dt = "20190101"
    // Key = 1, Dt = "20190102"
    // Key = 2, Dt = "20190101"
    // Key = 3, Dt = "20190310"
    // Key = 3, Dt = "20190305"
    // Key = 3, Dt = "20190301"
    // Key = 4, Dt = "20190101"
    // Key = 4, Dt = "20190102"
    // Key = 5, Dt = "20190103"

    List<Hoge> retList = list
        .GroupBy(
            // 第1引数では、何のキーでグループ化するか指定する。
            // 今回はKeyなので、Keyでグループ化すると内部ではキーの数の分ループする。
            // 1,1,2,3,3,3,4,4,5
            h => h.Key,
            // 第2引数では、キーとキーでグループ化されたレコードが来る。
            // kが上位の結果。重複はなくなっていた。なので1,2,3,4,5と順番に来る。
            // vがキーでグループ化したレコード。
            // 1回目のループはk=1, v={Key = 1, Dt = "20190101"},{Key = 1, Dt = "20190102"}
            // 2回目のループはk=2, v={Key = 2, Dt = "20190101"}
            // 3回目のループはk=3, v={Key = 3, Dt = "20190310"},{Key = 3, Dt = "20190305"},{Key = 3, Dt = "20190301"}
            // 以下略
            // 後はvに対して更にOrderByをかけ、DtをDESCしてfirstを取ると、そのキーの中での最大値が取れるというわけ。
            (k, v) => v.OrderByDescending(o => o.Dt).First())
        .ToList();

    // 1 20190102
    // 2 20190101
    // 3 20190310
    // 4 20190102
    // 5 20190103
    retList.ForEach(f => Console.WriteLine($"{f.ley} {f.Dt}"));
```

[groupbyして先頭1](https://entityframework.net/knowledge-base/3850429/get-the-first-record-of-a-group-in-linq-)  
[groupbyして先頭2]<https://stackoverflow.com/questions/19012986/how-to-get-first-record-in-each-group-using-linq/39932057>  
親戚もおいておく。  

``` C#
var count = SettlementDetailList
   // 割り勘の伝票IDを取得
   .Select(s => s.SlipList.FirstOrDefault(w => w.SlipType == SlipType.DutchTreat)?.SlipID)
   // 同じ伝票IDはカウントしない。
   .Distinct()
   // nullを除いた結果が2以上なら、別々の割り勘伝票の人をセットしているのでダメ。
   .Count(c => c != null);
if (count >= 2)
{
    MessageDialogUtil.ShowWarning(Messenger,"dame");
    return;
};

// とりあえず完成形。
// IDだけは別で抜き出さないといけないのがちょっとあれだけど、頑張ればいけそうな気もする。
IEnumerable<string> dutchTreatSlipIDList = SettlementDetailList
    // 割り勘の伝票IDを取得
    .Select(s => s.SlipList.FirstOrDefault(w => w.SlipType == SlipType.DutchTreat)?.SlipID)
    .Distinct()
    .Where(w => w != null)
    .ToList();
if (dutchTreatSlipIDList.Count() >= 2)
{
    IEnumerable<string> targetPlayerList = SettlementDetailList
        // 上の例に即せば、ここではこのようなレコードが出来上がる。
        // AccountNo:0001 , name:A , SlipID AAA
        // AccountNo:0001 , name:A , SlipID AAA
        // AccountNo:0001 , name:A , SlipID BBB
        // AccountNo:0002 , name:B , SlipID DDD
        // AccountNo:0002 , name:B , SlipID EEE
        // AccountNo:0002 , name:B , SlipID EEE
        .SelectMany(
            p => p.SlipList,
            (s, slip) => (s.AccountNo, s.ReservationPlayerName, slip.SlipID)
        )
        // AAA,DDDだけにする。
        // AccountNo:0001 , name:A , SlipID AAA
        // AccountNo:0001 , name:A , SlipID AAA
        // AccountNo:0002 , name:B , SlipID DDD
        .Where(w => dutchTreatSlipIDList.Contains(w.SlipID))
        // GroupByした時のキーとバリューの構成
        // k=SlipID:AAA, v={AccountNo:0001 , name:A , SlipID AAA},{AccountNo:0001 , name:A , SlipID AAA}
        // k=SlipID:DDD, v={AccountNo:0002 , name:B , SlipID DDD}
        .GroupBy(
            p => p.SlipID,
            (k, v) => (key: k, value: v.FirstOrDefault())
        )
        // 最終結果
        // v={AccountNo:0001 , name:A , SlipID AAA}
        // v={AccountNo:0002 , name:B , SlipID DDD}
        .Select(s => $"【{s.value.AccountNo}】【{s.value.ReservationPlayerName}】様")
        .ToList();
    var msg = string.Join(" と ", targetPlayerList) + " はそれぞれで既に割り勘済みのため、割り勘を開くことができません。";
    MessageDialogUtil.ShowWarning(Messenger, msg);
    return;
}
```

---

## groupbyに条件指定可能

連番を指定した数の塊に分解する方法と合わせての話になる。  
連番を塊にわけてFromToを取れたとして、次はその範囲内の年齢を集めて集計しないといけない。  
真っ先に思い浮かぶのはgroupbyを使う事だが、`from < age && age < to` という条件を当てはめる事ができるのかわからなかった。  
実際にやってみたら普通に出来たのでまとめ。  

groupbyに条件を指定した場合、Keyがboolになる。  
Trueの集合とFalseの集合が生成されるので、`group().where(w => w.key)`とすることでtrueの集合だけを操作可能になる。  
この時、select句のsはIGroupingインターフェースで返却されるので、普通の型に戻したかったらs.ToList()とするればよい。  

whereで絞った後にgroupbyでもいいのかもしれない。  
てか、普通にそれでいいか？  
それでいいかも・・・。  
いいといえばいいが、微妙に解釈が違うので、それはそれで何とかしないといけないかも。  
とりあえず、目的のものがある程度スマートに出来たし、頭でも理解できたので良しとしましょう。  

GroupByで条件で絞れて、WhereでTrueのモノを拾えば、その条件でグルーピングされた一覧を操作可能って話。

``` C# : 完成形
    enum GenderType
    {
        Male,
        Female
    }

    static List<(string Name, GenderType Gender, int Age)> tupleList = new List<(string Name, GenderType Gender, int Age)>
        {
            ("井村 由宇",GenderType.Female,58),
            ("脇田 大",GenderType.Male,30),
            ("平 千佳子",GenderType.Female,75),
            ("江川 那奈",GenderType.Female,66),
            ("永島 美幸",GenderType.Female,80),
            ("中塚 明日",GenderType.Female,55),
            ("黒木 りえ",GenderType.Female,52),
            ("戎 杏",GenderType.Female,45),
            ("菅 亮",GenderType.Male,79),
            ("池本 しぼり",GenderType.Female,38),
            ("本多 路子",GenderType.Female,52),
            ("山内 博明",GenderType.Male,21),
            ("木下 さやか",GenderType.Female,27),
            ("市川 竜也",GenderType.Male,61),
            ("山城 りえ",GenderType.Female,41),
            ("長田 窈",GenderType.Female,79),
            ("井手 秀樹",GenderType.Male,35),
            ("松井 明慶",GenderType.Male,68),
            ("立石 たまき",GenderType.Female,79),
            ("とよた さやか",GenderType.Female,51),
            ("小木 美月",GenderType.Female,77),
            ("今西 まなみ",GenderType.Female,66),
            ("河本 友也",GenderType.Male,74),
            ("北条 ヒカル",GenderType.Female,24),
            ("天野 瑠璃亜",GenderType.Female,68),
            ("大塚 浩正",GenderType.Male,69),
            ("真田 大五郎",GenderType.Male,57),
            ("堀井 京子",GenderType.Female,76),
            ("渡辺 美佐",GenderType.Female,51),
            ("浅川 美帆",GenderType.Female,75),
            ("岩井 賢治",GenderType.Male,20),
            ("村瀬 莉沙",GenderType.Female,31),
            ("市川 研二",GenderType.Male,57),
            ("河原 美幸",GenderType.Female,27),
            ("黒岩 憲史",GenderType.Male,30),
            ("薬師丸 美嘉",GenderType.Female,62),
            ("阿部 禄郎",GenderType.Male,50),
            ("吉野 公顕",GenderType.Male,48),
            ("八十田 隼士",GenderType.Male,40),
            ("清田 美和子",GenderType.Female,59),
            ("矢口 あさみ",GenderType.Female,26),
            ("米沢 明宏",GenderType.Male,80),
            ("神野 礼子",GenderType.Female,55),
            ("辻 三郎",GenderType.Male,29),
            ("百瀬 有海",GenderType.Female,32),
            ("村瀬 俊介",GenderType.Male,73),
            ("金丸 寿明",GenderType.Male,43),
            ("小寺 勝久",GenderType.Male,25),
            ("今泉 由宇",GenderType.Female,64),
            ("岡田 大",GenderType.Male,40),
        };

    static void Do(int chunkSize = 1)
    {
        var male = new List<(string test, int count)>();
        var female = new List<(string test, int count)>();

        (string test, int count) CreateT(List<(string Name, GenderType Gender, int Age)> s, string name) =>
            (test: name, count: s.Count());

        // 2万ms
        foreach (var chunk in Enumerable.Range(0, 150)
            .Select((v, i) => (v, i))
            .GroupBy(x => x.i / chunkSize)
            .Select(g => g.Select(x => x.v))
            .ToList())
        {
            var from = chunk.Min();
            var to = chunk.Max();
            var fromTo = from.ToString() + "～" + to.ToString();

            male.AddRange(
                tupleList
                    .GroupBy(g => (Gender: g.Gender == GenderType.Male, Age: g.Age >= from && g.Age <= to))
                    .Where(w => w.Key.Gender && w.Key.Age && w.Any())
                    .Select(ss => CreateT(ss.ToList(), "男性 " + fromTo))
                    .ToList()
            );
            female.AddRange(
                tupleList
                    .GroupBy(g => (Gender: g.Gender == GenderType.Female, Age: g.Age >= from && g.Age <= to))
                    .Where(w => w.Key.Gender && w.Key.Age && w.Any())
                    .Select(ss => CreateT(ss.ToList(), "女性 " + fromTo))
                    .ToList()
            );
        }
    }
```

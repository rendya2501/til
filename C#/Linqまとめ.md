# Linqのあれこれ

## Linqのサンプル用タプルリスト(タプルのリストを簡単に初期化する方法)

いつもやっているような、whereで要素にアクセスするのを簡単に実現出来ないかやってみた奴。  
2,3個のフィールドのためにわざわざクラスは作りたくない。  
ValueTupeを使えばいつもの感じでフィールドにアクセスできるので、ちょっとやる分にはこれでいいでしょう。  

``` C# : これ使え
    var tupleList = new List<(int Index, string Name)>
    {
        (1, "cow"),
        (2, "chickens"),
        (3, "airplane"),
    };

    var tupleList = new List<(string Name, byte Gender, int Age)>
    {
        ("井村 由宇",1,58),
        ("脇田 大",0,30),
        ("平 千佳子",1,75),
        ("江川 那奈",1,66),
        ("永島 美幸",1,80),
        ("中塚 明日",1,55),
        ("黒木 りえ",1,52),
        ("戎 杏",1,45),
        ("菅 亮",0,79),
        ("池本 しぼり",1,38),
        ("本多 路子",1,52),
        ("山内 博明",0,21),
        ("木下 さやか",1,27),
        ("市川 竜也",0,61),
        ("山城 りえ",1,41),
        ("長田 窈",1,79),
        ("井手 秀樹",0,35),
        ("松井 明慶",0,68),
        ("立石 たまき",1,79),
        ("とよた さやか",1,51),
        ("小木 美月",1,77),
        ("今西 まなみ",1,66),
        ("河本 友也",0,74),
        ("北条 ヒカル",1,24),
        ("天野 瑠璃亜",1,68),
        ("大塚 浩正",0,69),
        ("真田 大五郎",0,57),
        ("堀井 京子",1,76),
        ("渡辺 美佐",1,51),
        ("浅川 美帆",1,75),
        ("岩井 賢治",0,20),
        ("村瀬 莉沙",1,31),
        ("市川 研二",0,57),
        ("河原 美幸",1,27),
        ("黒岩 憲史",0,30),
        ("薬師丸 美嘉",1,62),
        ("阿部 禄郎",0,50),
        ("吉野 公顕",0,48),
        ("八十田 隼士",0,40),
        ("清田 美和子",1,59),
        ("矢口 あさみ",1,26),
        ("米沢 明宏",0,80),
        ("神野 礼子",1,55),
        ("辻 三郎",0,29),
        ("百瀬 有海",1,32),
        ("村瀬 俊介",0,73),
        ("金丸 寿明",0,43),
        ("小寺 勝久",0,25),
        ("今泉 由宇",1,64),
        ("岡田 大",0,40),
    };
```

``` C# : 他の書き方
    // ValueTupleのListその1
    var tt = new List<(int, string)>
    {
        (3, "first"),
        (6, "second")
    };

    // ValueTupleのListその2
    List<(int example, string descrpt)> list = Enumerable
        .Range(0, 10)
        .Select(i => (example: i, descrpt: $"{i}"))
        .ToList();

    // 配列 もいけるよ
    var tupleList = new (int Index, string Name)[]
    {
        (1, "cow"),
        (5, "chickens"),
        (1, "airplane")
    };
```

---

## タプル初期化奮闘記

配列を宣言してFor文で回して値をいれるやり方をもっと洗練できないか試行錯誤したのでまとめる。  
すぐ使い捨てる変数に参照型は使いたくなかったので、ValueTuple一択だったが、どうやってLinqだけで初期化できるかが分かりそうで分からなかった。  
最初はLinq.Repeatを使ったが、初期化に無駄があったのでLinq.Rangeを使うことで解決できた。  

``` C# : 元からあった例
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

## 特定の月のすべての日付を取得する方法

<https://www.it-mure.jp.net/ja/c%23/c%EF%BC%83%E3%81%A7%E7%89%B9%E5%AE%9A%E3%81%AE%E6%9C%88%E3%81%AE%E3%81%99%E3%81%B9%E3%81%A6%E3%81%AE%E6%97%A5%E4%BB%98%E3%82%92%E5%8F%96%E5%BE%97%E3%81%99%E3%82%8B%E6%96%B9%E6%B3%95/971126391/>

``` C#
public static List<DateTime> GetDates(int year, int month)
{
   return Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
        .Select(day => new DateTime(year, month, day)) // Map each day to a date
        .ToList(); // Load dates into a list
}

// 一応forバージョンも。最も直観的でわかりやすいのでは？
public static List<DateTime> GetDates(int year, int month)
{
   var dates = new List<DateTime>();
   for (var date = new DateTime(year, month, 1); date.Month == month; date = date.AddDays(1))
   {
      dates.Add(date);
   }
   return dates;
}
```

---

## IN と NOT IN

<https://www.kazukiio.com/entry/2019/01/29/000919>

似たようなのに差集合を取得するやつも作ったけど、片方にあるやつを抜き出すだけならINで十分だった。  

``` C#
// テストデータ
var shops = new[]
    {
        new Shop() {ShopId = 1, ShopName = "shop1"},
        new Shop() {ShopId = 2, ShopName = "shop2"},
        new Shop() {ShopId = 3, ShopName = "shop3"},
        new Shop() {ShopId = 4, ShopName = "shop4"},
        new Shop() {ShopId = 5, ShopName = "shop5"},
        new Shop() {ShopId = 6, ShopName = "shop6"},
    };
var inCause = new int[] { 1, 3, 5 };

// IN
var results = shops.Where(x => inCause.Contains(x.ShopId));
// NOT IN
var results = shops.Where(x => !inCause.Contains(x.ShopId));
```

### Contains

[配列やコレクション内に指定された要素があるか調べる](https://dobon.net/vb/dotnet/programing/arraycontains.html)  
LinqのIN,NOT INでしれっとContainsを使っているけど、どういうメソッドなのか、実は知らないのでしらべた。  

というわけで、上のINとNOT INでは、Whereで1レコード引っ張って、それをそのコレクションのContainsで回すって流れるなるわけか。  
で、あればTrueが返却されるので、そのレコードは取得され、それを繰り返していくわけだ。  

``` C#
// 指定した値と同じ要素がコレクション内に存在するかを調べるメソッド
// 存在すればtrueを、しなければfalseを返します。
// 順次検索のため、O(n)操作です。
// IList<T> or LINQ（Enumerable.Contains<TSource>メソッド）のメソッド

//検索元のArrayListを作成する
System.Collections.ArrayList al = new System.Collections.ArrayList();
al.Add("b");
al.Add("aaaaa");
al.Add("cc");

//al内に"cc"があるか調べる
//あるので、"true"を返す
bool b1 = al.Contains("cc");
//al内に"a"があるか調べる
//ないので、"false"を返す
bool b2 = al.Contains("a");
```

---

## 差集合を取得

<https://www.urablog.xyz/entry/2018/07/04/070000>  

``` C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Hello{
    
    private class Parameter
    {
        public int      ID      { get; set; }
        public string   Name    { get; set; }
    }

    public static void Main(){
        // 人物データ
        var dataA = new List<Parameter>()
        {
            new Parameter() { ID = 0, Name = "正一郎" },
            new Parameter() { ID = 5, Name = "清次郎" },
            new Parameter() { ID = 3, Name = "誠三郎" },
            new Parameter() { ID = 9, Name = "征史郎" },
        };
        var dataB = new List<Parameter>()
        {
            new Parameter() { ID = 5, Name = "清次郎" },
            new Parameter() { ID = 3, Name = "誠三郎" },
            new Parameter() { ID = 2, Name = "征史郎" },
        };

         var results  = dataA.Select(s => s.ID).Except(dataB.Select(s => s.ID));
         
        // System.Console.WriteLine( "dataA  :{0}", dataA.Text() );
        // System.Console.WriteLine( "dataB  :{0}", dataB.Text() );
        foreach (var item in results){
            System.Console.WriteLine( "results:{0}", item );    
        }
        
    }
}

// 引いて残った時間の情報だけをパターン明細から抜き出す。
//foreach (var startTime in diffTimeList)
//{
//    // 絶対に時間はあるはずなのでFirst。なかったならおかしいので処理すべきではない。
//    addList.Add(patternDetail.Where(w => w.StartTime == startTime).First());
//}

var huga1 = patternDetail.Select(s => s.StartTime);
var huga2 = reservationFrame.Select(s => s.StartTime);
_ = huga1.Except(huga2);


var diffTimeList = patternDetail
    .Select(s => s.StartTime)
    .Except(reservationFrame.Select(s => s.StartTime));  


//List<TRe_ReservationFramePatternDetail> getAddList()
//{
//    // パターン明細のスタート時間から予約枠のスタート時間を引く。
//    // 引いて残った時間は予約枠に存在しない時間なので追加されるべき時間とみなせる。
//    var diffTimeList = patternDetail
//        .Select(s => s.StartTime)
//        .Except(reservationFrame.Select(s => s.StartTime));
//    return diffTimeList.Count() == 0
//        // 時間の差異がないということは、パターン明細にある以上の時間が設定されているので追加する必要はない
//        ? null
//        // 引いて残った時間の情報だけをパターン明細から抜き出す。
//        : patternDetail.Where(w => diffTimeList.Contains(w.StartTime)).ToList();
//}
//addList = reservationFrame.Count() == 0
//    ? patternDetail.ToList()
//    : getAddList();
//if (addList == null)
//{
//    continue;
//}
```

---

## x.Items!=null && x.Items.Any()のショートカット

<https://stackoverflow.com/questions/28903952/a-shortcut-for-c-sharp-null-and-any-checks/28904185>  

``` C#
// この2つの比較は
if(x.Items!=null && x.Items.Any())
// このように1つにすることができる
if(x.Items?.Any() == true)

// ちなみにnullまたは1件もない場合をはじきたい場合はこう書かないといけない。
// null == false は falseになるので、if文の中に入らないので、nullだったらfalseに変換してあげる。
if(x.Items?.Any() ?? false == false)
// こっちでもよかった。単純なのに奥が深い。
// そりゃそうだよな。nullはtrueでもfalseでもない。
// わざわざnullをfalseにしてfalseと比較するのではなく、trueではないってやればいいだけの話だった。
if(x.Items?.Any() != true)
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

---

## SelectMany

<https://www.urablog.xyz/entry/2018/05/28/070000>  
selectだと配列の中の配列があったときに、`IEnumerable<IEnumerable<array>>`ってなっちゃうから、  
そういう場合はSelectManyってのを使って余計な入れ子をなくそうねってサンプル。  
実際に実務で使うまでは便利さがわからなかったけど、使ってみて完全に理解した。  

``` C#
    // 現在時刻を取得しておく
    var updateDateTime = DateTime.Now;

    // Foreachで頑張った場合
    // こちらの場合、変数を複数用意しないといけないのでまとまりがない。
    // 最も単純でわかりやすいが、野暮ったい感じは否めない。
    var SettlementSettingPlayerViewList = GetSettlementSettingPlayerViewList(data.SettlementSetting);
    List<TFr_Slip> SlipList = new List<TFr_Slip>();
    foreach (var settingPlayer in SettlementSettingPlayerViewList)
    {
        var slipList = _TFr_SlipModel.GetList(
            new PlayerCondition()
            {
                PlayerNo = settingPlayer.SettelementPlayerNo
            }
        );
        foreach (var slip in slipList)
        {
            slip.SettlementPlayerNo = slip.PlayerNo;
            slip.UpdateDateTime = updateDateTime;
            SlipList.Add(slip);
        }
    }

    // SelectManyの場合
    // 変数なんて用意する必要はない。とてもすっきりしてまとまっている。Linq最高。
    // SelectだとIEnumerable<IEnumerable<TFr_Slip>>になるけど、
    // SelectManyすることでIEnumerable<TFr_Slip>にすることができる。平坦化ってやつ。
    var SlipList = GetSettlementSettingPlayerViewList(data.SettlementSetting)
        .SelectMany(
            settingPlayer =>
                _TFr_SlipModel
                    .GetList(
                        new PlayerCondition()
                        {
                            PlayerNo = settingPlayer.SettelementPlayerNo
                        }
                    )
                    .Select(slip =>
                    {
                        slip.SettlementPlayerNo = slip.PlayerNo;
                        slip.UpdateDateTime = updateDateTime;
                        return slip;
                    })
        );
```

---

## LINQを使用して文字列を連結する

[なんか外国のよく見るところ](https://www.it-swarm-ja.com/ja/c%23/linq%E3%82%92%E4%BD%BF%E7%94%A8%E3%81%97%E3%81%A6%E6%96%87%E5%AD%97%E5%88%97%E3%82%92%E9%80%A3%E7%B5%90%E3%81%99%E3%82%8B/958428705/)  
[複数の文字列をセパレータ文字で区切って結合する方法(LINQ編)](https://www.exceedsystem.net/2020/08/29/how-to-join-multiple-strings-with-delimiter/)  

①string.Join  
②Linq.Aggregate  

主にAggregateとString.joinを使う方法があるけれど、String.joinを使うことをおすすめする。  
理由は速度だそうだ。  

``` C#
var data = new[] { "a", "b", "c" };

// string.Join方式
// 出力は「a,b,c」
Console.WriteLine(string.Join(",", data));

// Linq.Aggregate方式
// "a"
// "a" + "," + "b"
// "a,b" + "," + "c"
// のような文字列の結合となるため結合文字数が増えると大きなパフォーマンス低下が生じる。
Console.WriteLine(data.Aggregate((x, y) => $"{x},{y}"));
```

``` C# : おまけ Aggregate+StringBuilder方式
// チェーンしすぎてわけわからんので、自己満足以外で使うことはないだろう。
var res = words.Aggregate(
        new StringBuilder(), 
        (current, next) => current.Append(current.Length == 0? "" : ", ").Append(next)
    )
    .ToString();
Console.WriteLine(res);
```

```C# : 実装例1
// 中々うまくまとめれたのでまとめる。
// 精算済みプレーヤーの警告を出すためメッセージを生成(精算済みプレーヤーのアナウンス)
var warningMessage = _TRe_ReservationPlayerModel
    .GetList(
        new PlayerNoListCondition()
        {
            PlayerNoList = playerNoList,
            ReservationCancelFlag = false,
        }
    )
    .Where(w => w.SettlementFlag == true)
    ?.Select(s => s != null
        ? (!string.IsNullOrEmpty(s.AccountNo) ? "【" + s.AccountNo + "】　" : string.Empty)
            + (!string.IsNullOrEmpty(s.Name) ? "【" + s.Name + "】様" : string.Empty)
            + "は精算済みです。"
        : null
    )
    ?.Aggregate(
        "",
        (a, b) => a + Environment.NewLine + b
    );
```

``` C# : 実装例2
// これを
if (AccountsReceivableList?.Any() == true)
{
    var context = "売掛者";
    foreach (var item in AccountsReceivableList)
    {
        var name = item.AccountsReceivableName ?? string.Empty;
        var len = 60 - Encoding.GetEncoding("shift_jis").GetByteCount(name.ToCharArray());
        context += Environment.NewLine + " " + name.PadRight(len) + string.Format("￥{0:#,0}", item.Amount);
    }
    Data.BasicItem.BusinessReport = context;
}

// こうできた
Data.BasicItem.BusinessReport = AccountsReceivableList?.Any() == true
    ? "売掛者" + AccountsReceivableList
        .Select(s =>
        {
            string name = s.AccountsReceivableName ?? string.Empty;
            int len = 60 - Encoding.GetEncoding("shift_jis").GetByteCount(name.ToCharArray());
            // 名前 空白 \金額 の構成にする
            return $"{ name.PadRight(len)}￥{s.Amount:#,0}";
        })
        ?.Aggregate("", (a, b) => a + Environment.NewLine + b)
    : string.Empty;
```

---

## 配列の全ての要素が同じであるか判定したい

<https://teratail.com/questions/140064>

``` C#
if(rawA.Distinct().Count()==1)
```

---

## 重複を確認したい

[【C#】List中の重複する要素を抽出する方法](https://qiita.com/nkojima/items/c927255b8d621d714f0a)  
[C#, LINQのGroupByで重複した要素と重複した要素が何個あるかを得る](https://hayaup.hatenablog.com/entry/2020/04/16/012501)  

商品台帳の修正中にフロント側で重複のチェックをしたほうがだろうというバグを見つけたので、  
Linqで重複チェックくらいできるだろう、ということで探したのでまとめ。  
Linq万歳。  

``` C#
// GroupByしてWhereとやると、まとめた要素がレコードごとにループされるっぽいので、Countってやると、そのまとめた件数が何件あるか取れる。
// なので、1レコードが2件以上あるということは、どこかしら重複があると見なせるのでAnyで取得する。
if (Data.ProductUnitPriceList
        .GroupBy(g => new { g.PrivilegeTypeCD, g.SeasonCD, g.FeeTypeCD, g.UseTimeZoneCD })
        .Any(a => a.Count() > 1)
)
if (Data.ProductJanList.GroupBy(g => g.JanCD).Any(w => w.Count() > 1))
```

---

## GroupJoinSample

日次帳票のサンプルを掲載。  

``` C# : うまくいった例
    // データを作りながらやる方法。一番おすすめ。
    // 科目データをもとに本日、本月、本年、前年データをLEFT OUTER JOINしてデータをセットする。
    var list = subjectData
        .GroupJoin(
            todaySlipDate,
            subject => new { subject.SubjectSummaryCD, subject.SubjectClsCD, subject.SubjectMidClsCD },
            today => new { today.SubjectSummaryCD, today.SubjectClsCD, today.SubjectMidClsCD },
            (subject, today) => new DailyReportPrintGetPrintResponse()
            {
                SubjectSummaryCD = subject.SubjectSummaryCD,
                SubjectClsCD = subject.SubjectClsCD,
                SubjectClsName = subject.SubjectClsName,
                SubjectCD = subject.SubjectMidClsCD,
                SubjectName = subject.SubjectMidClsName,
                TodayPrice = today.FirstOrDefault()?.Price ?? decimal.Zero
            }
        )
        .GroupJoin(
            monthSlipData,
            basic => new { basic.SubjectSummaryCD, basic.SubjectClsCD, SubjectMidClsCD = basic.SubjectCD },
            month => new { month.SubjectSummaryCD, month.SubjectClsCD, month.SubjectMidClsCD },
            (basic, month) =>
            {
                basic.MonthPrice = month.FirstOrDefault()?.Price ?? decimal.Zero;
                return basic;
            }
        )
        .GroupJoin(
            yearSlipData,
            basic => new { basic.SubjectSummaryCD, basic.SubjectClsCD, SubjectMidClsCD = basic.SubjectCD },
            year => new { year.SubjectSummaryCD, year.SubjectClsCD, year.SubjectMidClsCD },
            (basic, year) =>
            {
                basic.YearPrice = year.FirstOrDefault()?.Price ?? decimal.Zero;
                return basic;
            }
        )
        .GroupJoin(
            lastYearSlipData,
            basic => new { basic.SubjectSummaryCD, basic.SubjectClsCD, SubjectMidClsCD = basic.SubjectCD },
            lastYear => new { lastYear.SubjectSummaryCD, lastYear.SubjectClsCD, lastYear.SubjectMidClsCD },
            (basic, lastYear) =>
            {
                var lastYearPrice = lastYear.FirstOrDefault()?.Price ?? decimal.Zero;
                // 当日伝票は当日分の金額を過去分にも足す。
                // 過去はすでに出来上がったデータを使うので、当日分を足す必要はない。
                if (_ExCon.IsCurrentDay)
                {
                    basic.MonthPrice += basic.TodayPrice;
                    basic.YearPrice += basic.TodayPrice;
                    lastYearPrice += basic.TodayPrice;
                }
                basic.LastYearPrice = lastYearPrice;
                basic.LastYearContrast = basic.YearPrice - lastYearPrice;
                basic.Ratio = lastYearPrice != 0 ? (basic.YearPrice / lastYearPrice).RoundDown(3) : 0;
                return basic;
            }
        )
        .ToList();
```

``` C# : お勧めしないけどできるという例
    // joinする度に必要なフィールドを取り出すパターン
    var list2 = todaySlipDate
        .GroupJoin(
            monthSlipData,
            today => new { today.SubjectSummaryCD, today.SubjectClsCD, today.SubjectMidClsCD },
            month => new { month.SubjectSummaryCD, month.SubjectClsCD, month.SubjectMidClsCD },
            (today, month) => new { today, monthPrice = month.FirstOrDefault()?.Price ?? decimal.Zero }
        )
        .GroupJoin(
            yearSlipData,
            joined1 => new { joined1.today.SubjectSummaryCD, joined1.today.SubjectClsCD, joined1.today.SubjectMidClsCD },
            year => new { year.SubjectSummaryCD, year.SubjectClsCD, year.SubjectMidClsCD },
            (joined1, year) => new { joined1.today, joined1.monthPrice, yearPrice = year.FirstOrDefault()?.Price ?? decimal.Zero }
        )
        .GroupJoin(
            lastYearSlipData,
            joined2 => new { joined2.today.SubjectSummaryCD, joined2.today.SubjectClsCD, joined2.today.SubjectMidClsCD },
            lastYearSlip => new { lastYearSlip.SubjectSummaryCD, lastYearSlip.SubjectClsCD, lastYearSlip.SubjectMidClsCD },
            (joined2, lastYear) =>
            {
                var todayPrice = joined2.today.Price;
                var monthPrice = joined2.monthPrice + todayPrice;
                var yearPrice = joined2.yearPrice + todayPrice;
                var lastYearPrice = (lastYear.FirstOrDefault()?.Price ?? decimal.Zero) + todayPrice;
                return new DailyReportPrintGetPrintResponse()
                {
                    SubjectSummaryCD = joined2.today.SubjectSummaryCD,
                    SubjectClsCD = joined2.today.SubjectClsCD,
                    SubjectClsName = joined2.today.SubjectClsName,
                    SubjectCD = joined2.today.SubjectMidClsCD,
                    SubjectName = joined2.today.SubjectMidClsName,
                    TodayPrice = todayPrice,
                    MonthPrice = monthPrice,
                    YearPrice = yearPrice,
                    LastYearPrice = lastYearPrice,
                    LastYearContrast = yearPrice - lastYearPrice,
                    Ratio = lastYearPrice != 0 ? (yearPrice / lastYearPrice).RoundDown(3) : 0
                };
            }
        );

    // joinしていって、最後に全部取得するパターン
    var list = todaySlipDate
        .GroupJoin(
            monthSlipData,
            today => new { today.SubjectSummaryCD, today.SubjectClsCD, today.SubjectMidClsCD },
            month => new { month.SubjectSummaryCD, month.SubjectClsCD, month.SubjectMidClsCD },
            (today, month) => new { today, monthPrice = month.FirstOrDefault()?.Price ?? decimal.Zero }
        )
        .GroupJoin(
            yearSlipData,
            joined1 => new { joined1.today.SubjectSummaryCD, joined1.today.SubjectClsCD, joined1.today.SubjectMidClsCD },
            year => new { year.SubjectSummaryCD, year.SubjectClsCD, year.SubjectMidClsCD },
            (joined1, year) => new { joined1, yearPrice = year.FirstOrDefault()?.Price ?? decimal.Zero }
        )
        .GroupJoin(
            lastYearSlipData,
            joined2 => new { joined2.joined1.today.SubjectSummaryCD, joined2.joined1.today.SubjectClsCD, joined2.joined1.today.SubjectMidClsCD },
            lastYearSlip => new { lastYearSlip.SubjectSummaryCD, lastYearSlip.SubjectClsCD, lastYearSlip.SubjectMidClsCD },
            (joined2, lastYear) =>
            {
                var todayPrice = today.Price;
                var monthPrice = joined2.joined1.monthPrice;
                var yearPrice = joined2.yearPrice;
                var lastYearPrice = lastYear.FirstOrDefault()?.Price ?? decimal.Zero;
                return new DailyReportPrintGetPrintResponse()
                {
                    SubjectSummaryCD = joined2.joined1.today.SubjectSummaryCD,
                    SubjectClsCD = joined2.joined1.today.SubjectClsCD,
                    SubjectClsName = joined2.joined1.today.SubjectClsName,
                    SubjectCD = joined2.joined1.today.SubjectMidClsCD,
                    SubjectName = joined2.joined1.today.SubjectMidClsName,
                    TodayPrice = todayPrice,
                    MonthPrice = monthPrice,
                    YearPrice = yearPrice,
                    LastYearPrice = lastYearPrice,
                    LastYearContrast = yearPrice - lastYearPrice,
                    Ratio = lastYearPrice != 0 ? (yearPrice / lastYearPrice).RoundDown(3) : 0
                };
            }
        );
```

---

## 指定の要素数で初期化されたListを作成する方法

[C#の配列を同じ値で初期化する](https://www.paveway.info/entry/2019/07/**15_csharp_initarray**)  
Janコードの検索対応で調べたのでまとめ。  
検索処理が複雑だったので、1件の時はいつも通りにするけど、2件以上あったら、適当に2件作って、
フロント側で判断してごにょごにょするって感じで作った。  

``` C#
    // List<SimpleProduct> Count() = 2にする
    Enumerable.Repeat(new SimpleProduct(), 2).ToList();
```

---

## LinqのCast

[【LINQ】Cast<T>は使わない方が良い？OfType<T>を使おう](https://threeshark3.com/castistrap/)  

Castって失敗した時の挙動ってどうなのか知らなかったので調べた。  
→  
失敗すると例外を吐く模様。  

それはそうと、CastはFirstと同じくらい罠らしく、CastではなくOfTypeを使うべきって記事を見つけたのでまとめる。  

OfType<T>
キャストできなかった時 : 要素はフィルタリングされる
要素がnullだった時 : 要素はフィルタリングされる

Cast<T>
キャストできなかった時 : 例外になる
要素がnullだった時 : 例外になる or 要素はフィルタリングされない

OfType<T>は挙動の一貫性があり、予想外の値が来た時も安定してフィルタリングしてくれますね。  
以上の点から、Cast<T>を使う必要はほぼなく、OfType<T>を使うことを強くお勧めします。  

``` C#
    object[] hogeArray = new object[] { 3, "apple", 42 };
    foreach (var item in hogeArray.OfType<int>())
    {
        // キャストできたものだけforeachの中にくるので、
        // 3 と 42 だけが出力される
        Console.WriteLine(item);
    }
    // 2個目の要素を列挙しようとした時に例外発生
    foreach (var item in hogeArray.Cast<int>())
    {
        // 3 はintにキャストできるので出力されるが、
        // "apple"をキャストしようとしたタイミングで例外発生!
        Console.WriteLine(item);
    }
```

``` C# : Wpf.Front.DutchTreat.ViewModels.cs
    // こうやるのと
    if (args?.AddedItems?.Count > 0
        && args.AddedItems.OfType<SubjectLargeTypeWithSubjectCDList>() is IEnumerable<SubjectLargeTypeWithSubjectCDList> addItems)
    {
        SelectedSubjectLargeTypeItemList.AddRange(addItems);
    }
    // こうやるのとだったら、こっちのほうがいいのでは
    foreach (var addItem in args.AddedItems.OfType<SubjectLargeTypeWithSubjectCDList>())
    {
        SelectedSubjectLargeTypeItemList.Add(addItem);
    }
```

---

## 単純にコード一覧が欲しい時にイミディエイトで出力する処理

まぁ、これといったあれはないけど、意外と迷ったのでメモしておく。  
必要なくなったら消してもいいでしょう。  

``` C#
string.Join(",", DutchTreatList.SelectMany(s2 => s2.SlipList.Select(s3 => s3.SubjectCD)).Distinct())
// "20,120,900,131,800,900,20,120,900,110"
```

---

## Take,Skip

[Take, Skip](https://symfoware.blog.fc2.com/blog-entry-1927.html)  

割り勘で、入力した人より下の人を割り勘したかったんですよ。  
つまり、そこより上の人は飛ばして、残った人で割り勘をするので、Linqでなんかないか探したらありました。  

最初はTakeしか見つけられなくて、一番上を飛ばすために、Reserse().Take().Reverse()なんてくそ面倒くさいことをする始末だった。  
そこで見つけたSkip。まさに神だったね。  

Take:指定した数だけ要素をより出します。
Skip:指定した数だけ読み飛ばします。

``` C#
// 単純なTakeの例。
// 先頭から2つの要素を取得する。
int[] src = new int[]{1, 2, 3, 4, 5};
foreach(int item in src.Take(2)) {
    Console.WriteLine(item);
}
// →1,2


// 単純なSkipの例。
// 先頭から2つの要素を読み飛ばす。Skipした以降の全要素が取得できます。
int[] src = new int[]{1, 2, 3, 4, 5};
foreach(int item in src.Skip(2)) {
    Console.WriteLine(item);
}
// →3,4,5


// 実際には値を読み飛ばすSkipと合わせて使用することになると思います。
// 1つ読み飛ばし、先頭から2つの要素を取得。
int[] src = new int[]{1, 2, 3, 4, 5};
foreach(int item in src.Skip(1).Take(2)) {
    Console.WriteLine(item);
}
// →2,3


// TakeしたあとにSkipすることも可能。
// 先頭から2つの要素を取得し、1つ読み飛ばす。
int[] src = new int[]{1, 2, 3, 4, 5};
foreach(int item in src.Take(2).Skip(1)) {
    Console.WriteLine(item);
}
// →2
```

---

## 特定の値を先頭にして、後はそのままの順番にするやり方

[C# LINQで特定の値を先頭にして並び替え](https://teratail.com/questions/120228)  

チェックアウトの割り勘呼び出しで必要になったのでまとめ。  
100,546を割り勘→100をチェックアウトで呼び出す→割り勘起動→546も引っ張られて表示される。  
この時、もう一つ割り勘を開いて、546と入力すると、割り勘で排他を取っているはずなのに、「チェックアウトで精算中です」って言われてしまう。  
原因は100,546の順で排他を取るので、100はチェックアウトで排他中なので、546と打ってもそうなってしまうから。  
というわけで、排他を取る順番を指定した会計Noを先頭にして、後はそのままってやりたかったわけです。  
そしたらドンピシャなモノがありました。  
おかげで無事解決しましたが、後日エラーになって、それも解決したのでまとめます。  

``` C#
    var result = Enumerable
        // 10,11,12,13,14
        .Range(10, 5)
        // A,B,C,D,E
        .Select(a => a.ToString("X"))
        // C,A,B,D,E
        .OrderBy(a => a == "C" ? 0 : 1);
        // サンプルではThenByがあるが、なくても想定した動作になる。
        // .ThenBy(a => a);

    // 成果物
    IEnumerable<SettlementDetailView> dutchTreatExclusive = settlementSet.SettlementDetailList
        .Where(w => outerEditPlayerNoList?.Any() != true || !outerEditPlayerNoList.Contains(w.ReservationPlayerNo))
        ?.OrderBy(a => a.AccountNo == accountNo ? 0 : 1)
        // サンプルの通り、ThenBy(a => a)と書いていたが、それだと「failed to compare two elements in the array」とかいうエラーになってしまう。
        // 単純に消したらうまく行ったし、順番に影響もなかった。
        ?.ToList();
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

``` C# : 失敗作
// 何がしたいかわからないね。
IEnumerable<string> targetPlayerList = SettlementDetailList
    .SelectMany(
        p => p.SlipList,
        (s, slip) => new { s.AccountNo, s.ReservationPlayerName, slip }
    )
    .Where(w => dutchTreatSlipIDList.Contains(w.slip.SlipID))
    .Select(s => $"【{s.AccountNo}】【{s.ReservationPlayerName}】様")
    .ToList();
```

---

## 遅延実行とIQueryable

[LINQの遅延実行&即時実行とforeach+遅延実行の問題【C#】【LINQ】](https://kan-kikuchi.hatenablog.com/entry/LINQ_Delay)  

IQueryableのヒントになるかどうか知らないが、このサイトのある一文がほぼ全てな気がした。  
`whereは呼び出された時点で処理をして、その結果を返しているわけではなく、どういう処理をするかの命令(クエリ)を返している`  
つまり`Whereは配列を作ってるわけではなくクエリを作っている`  

IQueryableは外部実行が何たらかんたらって言ってたから、つまりそういうことなのかなーと思ったり。  
とりあえず、即時実行の動作確認。  

``` C# : 遅延実行サンプル
    var aa = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    var res1 = aa.Where(w => w > 7);
    var res2 = aa.Where(w => w > 7).ToList();
    
    // 8,9
    foreach (var item in res1) Console.WriteLine(item);
    // 8,9
    foreach (var item in res2) Console.WriteLine(item);

    aa.Add(10);
    // 実際に値が必要になった時に処理を実行(遅延実行)
    // 8,9,10 
    foreach (var item in res1) Console.WriteLine(item);
    // 8,9
    foreach (var item in res2) Console.WriteLine(item);
```

[2種類のLINQ](https://csharptan.wordpress.com/2011/12/09/2%E7%A8%AE%E9%A1%9E%E3%81%AElinq/)  
[結局IEnumerable<T>とIQueryable<T>はどう違うの？](https://qiita.com/momotaro98/items/7be27447f5f4a5c8bac9)  
[What is the difference between IQueryable<T> and IEnumerable<T>?](https://stackoverflow.com/questions/252785/what-is-the-difference-between-iqueryablet-and-ienumerablet)  
[IEnumerable vs IQueryable](https://samueleresca.net/the-difference-between-iqueryable-and-ienumerable/)  

``` txt
IQueryable<T>は外部リソースを扱うLinqプロバイダのために提供されているもの。
外部クエリ方式: クエリを投げて、外部のサーバー上で処理してもらって、結果だけ受け取る。
```

なんか頻繁に外部がどうだの言ってるけど、んなわけなくね？  
データベースなんてまったく関係ないでしょ。  
正直、こいつらが何言ってるかわからないんだけど。  

``` txt
IQueryable<T>はLinqメソッド実行時はあくまでクエリが構築されるだけのようです。
foreachなどの評価で初めてクエリが外部ソースに発行され結果が取得されるので、IQueryable<T>は遅延"読み込み"(lazy loading)の特徴があると説明されます。
```

「クエリを構築するだけ。」「問い合わせ可能」って認識程度でいいのかもねぇ。  
本質は「即時実行されない」「評価が必要になった時に実行される」で充分だと思う。  

---

## OrderBy,ThenBy

`list.ThenBy(o => true ? o.ID : o.name);`この構文がエラーになるのだが、分からないかと聞かれたのでまとめ。  
よく考えたら、OrderByって複数のキーを指定できたっけ？みたいなこともあいまいだったので復習も兼ねる。  

「OrderBy 複数」で調べても何も出てこなかった記憶がある。  
改めて調べても出てこないし、よく考えればOrderByって1つのキーで絞らないとソートも何もあったものじゃないよな。  
でもって、またOrderByすると、さっきまでソートした結果を無視して新しくソートしなおしてしまう。  
だからThenByでどんどんソートした結果を保持したままソートする。  

まとめ
・OrderBy,ThenByはキーは1つしか指定できない。(例外はある模様。詳しくは下で)  
・OrderByした結果に対してThenByを行う。  
・ThenByした結果にOrderByをすると、今までの結果は考慮されず一からソートし直す。  

・匿名型は指定不可。  
・タプル、バリュータプルは指定可能。  
・タプルではOrderby,ThenByの順にソートしたのと同じ動作になる模様。  

・タプルでも、内部の型が複数になるとエラーになる。  
・OrderBy中でタプルは.NetFramework4.8(C# 7.3)でも行けた  

## OrderByでタプルや匿名型の動作検証

``` C#
    var list4 = new List<(int id, string name)>
    {
        (1, "A"),
        (2, "B"),
        (3, "C"),
        (4, "D"),
        (5, "E"),
    };
    
    // IDがint,nameがstringの場合、以下のコードはコーディングの段階でエラーになる。
    // 実行する時にならないと型が確定しないからだと思われる。
    // CS0411
    _ = list4.OrderBy(o => true ? o.ID : o.name);
    // これもダメ
    _ = list4.OrderBy(o => true ? (o.id, o.name) : (o.name, o.id));
    // 当然ながら匿名型はだめ。
    var aaa = list4.OrderBy(o => new { o.name, o.id } ).ToList();

    // 素直にOrderBy,ThenByを使う必要がある。
    list4.OrderBy(o => o.id ).ThenBy(o => o.name);
    list4.OrderBy(o => o.name).ThenBy(o => o.id);
```

``` C#
    var list = new List<(int Data1, int Data2)>
    {
        (11, 20),
        (12, 20),
        (10, 22),
        (10, 21),
        (10, 20),
    };

    // 匿名型は不可
    // コーディング段階でエラーにならないが実行するとエラーになる。
    // System.InvalidOperationException: 'Failed to compare two elements in the array.'
    var aa = order_list3.OrderBy(o => new { o.Data1, o.Data2 });
    
    // タプルはいける
    // data1=10, data2=20
    // data1=10, data2=21
    // data1=10, data2=22
    // data1=11, data2=20
    // data1=12, data2=20
    var a2 = order_list3.OrderBy(o => (o.Data1, o.Data2));
    var order_list3 = list.OrderBy(d => d.Data1).ThenBy(d => d.Data2);

    // data1=10, data2=20
    // data1=11, data2=20
    // data1=12, data2=20
    // data1=10, data2=21
    // data1=10, data2=22
    var a2 = order_list3.OrderBy(o => (o.Data2, o.Data1));
    var order_list33 = list.OrderBy(d => d.Data2).ThenBy(d => d.Data1);

    // タプル内の型が全て同じ場合、こういう芸当も可能
    _ = list
        .OrderBy(o => boolean ? (o.Data1, o.Data2) : (o.Data2, o.Data1))
```

``` C#
    var list2 = new List<(int Large, int Middle, int small)>
    {
        (1, 1, 1),(1, 1, 2),(1, 2, 3),(1, 2, 4),(1, 3, 5),(1, 3, 6),
        (2, 1, 7),(2, 1, 8),(2, 2, 9),(2, 2, 10),(2, 3, 11),(2, 3, 12),
        (3, 1, 13),(3, 1, 14),(3, 2, 15),(3, 2, 16),(3, 3, 17),(3, 3, 18),
    };

    // タプルの中でさらに入れ子のタプルを定義し、三項演算子によって分岐させることも可能。  
    var a2222 = list2
        .OrderBy(o => (o.Middle, true ? (o.Large, o.small) : (o.small, o.Large)))
        .ToList();
    // middle → large → small
    // [0]: (1, 1, 1)
    // [1]: (1, 1, 2)
    // [2]: (2, 1, 7)
    // [3]: (2, 1, 8)
    // [4]: (3, 1, 13)
    // [5]: (3, 1, 14)
    // [6]: (1, 2, 3)
    // [7]: (1, 2, 4)
    // [8]: (2, 2, 9)
    // [9]: (2, 2, 10)
    // [10]: (3, 2, 15)
    // [11]: (3, 2, 16)
    // [12]: (1, 3, 5)
    // [13]: (1, 3, 6)
    // [14]: (2, 3, 11)
    // [15]: (2, 3, 12)
    // [16]: (3, 3, 17)
    // [17]: (3, 3, 18)
    
    // Middle → small → Large
    // [0]: (1, 1, 1)
    // [1]: (1, 1, 2)
    // [2]: (2, 1, 7)
    // [3]: (2, 1, 8)
    // [4]: (3, 1, 13)
    // [5]: (3, 1, 14)
    // [6]: (1, 2, 3)
    // [7]: (1, 2, 4)
    // [8]: (2, 2, 9)
    // [9]: (2, 2, 10)
    // [10]: (3, 2, 15)
    // [11]: (3, 2, 16)
    // [12]: (1, 3, 5)
    // [13]: (1, 3, 6)
    // [14]: (2, 3, 11)
    // [15]: (2, 3, 12)
    // [16]: (3, 3, 17)
    // [17]: (3, 3, 18)
```

## OrderByの要素を切り替えたい

OrderByのソート内容を三項演算子で切り替えたい、という要件を実現するのが本命。  
タプルの発見は面白かったが、調べるべきはこれ。  
結果としてできた。  

[型引数を使い方から推論することはできません。型引数を明示的に指定してください。]
ということらしいので、コーディング段階で型推論できないという事であれば、dynamicにして実行時に対応するようにすればいけるのではということでやったらできた。  
dynamicにキャストするだけで済む話だったが、なんでこんなことがどこにも書いてないのだろうか。  

``` C#
    var list= new List<(int id, string name)>
    {
        (1, "E"),
        (2, "D"),
        (3, "C"),
        (4, "B"),
        (5, "A"),
    };
    // 完成形
    var type1 = list.OrderBy(o => false ? (dynamic)o.id : o.name).ToList();
    // タプルでも可能
    var type2 = list.OrderBy(o => false ? (dynamic)(o.id, o.name) : (o.name, o.id)).ToList();

    // 過程1
    Func<(int id, string name), dynamic> sortIdFunc = (p) => { return p.id; };
    Func<(int id, string name), dynamic> sortNameFunc = (p) => { return p.name; };
    var process1 = list.OrderBy(false ? sortIdFunc : sortNameFunc).ToList();
    // https://10.hateblo.jp/entry/2014/05/30/154157
    // このサイトで下の案はいけるってことなので、ならなんでもOKなdynamicにしてそもそもソートされるのか試したらできた。
    // Func<Person,int> f=(p)=>{return p.id;}
    // Func<Person,string> f2=(p)=>{return p.Name;}
    // query = query.OrderBy(model.Order==0?f:f2);

    // 過程2
    // dynamicでいけるなら、後は単純に三項演算子の形に落とし込んでいくだけ。これも問題なかった。
    // 最終的に完成形となる。
    Func<(int id, string name), dynamic> sortFunc = p => (false ? (dynamic)p.id : p.name);
    var process2 = list.OrderBy(sortFunc).ToList();

```

[OrderByのセレクタを外出ししたい](https://10.hateblo.jp/entry/2014/05/30/154157)  
[Create an OrderBy Expression for LINQ/Lambda](https://stackoverflow.com/questions/5766247/create-an-orderby-expression-for-linq-lambda)  

同じ要望を実現したいと考えている人で、一番参考になった。  
紹介されている`Expression<Func<Person,dynamic>>`を素直に実装してみたが、これではダメだった。  
しかし、ここまで近しい資料は他になかったので、この考え方をベースにいろいろやったらいけたという感じである。  

``` C#
    // これがダサい。
    if (model.OrderDirection==0){
        query = model.Order==0? query.OrderBy(p=>p.Name) : query.OrderBy(p=>p.id);
    }else{
        query = model.Order==0? query.OrderByDescending(p=>p.Name) : query.OrderByDescending(p=>p.id);
    }

    // これが通れば、もっといろいろできるはずだったが、受け取る型がどうのこうのというエラーになってしまった。
    Expression<Func<Person,dynamic>> f;
    if (model.Order==0){
        f = p=> new{p.id};
    }else{ 
        f = p=> new{p.Name};
    }
    query = query.OrderBy(f);
```

[Lambda Expressions Are Wonderful](https://www.c-sharpcorner.com/UploadFile/afenster/lambda-expressions-are-wonderful/)  

クラスのメソッドとして定義する案っぽいけど、正直微妙だ。  
やることはないだろう。  

``` C#
    List<Employee> employees = new List<Employee>();
    new Employee().Sort(ref employees, e => e.LastName, SortOrder.Ascending);

    public class Employee
    {
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public decimal Salary { set; get; }
        public bool IsManager { set; get; }

        public void Sort<TKey>(
             ref List<Employee> list,
             Func<Employee, TKey> sortKey,
             SortOrder sortOrder = SortOrder.Ascending)
        {
            if (sortOrder == SortOrder.Ascending)
                list = list.OrderBy(sortKey).ToList();
            else
                list = list.OrderByDescending(sortKey).ToList();
        }
    }
    public enum SortOrder { Ascending, Decending }
```

---

## 連番を指定した数の塊に分解する方法

間隔3と指定した場合、
{
    {0,1,2},
    {3,4,5},
    {6,7,8}
}

間隔5を指定した場合、
{
    {0,1,2,3,4},
    {5,6,7,8,9},
    {10,11,12,13,14},
}

年齢を範囲で区切る処理で必要になったのでまとめ。  
みたいな一連のリストを生成するスマートな処理はないものか探した。

最後に考えたのはForで愚直にやる方法。  
スマートじゃないし、直観的でもないけど、十分動く。でもって速度的には一番早い。  

``` C#
var kankaku = 5;
var from = 0;
// 間隔分インクリメントされる。
for (var to = kankaku - 1; to < 150; to += kankaku)
{
    // ここの間は0,1,2,3,4  5,6,7,8,9 になってる。

    // fromは6,10,16みたいな感じで常にto + 1の値にする。
    from = to + 1;
}
```

探したら使えそうなのがたくさんあった。  

[【C#】LINQ でコレクションをN個ずつの要素に分割する](https://qiita.com/Nossa/items/db9bff2390291432d138)  
[連続する数値でグループ分けする](https://noriok.hatenadiary.jp/entry/2015/06/14/122043)  
[LINQでn個ずつグルーピング](https://ichiroku11.hatenablog.jp/entry/2015/04/16/230309)  
[[C#][VB] LINQでコレクションをチャンク(N個ずつ)に分割](https://webbibouroku.com/Blog/Article/chunk-linq)  

1から100までの連番を生成して、それを指定した要素数の塊にわけて、ループして最小値と最大値を取れば実現できるという寸法。  

``` C#
    // N 個ずつの N
    var chunkSize = 5;
    var chunks = Enumerable.Range(0, 100)
        .Select((v, i) => (v, i))
        .GroupBy(x => x.i / chunkSize)
        .Select(g => g.Select(x => x.v));
    // 動作確認
    foreach (var chunk in chunks)
    {
        foreach (var item in chunk)
            Console.Write($"{item} ");
        Console.WriteLine();
    }

    // fromtoとnullのバージョン
    List<(int, int)?> chunks2 = Enumerable.Range(0, 100)
        .Select((v, i) => (v, i))
        .GroupBy(x => x.i / 5)
        .Select(g => ((int, int)?)(g.Min(m => m.v), g.Max(m => m.v)))
        .ToList();
    chunks2.Insert(0, null);
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

---

## firstordefaultした奴は変更したら元にも反映される

FirstOrDefaultしたものがプリミティブなら、Listは変更されないが、参照型の場合、普通に考えたら変更されるわな。  
だけど、改めて考えてみたらどうなるんだろうってなったのでまとめ。  

---

## GroupByして単純に足したい場合

``` C#
    ConsumptionTaxList = aac
        .GroupBy(g => new { g.TaxationType, g.TaxRate })
        .Select(s =>
        {
            var taxSlip = s.FirstOrDefault();
            taxSlip.TargetPrice = s.Sum(sum => sum.TargetPrice);
            taxSlip.Tax = s.Sum(sum => sum.Tax);
            return taxSlip;
        }).ToList();
```

---

## Union

[要素が重複しないようにして、複数の配列（またはコレクション）をマージする（和集合を取得する）](https://dobon.net/vb/dotnet/programing/arrayunion.html)  

在席リストと空き席リストの予約枠番号を統合して重複を排除した合計が1より大きいか？みたいな判定する時に使ったのでメモ。

``` C#
if (selectedSeatList.Select(s => s.ReservationFrameNo).Union(emptieList.Select(s => s.ReservationFrameNo)).Distinct().Count(w => !string.IsNullOrEmpty(w)) > 1)
```

---

## AddとUnion

LinqのAddはvoidなので、チェーンして書くことができない。  
だけど、Unionを工夫して使うと全部繋げて書けるよっていう例  
速度は保証できないので、完全に好みであるが、備忘録として残しておく  

``` C#
    // 本来のパターン
    // 途中でAddさせたいならいったんチェーンを切らないといけないし、ToList化もしないといけない。
    var framePlayerList = _TRe_ReservationPlayerModel
        .GetList(
            new ReservationFrameCondition()
            {
                ReservationFrameNo = targetPlayer.ReservationFrameNo,
                ReservationCancelFlag = false
            }
        )
        .Where(w => w.PlayerNo != targetPlayer.PlayerNo)
        .ToList();
    // チェックインするプレーヤーを追加する
    framePlayerList.Add(targetPlayer);
    // 今回のチェックインでその枠に存在するプレーヤー全員がチェックイン済みになるなら組確定とする。
    reservationFrame.ConfirmFlag = framePlayerList.All(a => a.CheckinFlag == true);


    // UNIONで疑似的にADDしたパターン
    // 繋げるべきデータをUnion内で作ってしまえば、データの追加が可能というわけ
    reservationFrame.ConfirmFlag = _TRe_ReservationPlayerModel
        .GetList(
            new ReservationFrameCondition()
            {
                ReservationFrameNo = targetPlayer.ReservationFrameNo,
                ReservationCancelFlag = false
            }
        )
        .Where(w => w.PlayerNo != targetPlayer.PlayerNo)
        // そのプレーヤーNoを除外した後、Listを作成してUNIONすることで疑似的なADDが可能というわけ
        .Union(new List<TRe_ReservationPlayer>() { targetPlayer })
        .All(a => a.CheckinFlag == true);


    // ごめん。このサンプルだったらこれで済んだわ。
    // 比較したい要素はフラグだけだから、そのプレーヤーが既に存在するなら、
    // そのフラグだけ最新のプレーヤー情報に書き換えればいいだけだった。
    var framePlayerList = _TRe_ReservationPlayerModel
        .GetList(
            new ReservationFrameCondition()
            {
                ReservationFrameNo = targetPlayer.ReservationFrameNo,
                ReservationCancelFlag = false
            }
        )
        .All(a => {
            if (a.PlayerNo == targetPlayer.PlayerNo) {
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

---

## All

[LINQのAllとAnyが空のシーケンスに対して返す値](https://pdwslmr.netlify.app/posts/language/linq-all-any-empty/)  

シーケンスに条件を満たさない要素が含まれている場合はfalse、それ以外の場合はtrueを返す。  
**空のシーケンス**に条件を満たさない要素は存在しないので**trueを返す**。  

[Allの実装](https://github.com/microsoft/referencesource/blob/4.6.2/System.Core/System/Linq/Enumerable.cs#L1182)

``` C# : Enumerable.cs
public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
    if (source == null) throw Error.ArgumentNull("source");
    if (predicate == null) throw Error.ArgumentNull("predicate");
    foreach (TSource element in source) {
        if (!predicate(element)) return false;
    }
    return true;
}
```

1件も要素がない場合、foreachは実行されないのでtrueになる。  
ではあるのだが、要素がない場合、falseにしてもらいたい。  
Select,Distinct,OrderBy,FirstOrDefaultを組み合わせることで実現可能であることを発見した。  
ヒントはStackOverFlowから得た。  
[Is there a better way of calling LINQ Any + NOT All?](https://stackoverflow.com/questions/29993814/is-there-a-better-way-of-calling-linq-any-not-all)  
やはり外人ニキ。  

``` C# : できたやつ
    // 混合要素の場合
    // 結果: false
    var tupleList = new List<(int Index, bool? flag)>{(1, false),(2, true),(3, null)}
        .Select(s => s.flag == true)
        .Distinct()
        .OrderBy(o => o)
        .FirstOrDefault();

    // 要素が全てtrueの場合
    // 結果: true
    var tupleList = new List<(int Index, bool? flag)>{(1, true),(2, true),(3, true)};

    // 要素なしの場合
    // 結果: false
    var tupleList = new List<(int Index, bool? flag)>();
```

``` C# : 要素あり解説
    var tupleList = new List<(int Index, bool? flag)>
        {
            (1, true),
            (2, false),
            (3, null),
        };
    // ①nullはfalseに置き換わる。
    // 結果: true,false,false
    var v1 = tupleList.Select(a => a.flag == true);
    // ②重複をなくしてtrueとfalseの2つだけにする
    // 結果: true,false
    var v2 = v1.Distinct();
    // ③OrderByするとfalseが先に来る。trueしかない場合、もちろんtrueが先頭になる。
    // 結果: false,true
    var v3 = v2.OrderBy(o => o);
    // ④一番最初を取得すれば結果になる。
    // 結果: false
    var v4 = v3.FirstOrDefault();
```

``` C# : 要素なし解説
    var nullList = new List<(int Index, bool? flag)>();
    // 結果なし
    var v1 = nullList.Select(a => a.flag == true);
    // 結果なし
    var v2 = v1.Distinct();
    // 結果なし
    var v3 = v2.OrderBy(o => o);
    // 結果がない場合のデフォルトはfalseになる
    var v4 = v3.FirstOrDefault();
```

``` C#
    // nullまたはfalseが1件でもあればtrue、を打ち消す。
    var ee = nullList.Any(a => a.flag == null || a.flag == false);
```

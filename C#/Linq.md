# Linqのあれこれ

## Linqのサンプル用タプルリスト

いつもやっているような、whereで要素にアクセスするのを簡単に実現出来ないかやってみた奴。  
2,3個のフィールドのためにわざわざクラスは作りたくない。  
ValueTupeを使えばいつもの感じでフィールド名でアクセスできるので、ちょっとやる分にはこれでいいでしょう。  

``` C#
    // TupleのList
    var tt = new List<(int, string)>
    {
        (3, "first"),
        (6, "second")
    };
    // ValueTupleのListその１
    List<(int example, string descrpt)> list = Enumerable.Range(0, 10)
        .Select(i => (example: i, descrpt: $"{i}"))
        .ToList();
    // ValueTupleのListその2
    var tupleList = new List<(int Index, string Name)>
    {
        (1, "cow"),
        (5, "chickens"),
        (1, "airplane")
    };
```

## C＃で特定の月のすべての日付を取得する方法

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
    var SlipList
        = GetSettlementSettingPlayerViewList(data.SettlementSetting)
        .SelectMany(settingPlayer =>
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

<https://www.it-swarm-ja.com/ja/c%23/linq%E3%82%92%E4%BD%BF%E7%94%A8%E3%81%97%E3%81%A6%E6%96%87%E5%AD%97%E5%88%97%E3%82%92%E9%80%A3%E7%B5%90%E3%81%99%E3%82%8B/958428705/>

``` C#
// Aggregate
string[] words = { "one", "two", "three" };
var res = words.Aggregate(
   "", // start with empty string to handle empty list case.
   (current, next) => current + "," + next);
Console.WriteLine(res);


// Aggregate+StringBuilder方式
var res = words.Aggregate(
         new StringBuilder(), 
         (current, next) => current.Append(current.Length == 0? "" : ", ").Append(next)
     )
     .ToString();
Console.WriteLine(res);

     
// string.Join方式
var sqrts = Enumerable.Range(1, 10).Select(n => Math.Sqrt(n));
//小数点以下3桁で表示
var str = string.Join(", ", sqrts.Select(x => x.ToString("0.000")));
Console.WriteLine(str);

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

どうもAggregateは遅いので、String.Joinを使ったほうがいいみたいですね。
<https://www.exceedsystem.net/2020/08/29/how-to-join-multiple-strings-with-delimiter/>  

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

``` C#
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

// 以下のようにも書けるが、お勧めしない。

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

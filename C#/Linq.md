# Linqのあれこれ

---

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

## 【IN】と【NOT IN】

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
if(x.Items!=null && x.Items.Any())
if(x.Items?.Any() == true)
```

# GroupJoin

LeftOuterJoinを実現するためのメソッド。  
Inner Joinは Joinメソッド。  

---

## 例題とおすすめの方法

データを作りながらやる方法

``` C#
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

---

## お勧めしないけどできるという例

joinする度に必要なフィールドを取り出すパターン

``` C#
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
```

joinしていって、最後に全部取得するパターン

``` C#
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

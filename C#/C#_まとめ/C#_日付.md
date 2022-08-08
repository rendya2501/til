# 日付まとめ

---

## DateTimeで年月日だけ取得する

[日時（DateTimeオブジェクト）の情報を取得する](https://dobon.net/vb/dotnet/system/datetime.html)  

GrapeCityのGcDateのFormatが効かず、時間も入ってきてうまく検索できない問題が発生して、年月日だけ取得する書き方が分からなかったので調べた。  
こういうのって地味にわからない。  

``` C#
//2000年9月30日13時15分30秒を表すDateTimeオブジェクトを作成する
DateTime dt = new DateTime(2000, 9, 30, 13, 15, 30);
//「2000/09/30 0:00:00」のDateTimeオブジェクトを作成する
DateTime dtd = dt.Date;

//nullの場合も対応できる
DateTime? dt = null;
var test = dt?.Date;
```

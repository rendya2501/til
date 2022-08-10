# SQLServer関係メモ

[SQL Serverチートシート](https://qiita.com/esflat/items/7885e53737163eb955fe)  

---

## null許可のフィールドを追加→変更の保存が許可されていません

フィールドを追加する際にたまに発生するので備忘録として残しておく。  
発生原因としてはテーブルの監視履歴が削除されてしまうからだそうです。  

1. [ツール] メニューの [オプション] をクリックします。  
2. [オプション] ウィンドウのナビゲーション ウィンドウ で 、[デザイナー] をクリックします。  
3. [テーブルの再作成を必要とする変更の保存を防止する] チェックボックスをオンまたはオフにし 、[OK] をクリックします。  

[SQL Server カラム定義を変更すると「変更の保存が許可されていません」が表示された場合の対処法](https://nasunoblog.blogspot.com/2013/10/sql-server-column-edit-error.html)  

[変更の保存は、エラー メッセージで許可SSMS](https://docs.microsoft.com/ja-jp/troubleshoot/sql/ssms/error-when-you-save-table)  

---

## テーブルデザイナーにコメント列を追加する方法

あると便利といわれたのでまとめ。  
レジストリをいじらないと駄目です。  

[SSMS のテーブルデザイナの列をカスタマイズした](https://qiita.com/d01tsumath/items/906043d69f86a6a53cef)  
[SQL Server Management Studio のテーブルデザイナの列をカスタマイズする](https://blog.xin9le.net/entry/2018/06/17/165526)  

---

## sqlserver 断片化  

[SQL Serverの断片化したインデックスを再構築する方法](https://www.fenet.jp/dotnet/column/database/sql-server/4365/)  

毎日稼働しているシステムは、日々テーブルにデータがため込まれていきます。  
トランザクションテーブルのように最終行にデータが追加されるテーブルもあれば、マスタテーブルのように行の途中にデータを追加、またはインデックスのキー値を変更するテーブルも存在します。  
テーブルによっては、データの物理的な順序や論理的な順序がバラバラになることがあり、このことを「断片化」と呼びます。  

断片化の状況は、断片化率を見ることで確認可能です  

``` sql
declare @DB_ID int
  ,@OBJECT_ID int
 
set @DB_ID = DB_ID('DB_TEST')
set @OBJECT_ID = OBJECT_ID('USER')
 
select *
from sys.dm_db_index_physical_stats(@DB_ID, @Object_ID, null, null, 'DETAILED') as A
join sys.objects as B on A.object_id = B.object_id
```

---

## SQL Server のRand関数を用いたテストデータの作成

[SQLで乱数を使ってテストデータを作る](https://nonbiri-dotnet.blogspot.com/2017/04/sql.html)

``` SQL
-- とりあえず5回ループして乱数を生成するサンプル
DECLARE @counter smallint
SET @counter = 0

WHILE @counter < 5
BEGIN
    SELECT rand() * 100
    SET @counter = @counter + 1
END
```

---

## FOR XML PATH の TYPE .valueとは何か？

for xml pathは、なんかインテリセンスが働かないけど、`,TYPE).value(,)`なるオプション？が使える模様。  
結論からいうと、型を変換するための命令っぽい。  
別にそこまで厳密に型指定しなくても動く。  
しかし、型の変換が必要な場合もあるのだろう。  

``` SQL
-- このSQLを実行しても上でやったSQLの結果と違うことはない。
-- ABC202007250541,ABC202007250541,ABC202007250541
SELECT STUFF(
    (SELECT TOP 3 ',' + TestID FROM TestTable FOR xml path(''),TYPE).value('.', 'NVARCHAR(MAX)'),
    1,
    1,
    ''
)
```

[Please explain what does "for xml path(''), TYPE) .value('.', 'NVARCHAR(MAX)')" do in this code](https://dba.stackexchange.com/questions/207371/please-explain-what-does-for-xml-path-type-value-nvarcharmax)  

私は人々が時々`, TYPE).value('.', 'NVARCHAR(MAX)')`のテクニックを使うのを省略しているのを見ます。  
これに伴う問題は、一部の文字をXMLエンティティ参照（引用符など）でエスケープする必要があるため、その場合、結果の文字列が期待どおりにならないことです。  
→  
まぁ、やっておいて損はないということだな。  
省略せず書くべし!!  

[SQLServerで複数レコードの文字列を結合](http://icoctech.icoc.co.jp/blog/?p=998)  
FOR XML句で出力した値はXML形式で出力されるため、他のフィールドとは扱いが異なりますので、value()メソッドを使い、通常のSQL型に変換します。  
サブクエリを表す()の後に「.value(‘.’, ‘VARCHAR(MAX)’)」と記述しております。  
value()メソッドの第一引数はXQuery式で、第二引数はSQL型となります。  
第一引数の「'.'」はXQuery式「self::node()」の省略形、  
第二引数では、ユーザー名を扱い、出力文字数の制限をしたくないので、VARCHAR(MAX)としており、  
出力されたXML形式のデータからvalue値を取得するため、  
属性が指定されていようが関係なく、「・アイチャン・ワークン」といったvalue値を取得し、  
第二引数で指定されたVARCHARとして、SQL型の結果を返します。  

[value() メソッド (xml データ型)](https://docs.microsoft.com/ja-jp/sql/t-sql/xml/value-method-xml-data-type?redirectedfrom=MSDN&view=sql-server-ver15)  
意外と奥が深かった。  

---

## FLOOR関数とCEILING関数

Y君から、「これ何したいかわかります？」って質問されたが、そもそも関数が何やるかわからなかったのでまとめる。  

``` SQL
    CEILING(FLOOR(金額 * 10) / 10)
```

- FLOOR : 床→切り下げ  
- CEILING : 天井→切り上げ  

なるほど。言葉の意味そのままなのね。  
因みにROUNDは四捨五入ね。

``` txt : 金額が「112.23」と来た場合
デフォ  →  112.23
*10     →  1122.3
FLOOR   →  1122  
/10     →  112.2 
CEILING →  113   
```

普通に112.23の地点でCEILING使えばよくね？ってなるよね。  
修正の履歴を見る限り、過去の修正を生かしつつ、辻褄を合わせるためにこうなったのだろうという結論で終結した。  

---

## Identityの発番テーブルで保存時に発番した値を取得する

`OUTPUT句`なるものがあるらしい。  
部長が知ってた。  

fkdさんはこんなのも提示した。  
`var id = rnWebConnection.QueryFirstOrDefault<long>("SELECT LAST_INSERT_ID()");`  

``` sql
INSERT INTO IDENTITY_TABLE
(OfficeCD)
OUTPUT
inserted.SeqNo
values('aaa')
```

---

## DISTINCTとワイルドカードの併用

DISTINCTとワイルドカード `*` を併用したら.NETFrameworkでは実行速度が遅くなるらしい  

---

## TOP 句 PERCENT

[SqlServerの SELECT TOP 100 PERCENT で作成したビューの並び順は『保証されない』](https://culage.hatenablog.com/entry/20170824/p1)  
[TOP 句 PERCENT](https://haradago.hatenadiary.org/entry/20180214/p1)  

TOPにPERCENT句を付けると上位N%のデータを取得することができる  

``` sql
select TOP 50 PERCENT * FROM TastTeble

--  100 : 90121件
--   50 : 45061件
--    1 :   902件
-- -100 : '-' 付近に不適切な構文があります。
-- 5000 : パーセント値は 0 から 100 までの値を指定してください。
```

### TOP 100 PERCENTをやる意味とは？

ビューを呼び出したときに最初からソートされているとありがたい。  
ビューを取得してから改めてOrderByする手間は省きたい。  
そういう訳で、それを実現するためのテクニックとして、この方法が用いられてきたみたいだが、Microsoft的には、この動作は意図した動作ではなかったので非推奨になったらしい。  
というわけで、今はほとんど意味がないんだとか。  

実務で使っていたのは、全てのデータを取得するメソッドで並び替えまで済ませ、そのあとで必要なフィールドだけを抽出する動作を実現したかったからだと思われる。  
完全なビューとして運用するわけではないので、この方法が有効だったのかもしれない。  
疑似的なビューとしてこの方法を用いた可能性がある。  

[Road to DBD](https://plaza.rakuten.co.jp/jamshid/diary/200805080000/)  
>Viewやテーブル関数にTOP 100 PERCENTを指定すると、ORDER BYが切れるようになるというのは、SQL Server 2000時代でもある意味Tipsとして通用していたと思うが、SQL Server 2005ではこれが使えなくなったのだった。  
>「TOP 100 PERCENTであれば、全件を返すわけだから、オプティマイザは並べ替えを行わない」という変更が行われた。  

[ビューのソートについて](https://oshiete.goo.ne.jp/qa/4420769.html)  
>ビューを単純に照会したときにORDER句を切らなくても希望する並び順でSELECTできれば、運用上は便利なことが多いです。  
>そのため、SQL Serverでは昔からTipsでTOP 100 PERCENTで並び替える方法が認識されていました。  
>（Tipsとある通り、普通はViewの外でOrder切ります）  
>ただし、SQL Server 2005になって、TOP 100 PERCENTは「並び替えの必要なし」とオプティマイザが判断するように仕様が変更されてしまいました。  
>そのため、SQL Server 2005ではTOP 100 PERCENT句をViewに切っても並び替えは起こりません。  
>それだけなら分かりやすいのですが、やはりこの技を使っていた人が多かったためでしょうか。  
>SP2の後の累積パッチ「SP2の累積プログラムその２」でこれを修正するモジュールが提供されています。  
>SP2には入っていないし、わざわざ当てる人も少ないと思うので、次のSP3が出たとして、それ当てた時からTOP 100 PERCENTが効くようになるでしょう。  
>ちなみにSQL Server 2008でも初期バージョンではTOP 100 PERCENTが効かず、累積パッチが提供されています。  
>ということで、そのようなビューの使い方は正しいアプローチではありませんが、SQL Serverの裏ワザの一つですと認識ください。  

---

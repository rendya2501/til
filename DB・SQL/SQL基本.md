# SQL基本

---

## UNOIN

``` SQL
SELECT 1 AS NUM
UNION
SELECT 2 AS NUM;
```

[union]は出力テーブルの構造が同じでないといけない。  

[union]は重複チェックする。  
[union all]は重複チェックしない。  

なので、速度的にはunion all のほうが早い。  

---

## COUNT

[【SQL】COUNTの使い方（レコード数取得）](https://medium-company.com/sql-count/#:~:text=COUNT%E9%96%A2%E6%95%B0%E3%81%AE%E5%BC%95%E6%95%B0%E3%81%AB%E5%88%97%E5%90%8D%E3%82%92%E6%8C%87%E5%AE%9A%E3%81%99%E3%82%8B,%E3%81%99%E3%82%8B%E3%81%93%E3%81%A8%E3%81%8C%E3%81%A7%E3%81%8D%E3%81%BE%E3%81%99%E3%80%82&text=%E3%80%8CID%3D%221005%22%E3%80%8D,%E7%B5%90%E6%9E%9C%E3%81%8C%E5%8F%96%E5%BE%97%E3%81%A7%E3%81%8D%E3%81%BE%E3%81%99%E3%80%829)  

### COUNT(*) : 件数を取得

COUNT関数の引数に*(アスタリスク)を指定することで、レコード数を取得することができます。

### COUNT(列名）:NULLを除いた件数を取得

COUNT関数の引数に列名を指定することで、指定した列がNULL以外のレコード数を取得することができます。

### COUNT(DISTINCT 列名）: 重複を除いた件数を取得

COUNT関数の引数にDISTINCT 列名を指定することで、重複を除いたレコード数を取得することができます。

### COUNT(*)の意味とNULLのCOUNT

[COUNT(*)　が何を意味しているのかわからない](https://ja.stackoverflow.com/questions/42915/count-%E3%81%8C%E4%BD%95%E3%82%92%E6%84%8F%E5%91%B3%E3%81%97%E3%81%A6%E3%81%84%E3%82%8B%E3%81%AE%E3%81%8B%E3%82%8F%E3%81%8B%E3%82%89%E3%81%AA%E3%81%84)  
→そもそも構文エラーになる。  
**COUNT(*)は行数を数えてくれる**  
COUNT()は、行数を数えて出力する集計関数です。→平成27年秋期 午後問3の解説より  

``` txt
OracleではCOUNT(*)とCOUNT(age)の結果は異なります。
ageにnullが入っているとCOUNT(age)では件数にカウントされません。
グループ化していても同様で、ageがnullのグループのみ0件となります。
COUNT(*)ではageにnullが入っていてもレコードの件数をカウントします。

COUNT(*)ではレコードの内容を取得するため、COUNT('X')やSUM(1)を使った方が高速化できると教わったことがあります。(10年ほど前に聞いたノウハウなので現在も適用されるのかは不明ですが…)
```

なるほど。COUNTはNULLはカウントしないのね。  
動作的にCOUNT(name)見たいにフィールド名を指定したほうが高速化できるっぽいけど、単純にレコード数を取得したいならCOUNT(*)でいいのか。  

### COUNT(*)とCOUNT(カラム名)の違い

基本情報27年春の問題にて遭遇。  
なんだかんだわかってなかったのでまとめ。  

[【SQL】COUNT(*)とCOUNT(カラム名)の違い](https://qiita.com/TomoProg/items/5ba5779b3015ac02f577)  

・COUNT(*)はNULL値かどうかに関係なく、取得された行の数を返す  
・COUNT(カラム名)はNULL値でない値の行数を返す  

``` txt
+----+--------+-------+
| id | name   | price |
+----+--------+-------+
|  1 | apple  |   100 |
|  2 | banana |   120 |
|  3 | grape  |   140 |
|  4 | melon  |  NULL |
|  5 | kiwi   |   120 |
+----+--------+-------+
```

select count(*) from shohin; →そもそも構文エラーになる。 5  
select count(price) from shohin; →そもそも構文エラーになる。 4  

これも基本情報27年春の問題にて遭遇。  

[SQL | COUNT(DISTINCT column_name) は「同じ値の種類数」をカウントする](https://qiita.com/YumaInaura/items/1a1123ed4f33d30d9548)  
初歩だって。トホホ・・・。  
[COUNT句内でDISTINCTを使う／重複を排除したカウント](https://nyoe3.hatenadiary.org/entry/20100313/1268468670)  

つまり、重複行を除いたカウントをしたい場合に有効な構文というわけだ。  
そうなると次はDISTINCTとはどこまで含めることができるのか気になってきたぞ。  

``` txt
+-------+--------+-------+
| name  | sex    | score |
+-------+--------+-------+
| Alice | female |    60 |
| Bob   | male   |    70 |
| Carol | female |    70 |
| David | male   |    80 |
| Eric  | male   |    80 |
+-------+--------+-------+
```

sex には male / famale の二種類がある。  
SELECT COUNT(DISTINCT(sex)) AS sex_kind FROM scores; →そもそも構文エラーになる。 2  

score には 60点 / 70点 / 80点の三種類がある。  
る。 3  

こんな感じでも書ける  
なる。 5,2  

---

## SUM

GROUP BY しなくても合計を求めることができる。  

```sql
select SUM(RoundCount)  
from TRe_ReservationFrame  
where BusinessDate = '20210215'  
```

---

## LIKE句

LIKE句は、指定したパターンと文字列比較を行うための演算子で、次の特殊記号を用いて文字列のパターンを指定します。  

- `_` … 任意の1文字  
- `%` … 0文字以上の任意の文字列  

### 基本情報技術者過去問題 平成31年春期 午後問3

エ : `= '201%'`  

ワイルドカードを使用した文字列は、LIKE句と同時に使用しなければ効果を生じません。  
年度が"201%"の行は存在しませんので結果は0行になります。  
エを選択したけど、との事。  

`_`が任意の1文字だとは知らなかった。  
つまり、普段よく使っている`LIKE '%○○%'`は、どこでもいいから○○があるかどうかを調べているってわけか。  

---

## ANY句

平成27年秋午後のデータベースより。  
ANY句なんて見慣れない句が出てきたのでまとめた。

最初はIN句と何が違うのかわからなかったが、副問い合わせの結果を条件として使うことができることがわかった。  
わかれば以外に便利そう。  
まぁ、使ったことないんだけどね。  

[ANY(SOME)句を用いた副問合せ](https://www.sql-reference.com/select/subquery_any.html)  
例としてANY句を使用して受注テーブルからレコードを抽出します．  

``` txt : 受注テーブル
注文番号,商品コード,受注個数
01-101,A001,100
01-102,A002,200
01-103,B001,300
01-104,B002,400
02-101,A001,150
02-102,A002,350
```

``` SQL
SELECT * FROM 受注
WHERE 受注個数 > ANY (
    SELECT 受注個数
    FROM 受注
    WHERE 商品コード = 'A002' (
)
```

``` txt 結果
注文番号,商品コード,受注個数
01-103,B001,300
01-104,B002,400
02-102,A002,350
```

---

## 相関副問合せ(相関サブクエリ)

<https://atmarkit.itmedia.co.jp/ait/articles/1703/01/news187.html>  
副問合せの文中で、副問合せの外側の属性（検索結果）を利用して検索している問合せを、相関副問合せと呼びます。  
一般的には、パフォーマンスは悪くなる見たい。  

``` SQL
SELECT 社員番号,社員名 FROM 社員 AS S1
WHERE 生年月日 > (SELECT MIN(生年月日) FROM 社員 AS S2 WHERE S1.性別 = S2.性別)
-- サブクエリ中でメインクエリのS1テーブルを参照している。

-- EXISTSを使えば自動的に相関副問い合わせになる。
SELECT * FROM USER_MASTER A
WHERE EXISTS (SELECT * FROM AUTHORIZATION B WHERE A.USER_ID = B.USER_ID )
```

処理の流れ  
（1）外側のSELECT文を1行分だけ実行  
（2）取り出した表を副問合せに代入して実行  
（3）外側のSELECT文における、1行目のWHERE句の判定を行う  

---

## SQLの結合条件のON句の順番について

<https://detail.chiebukuro.yahoo.co.jp/qa/question_detail/q11166323581>  

SQLのLEFT JOIN とかのONの順番は関係無いらしい。  
地味に知らなかったので、メモ。  

---

## CROSS JOIN+WHERE と INNER JOIN

<https://qiita.com/zaburo/items/548b3c40fee68cd1e3b7>  
<https://stackoverflow.com/questions/17759687/cross-join-vs-inner-join-in-sql>  

CROSS JOIN して WHERE で絞る方法(等価結合)とINNER JOINの結果は同じらしい。(厳密には内部処理的には違うらしいが)  
まぁ、CROSS JOINしてWHEREで絞るくらいなら素直にINNER JOIN使えって話。  
RN2.23では結構そういうことしてて、どういう挙動をするのかわからなかったのでやってみた。  

``` SQL
-- どちらも結果は同じになる

-- CROSS JOIN + WHERE
SELECT depts.dept_name,employees.name
FROM depts,employees
WHERE depts.dept_id = employees.dept_id;

-- INNER JOIN
SELECT depts.dept_name,employees.name
FROM depts INNER JOIN employees
WHERE depts.dept_id = employees.dept_id;
```

---

## NULLの扱い

### NULLのLIKE検索

LIKE検索には引っかからない。  
NULLを検索したかったからISNULLを使うこと。  

### NULLのORDERBY

[NULLと戯れる: ORDER BYとNULL](https://qiita.com/SVC34/items/c23341c79325a0a95979)  

どうにも、NULLを最小値とするか最大値とするかは、RDBMS毎に違ったり、設定で変更出来たりするみたい。  
Oracleは最大値扱いだが、SQLServerは最小値扱い見たい。  
まぁ、どちらにせよ、先頭か末尾であることに違いはないということですね。  

### NULLをキャスト

(NULL AS CHAR)→NULLのまま

### NULLとの演算

全てNULLになる  

`NULL -1 = NULL`  

### NULLのSUM

SUMはNULLが1件でも含まれると結果がNULLになる。

---

## insert intoのinto句のありなしの違い

[insert intoのinto句のありなしって違いは何ですか？](https://detail.chiebukuro.yahoo.co.jp/qa/question_detail/q1049319100)  
>MySQL、SQL Server、ACCESS では、 into は省略可能です。  
Oracle では、省略不可です。  

---

## N'文字列'の意味

1. Unicodeを使うという宣言・マーキング  
2. 日本語を使う場合に必須  

[SQLのNプレフィックスっていったい何？](http://once-and-only.com/programing/sql/sql%E3%81%AEn%E3%83%97%E3%83%AC%E3%83%95%E3%82%A3%E3%83%83%E3%82%AF%E3%82%B9%E3%81%A3%E3%81%A6%E3%81%84%E3%81%A3%E3%81%9F%E3%81%84%E4%BD%95%EF%BC%9F/)

>SQL Server で Unicode 文字列定数を扱う場合には、Unicode 文字列の前に大文字の N が必ず必要です。  
>これは、SQL Server Books Online の「Unicode データの使用」で説明されています。  
>”N” プレフィックスは、SQL-92 標準の National Language を意味し、必ず大文字にする必要があります。  
>Unicode 文字列定数の前に N を付加しない場合、その文字列は、SQL Server によって、使用される前に現在のデータベースの Unicode 以外のコードページに変換されます。  

※Unicodeは何故UじゃなくてNなのか？  
→  
National Languageという意味でUnicodeが採用されているため。  
National Languageは国語という意味ではなく「様々な国の文字」というニュアンス。  

[MS SQLServer のSQLで文字列の前にN:](https://oshiete.goo.ne.jp/qa/280266.html)

>N'***' とT-SQL内で書くと、''内の文字をUnicodeで表現されたものとして処理する、という意味になります。  
>Nは、nationalの略です。  
>ですから、日本語を使おうとするとNは必須になる、という事ですね。  
>こんな感じでつかいます。＃N'Unicode 文字列'  

---

## DISTINCTとワイルドカードの併用

DISTINCTとワイルドカード `*` を併用したら.NETFrameworkでは実行速度が遅くなるらしい  

---

## REPLACE

[SQLServerのREPLACE 文字列を置換する](https://sql-oracle.com/sqlserver/?p=195)  

``` sql
--文字列'SATOU'をREPLACEで連結する
--[結果] 'KATOU'
SELECT REPLACE('SATOU','S','K') ;
```

``` sql
--置換対象がない
--[結果] 'SATOU'
SELECT REPLACE('SATOU','Z','Y') ;
```

ここではREPLACEを使って「SATOU」の「Z」を「Y」に置換しようとしました。  
しかし、「SATOU」には「Z」が含まれないので「Y」には置換されませんでした。  
置換する対象がない場合はそのままの文字列が返ってきます。  

---

## NULLを排除した設計

[NULLを排除した設計①](http://onefact.jp/wp/2014/08/26/null%E3%82%92%E6%8E%92%E9%99%A4%E3%81%97%E3%81%9F%E8%A8%AD%E8%A8%88/)  

---

## TOP 句 PERCENT

[SqlServerの SELECT TOP 100 PERCENT で作成したビューの並び順は『保証されない』](https://culage.hatenablog.com/entry/20170824/p1)  
[TOP 句 PERCENT](https://haradago.hatenadiary.org/entry/20180214/p1)  

TOPにPERCENT句を付けると上位N%のデータを取得することができる  

``` sql
select TOP 50 PERCENT * FROM TPa_Reservation

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

## 主キーはなくても動く

主キーを設定することがあまりにも当たり前すぎて、主キーなしという状態が許可されるのか分からなかったので確かめた。  
普通にいけた。  
もちろん重複したデータをいれることができる。  

``` sql
CREATE TABLE [IDENTITY_INSERT_TEST_TBL]  
(
    [Id] INT NOT NULL,  
    [Code] INT NOT NULL,
    [Name] nvarchar(255)
);

-- 重複レコードが余裕で入る
INSERT INTO [IDENTITY_INSERT_TEST_TBL] ([id],[Code],[Name]) 
VALUES(1,1,'A'),(1,2,'B'),(1,3,'C'),(1,1,'A'),(1,2,'B'),(1,3,'C');
```

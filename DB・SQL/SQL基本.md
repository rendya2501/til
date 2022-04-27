# SQL基本

## INSERT

<https://itsakura.com/sql-insert>  

``` sql
-- ●INSERT文 (列名を書かない)  
INSERT INTO テーブル名 VALUES ( '値1' [ , '値2' ]・・・);

-- ●INSERT文 (列名を書く)
INSERT INTO テーブル名 ( テーブルの列名1 [ , テーブルの列名2 ]・・・) VALUES ( '値1' [ , '値2' ]・・・);

-- ●INSERT文 (selectの結果をinsertする)
INSERT INTO テーブル名 SELECT 項目名 FROM テーブル名

-- ●INSERT文 (selectの結果+列名の指定)
INSERT INTO テーブル名 ( [テーブルの列名1], [テーブルの列名2]... ) SELECT [項目名1],[項目名2]... FROM 別テーブル名

-- SELECTした結果をINSERTできたよなぁーと思ってやったら一発で行けたので備忘録として載せておく。
INSERT INTO Round3SysC3.dbo.TSm_ReportFileSetting
(WindowName,TemplateName,ReportName,ValidFlag,Sort,ApiUri,Remarks)
SELECT WindowName,TemplateName,ReportName,ValidFlag,Sort,ApiUri,Remarks
FROM Round3Sys_Test.dbo.TSm_ReportFileSetting
WHERE WindowName = 'RN3.Wpf.Front.CheckOut.Views.CheckOutWindow'
AND TemplateName = 'RN3.Wpf.Front.CheckOut.SettlementH.rdlx'
```

疑問

### 列名を書くタイプのINSERT文で別名を定義して

`INSERT INTO テーブル名 ([列1],[列2]) VALUES ('値1' AS [列2],‘値2’ AS [列1])`
ってやった場合どうなる？  
→そもそも構文エラーになる。

`INSERT INTO TMa_Route2 (OfficeCD,RouteCD) VALUES ('ALP' AS [AAA],100 AS [BBB])`
→
`メッセージ 156、レベル 15、状態 1、行 5 キーワード 'AS' 付近に不適切な構文があります。`

### 列名を書くタイプのINSERTは列の数が一致しないとエラーになる

`INSERT INTO TMa_Route2 VALUES ('ALP',102)`
→
`メッセージ 213、レベル 16、状態 1、行 5 列名または指定された値の数がテーブルの定義と一致しません。`

### SELECTを用いたタイプで別名を定義した場合も同義

→こっちは行ける。でもって別名関係なく、順番通りに入る。  

・定義していない列はデフォルト値が入る？  
→そう  

`INSERT INTO TMa_Route2 (OfficeCD,RouteCD) SELECT 'ALP' AS [AAA],100 AS [BBB]`
→

``` txt
OfficeCD    RouteCD    RouteName    RouteShortName    ClsCD    Color    BackgroundColor    Sort    ValidFlag    SearchKey    InsertDateTime    InsertStaffCD    InsertStaffName    InsertProgram    InsertTerminal    UpdateDateTime    UpdateStaffCD    UpdateStaffName    UpdateProgram    UpdateTerminal
ALP    100    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL
```

### 値を設定するタイプは設定した列の数があっていればおｋ?

→そうみたい。

`INSERT INTO TMa_Route2 (OfficeCD,RouteCD) VALUES ('ALP',100,22)`  
→

`メッセージ 110、レベル 15、状態 1、行 5`  
`VALUES 句で指定された値よりも INSERT ステートメントの列数が少なすぎます。VALUES 句の値の数は、INSERT ステートメントで指定される列数と一致させてください。`  

---

## UPDATE

<https://qiita.com/ryota_i/items/d17c7630bacb36d26864>  

特定テーブルにおける、条件に当てはまるレコードの特定のカラムの値を任意の値に書き換える。  
日本語に直して予約語さえ覚えてしまえば、なんてことはないな。  

```sql
-- ●基本
UPDATE テーブル名
SET 列名1 = 値1 [,列名2 = 値2]・・・
WHERE (条件);


-- ●UPDATE文のset句で副問合せを使用する
UPDATE syain
SET name = (
    select name
    from test
    where id = 2)
WHERE id = 2;


-- ●UPDATE文のwhere句で副問合せを使用する(where in)
-- https://sqlazure.jp/r/sql-server/403/
UPDATE syain
SET name = 'テスト'
WHERE id in (
    select id from test
    where name = 'TEST'
);

-- ●inner join 1
-- UPDATE SET FROM JOIN WHERE の流れを意識すれば、まぁ何とか鳴るでしょう。  
UPDATE
    [Table]
SET
    [Table].col1 = [other_table].col1,
    [Table].col2 = [other_table].col2
FROM
    [Table]
    INNER JOIN
        [other_table]
    ON
        [Table].id = [other_table].id


-- ●inner join 2
UPDATE
    テーブル名1 
    INNER JOIN テーブル名2 
    ON テーブル名1.列名X = テーブル名2.列名X
SET
    テーブル名1.列名1 = テーブル名2.列名2;
-- 下記でも同じ
UPDATE
    テーブル名1,テーブル名2
SET
    テーブル名1.列名1 = テーブル名2.列名2
WHERE
    テーブル名1.列名X = テーブル名2.列名X;


-- ●UPDATE文でCASE式を使用する
UPDATE
    syain
SET
    name = case id
        when 1 then '鈴木一郎'
        when 2 then '田中二郎'
        when 3 then '佐藤三郎'
    end,
    romaji = case
        when id > 1 then 'tokumei'
        ELSE 'i.suzuki'
    end
WHERE
    id in(1, 2, 3)
```

---

## DELETE文

```sql
-- ●基本
DELETE FROM テーブル名 WHERE (条件);


-- ●join
-- https://stackoverflow.com/questions/16481379/how-can-i-delete-using-inner-join-with-sql-server
-- DELETE FROM JOIN WHEREの流れは基本的に同じ見たい。それさえ分かれば後は十分だろう。
-- joinの場合、削除するテーブルを指定する必要があるので、DELETEの後ろにエイリアスを指定する(テーブル名ではダメ見たい)。
DELETE w
FROM WorkRecord2 w
INNER JOIN Employee e
ON EmployeeRun=EmployeeNo
WHERE Company = '1' AND Date = '2013-05-06'


-- ●複数テーブルの削除
-- https://hamalabo.net/mysql-multi-delete
-- 通常の場合
DELETE FROM M_UserData WHERE UserId = 1;
DELETE FROM T_TimeCard WHERE UserId = 1;

-- 複数テーブルの場合
DELETE User,Time
FROM M_UserData As User
LEFT JOIN T_TimeCard AS Time
ON User.UserId = Time.UserId
WHERE User.UserId = 1
```

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

<https://medium-company.com/sql-count/#:~:text=COUNT%E9%96%A2%E6%95%B0%E3%81%AE%E5%BC%95%E6%95%B0%E3%81%AB%E5%88%97%E5%90%8D%E3%82%92%E6%8C%87%E5%AE%9A%E3%81%99%E3%82%8B,%E3%81%99%E3%82%8B%E3%81%93%E3%81%A8%E3%81%8C%E3%81%A7%E3%81%8D%E3%81%BE%E3%81%99%E3%80%82&text=%E3%80%8CID%3D%221005%22%E3%80%8D,%E7%B5%90%E6%9E%9C%E3%81%8C%E5%8F%96%E5%BE%97%E3%81%A7%E3%81%8D%E3%81%BE%E3%81%99%E3%80%82>

### COUNT(*) : 件数を取得

COUNT関数の引数に*(アスタリスク)を指定することで、レコード数を取得することができます。

### COUNT(列名）:NULLを除いた件数を取得

COUNT関数の引数に列名を指定することで、指定した列がNULL以外のレコード数を取得することができます。

### COUNT(DISTINCT 列名）: 重複を除いた件数を取得

COUNT関数の引数にDISTINCT 列名を指定することで、重複を除いたレコード数を取得することができます。

## COUNT(*)の意味とNULLのCOUNT

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

## COUNT(*)とCOUNT(カラム名)の違い

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

## LIKE句

基本情報技術者過去問題 平成31年春期 午後問3より。  

LIKE句は、指定したパターンと文字列比較を行うための演算子で、次の特殊記号を用いて文字列のパターンを指定します。  
・`_` … 任意の1文字  
・`%` … 0文字以上の任意の文字列  

`_`が任意の1文字だとは知らなかった。  
つまり、普段よく使っている`LIKE '%○○%'`は、どこでもいいから○○があるかどうかを調べているってわけか。  

エ : `= '201%'`  
を選択したけど、ワイルドカードを使用した文字列は、LIKE句と同時に使用しなければ効果を生じません。年度が"201%"の行は存在しませんので結果は0行になります。  
との事。  

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
NULL -1 = NULL

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

[NULLを排除した設計①](http://onefact.jp/wp/2014/08/26/null%E3%82%92%E6%8E%92%E9%99%A4%E3%81%97%E3%81%9F%E8%A8%AD%E8%A8%88/)  

# SQLメモ

---

## ROW_NUMBER

連番を割り振る関数。  
連番の順番と区分ごとに連番を生成できるらしい。  

[SQL Fiddle](http://sqlfiddle.com/#!18/7374f/71)  
[よく使われる順位付け関数 1 - ROW_NUMBER](https://sql55.com/t-sql/sql-server-built-in-ranking-function-1.php)  
<https://style.potepan.com/articles/23566.html>  

``` SQL : 基本
ROW_NUMBER () OVER ( [ PARTITION BY [パティションカラム 1 ], [パティションカラム 2], ... ] ORDER BY [ソートカラム 1], [ソートカラム 2], ...  )
```

``` SQL
-- こんな感じで使うらしいよ。
select ROW_NUMBER() OVER (PARTITION BY Code ORDER BY UpdateDateTime DESC), * from [Table]

-- 関数 'ROW_NUMBER' には ORDER BY 句を伴う OVER 句が必要です。
-- こうやって怒られるので、最低限、ORDER BY は必要見たい。
select ROW_NUMBER() OVER (), * from [Table]
```

---

## PARTITION BY

[SQL PARTITION BYの基本と効率的に集計する便利な方法](https://zukucode.com/2017/08/sql-over-partition-by.html)  

``` txt : employee（社員）
id  first_name  last_name  department_id  height
1   一郎         山田       1              170
2   次郎         佐藤       2              175
3   三郎         田中       1              185
4   四郎         鈴木       2              155
```

``` sql
SELECT
  last_name,
  --全体の総件数
  COUNT(1) OVER() total_count,
  --部門ごとの件数
  COUNT(1) OVER(PARTITION BY department_id) section_count,
  --部門ごとの最大身長
  MAX(height) OVER(PARTITION BY department_id) section_max_height,
  --部門ごとの身長順（身長順に並び替えたときの行番号）
  ROW_NUMBER() OVER(PARTITION BY department_id ORDER BY height DESC) section_height_order,
  --全体の身長順（身長順に並び替えたときの行番号）
  ROW_NUMBER() OVER(ORDER BY height DESC) height_order
FROM
  employee
ORDER BY
  id
```

``` txt : 取得結果
last_name  total_count  section_count  section_max_height  section_height_order  height_order
山田        4            2              185                 2                     3
佐藤        4            2              175                 1                     2
田中        4            2              185                 1                     1
鈴木        4            2              175                 2                     4
```

---

## 分子分母を出力するサンプル

プレーヤーが複数の時間に跨いで予約を取っている状況において、その人にとってその時間が何番目なのかを分母分子で表示するサンプル  

``` sql : データ準備
CREATE TABLE TestReservation
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,  
    [ReservationTime] time,  
    [PlayerNo] nvarchar(255)
);

INSERT INTO TestReservation
VALUES
     ('07:00:00.0000000','Player202204160001')
    ,('07:07:00.0000000','Player202204160001')
    ,('07:14:00.0000000','Player202204160001')
    ,('07:00:00.0000000','Player202204160002')
    ,('07:07:00.0000000','Player202204160002')
    ,('07:00:00.0000000','Player202204160003')
    ,('07:00:00.0000000','Player202204160004')
    ,('07:14:00.0000000','Player202204160004')
```

``` sql
SELECT
    [ReservationTime],
    [PlayerNo],
    -- プレーヤー毎の時間順(分子)
    ROW_NUMBER() OVER(PARTITION BY [PlayerNo] ORDER BY [ReservationTime]) AS [Numerator],
    -- プレーヤー毎の件数(分母)
    COUNT(1) OVER(PARTITION BY [PlayerNo]) AS [Denominator]
FROM [TestReservation]

-- 07:00:00.0000000    Player202204160001    1    3
-- 07:07:00.0000000    Player202204160001    2    3
-- 07:14:00.0000000    Player202204160001    3    3
-- 07:00:00.0000000    Player202204160002    1    2
-- 07:07:00.0000000    Player202204160002    2    2
-- 07:00:00.0000000    Player202204160003    1    1
-- 07:00:00.0000000    Player202204160004    1    2
-- 07:14:00.0000000    Player202204160004    2    2
```

``` sql : 超愚直にやるならこう
SELECT
    [SQ_Numerator].[ReservationTime],
    [SQ_Numerator].[PlayerNo],
    CONVERT(nvarchar,[SQ_Numerator].[Numerator]) AS [Numerator],
    CONVERT(nvarchar,[SQ_Denominator].[Denominator]) AS [Denominator]
FROM (
    -- 時間を上から見ていった時のシーケンス(分子:Numerator)
    SELECT
        [ReservationTime],
        [PlayerNo],
        ROW_NUMBER() OVER(PARTITION BY [PlayerNo] ORDER BY [ReservationTime]) AS Numerator
    FROM 
        [TestReservation]
    GROUP BY
        [ReservationTime],[PlayerNo]
    ) AS [SQ_Numerator]
JOIN (
    -- プレーヤーが持っている時間の数(分母:Denominator)
    SELECT
        [PlayerNo],
        COUNT(DISTINCT [ReservationTime]) AS [Denominator]
    FROM 
        [TestReservation]
    GROUP BY 
        [PlayerNo]
) AS [SQ_Denominator]
ON [SQ_Numerator].[PlayerNo] = [SQ_Denominator].[PlayerNo]
```

---

## GROUP BY した結果の1件目を取得する方法

子テーブルから親テーブルを作る必要があるときに、コードで絞った結果の1件目だけを親テーブルにINSERTしたい状況に遭遇した時のまとめ。  
「group by 先頭1件」で検索。  
Rank() Over構文を使うらしい。  

[SELECT 文　GROUP　での1件目を取得](https://oshiete.goo.ne.jp/qa/3819843.html)  

``` sql
CREATE TABLE [Test]
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,  
    [Code] INT,  
    [Name] nvarchar(255)
);

INSERT INTO [Test]
VALUES
     ( 1 ,'A')
    ,( 1 ,'B')
    ,( 1 ,'C')
    ,( 1 ,'D')
    ,( 2 ,'F')
    ,( 2 ,'G')
    ,( 2 ,'H')
    ,( 3 ,'X')
    ,( 3 ,'Y')
    ,( 3 ,'Z')
```

``` sql
SELECT
    [X].[Code],
    [X].[Name]
FROM
(
    SELECT
        RANK() OVER(PARTITION BY [Code] ORDER BY [Name]) AS rk,
        [Code],
        [Name]
    FROM [Test]
) AS [X]
WHERE rk=1

--1 A
--2 F
--3 X
```

---

## DROP TABLEはロールバック可能か？

SQLServerではロールバック可能。  
他はDBによる。  

「クエリを2回流したらデータが消えた」みたいなトラブルがあったので、自分がいつも作る列追加のクエリは大丈夫なのか調べたのがきっかけ。  
そもそもDROP TABLEはロールバックが可能なのかと。  

DBによることが判明した。  
SQLServerではロールバック可能。
ロールバック不可能なDBに関してはdrop tableやtruncate等のDDL(テーブル構造)命令は、RollBackが利かない模様。  
なので、自分がいつも作るクエリは大丈夫なことが分かった。  

[DELETE文 TRUNCATE文 DROP文の違い(SQL構文)](https://www.earthlink.co.jp/engineerblog/technology-engineerblog/2680/)  

### DELETE文

表内のデータを(全)削除する。  

`DELETE FROM (表名);`  

- 語訳は「削除」  
- DML(データ操作言語)  
- COMMITしていなければロールバック可能です。  

### TRUNCATE文

表内のデータを全削除する。

`TRUNCATE TABLE (表名);`  

- 語訳は「切り取る」  
- DDL(データ定義言語)  
- TRUNCATE文はWHERE句で指定できませんのでテーブルのデータを全て削除する。  
- テーブルごと削除してから再作成するのでDELETE文よりも高速。  
- トランザクションが効かない。  
- ロールバックができない。  

### DROP文

表内のオブジェクトを完全に削除する。  

`DROP TABLE (表名);`  

- 語訳は「捨てる」  
- DDL(データ定義言語)  
- 完全に削除するのでロールバックができません。表構造も残りません。  
- DROP文はオブジェクトに対するSQL文なのでTABLEを変えてあげれば索引なども削除できる。  

---

## DDLのトランザクション

[DDLのトランザクション(PostgresSQL,Oracle,MySQL)](https://tamata78.hatenablog.com/entry/2017/02/20/112026)  
[SqlServerではDDL(create文等)をロールバックすることが出来る](https://culage.hatenablog.com/entry/20110129/p6)  
[SQL Server でDDLがRollbackできる？](https://www.ilovex.co.jp/Division/ITD/archives/2005/05/sql_server_ddlr.html)  

### PostgreSQL

CREATE TABLEやALTER TABLEなどのDDL命令もCOMMIT、ROLLBACKの対象になる。  

PostgreSQLでは、CREATE TABLE や DROP TABLE などの DDL もトランザクションの一部となるため、トランザクションの途中でDROP TABLE を実行した場合でも、最後に ROLLBACK すれば、DROP したテーブルが元に戻ります。  

### Oracle

DDLはトランザクション対象にはならない。暗黙コミットされる。

### MySQL

DDLはトランザクション対象にはならない。暗黙コミットされる。

### SqlServer

DDL(create文等)はロールバックすることが出来る。  
SQL Server ではTransaction 管理下ではRollback が可能なようです。

---

## 暗黙的なコミット

MySQLでの話にはなるが、概要として理解するには十分なのでそのまま引用する。  

MySQLの暗黙的なコミットは、特定のクエリを実行した際に現在のセッションで実行されているトランザクションを全てコミットしてから実行されるクエリで、クエリ自身の実行後もコミットされます。  

DROP TABLEを行ったクエリの順番で考えていきます。

``` sql
mysql> START TRANSACTION;
mysql> DROP TABLE zipcode;
mysql> ROLLBACK;
```

上記のクエリに、先ほど説明した内容の暗黙的なコミットを明示的に入れ込んでみると、以下のようになります。

``` sql
mysql> START TRANSACTION;
mysql> COMMIT; -- 処理の前に自動的にコミットされる
mysql> DROP TABLE zipcode;
mysql> COMMIT; -- 処理の後に自動的にコミットされる
mysql> ROLLBACK;
```

ということで、ROLLBACKを打ったとしても結果が全てコミットされてしまっているため、元に戻せないことがわかります。

よくある悲劇的な話としては、テーブル内のデータの削除の高速化のためにDELETE文で削除していたものを、TRUNCATE TABLEに変更した時などに起こります。  
トランザクション処理の途中で単純に置き換えをしてしまった場合に、暗黙のコミットが挟まってしまって予期せぬ挙動になってしまうことがあります。  

[DDLと暗黙的なコミットについて](https://gihyo.jp/dev/serial/01/mysql-road-construction-news/0134)  

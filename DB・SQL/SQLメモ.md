# SQLメモ

---

## DROP TABLEはロールバック可能か？

不可能。  
SQLServerはDDLをロールバック可能ということで、いけるかと思ったが、調べてみたら普通に駄目だった。  
SQLServerで無理なのだから他のDBでも基本的に無理なのだろう。  

ただ、SQLSERVER truncateまではロールバックできるので、なんか色々ある模様。  

``` sql
Begin Tran
Select * From A --- 1
drop Table A
Select * From A --- 2

Rollback Tran
Select * From A --- 3
```

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

---

## 検索条件にマッチするデータが存在しない場合にデフォルト値を返す

レコードが存在したらそのままINSERTして、レコードがなければデフォルト値をINSERTしたい。  

単純そうな課題だったが、30分くらい調べたのでまとめる。  

結果としてCOALESCEで実現できた。  
COALESCEはNULLの場合の置換だと思っていたが、レコードがなくても動いてくれる模様。  

sql 条件によってselectを変える
SQL 0件の場合
COALESCE　レコードなし
[検索条件にマッチするデータが存在しない場合にデフォルト値を返す](https://sebenkyo.com/2020/04/14/post-774/)
[NULLと戯れる: 集約関数とNULL](https://qiita.com/SVC34/items/dc1bc52c2d7b44a65459)
[レコードがない場合のselect結果](https://teratail.com/questions/57768)  

``` sql : データ用意
DROP TABLE IF EXISTS TestTable
CREATE TABLE TestTable (ID INT PRIMARY KEY,Hoge varchar(50))
INSERT INTO TestTable VALUES(1,'AAA'),(2,'DDD')
```

``` sql : 案1
SELECT COALESCE(
    (SELECT Hoge FROM TestTable WHERE ID = 1),
    'XXX'
)
-- 結果
--(列名なし)
--XXX
```

MAXは0件ならNULLを返す。  
COALESCEは結果がNULLなら指定した値に置き換えるので本来の動作に準拠したものになる。  
なのでこちらのほうがいいのかも？  

``` sql : 案2
SELECT COALESCE(
    (SELECT MAX(Hoge) FROM TestTable WHERE ID = 9),
    9999
)
```

存在しないIDの場合レコードはない

``` sql
SELECT Hoge FROM TestTable WHERE ID = 9
-- 結果
-- (列名なし)
```

CASE WHENを使って実現しようとしたが、駄目だったのでそれもまとめる。  

``` sql : 試行錯誤
SELECT 
    CASE WHEN EXISTS(SELECT Hoge FROM TestTable WHERE ID = 9)
        THEN Hoge
        ELSE 9999
    END
FROM TestTable
WHERE ID = 9

SELECT 
    CASE WHEN COUNT(*) = 0
        THEN Hoge
        ELSE 'XXX' 
    END AS 料金CD
FROM TestTable
WHERE ID = 9

SELECT
    CASE WHEN (CASE WHEN ID = 9 THEN 1 ELSE 0 END) = 0
        THEN Hoge
        ELSE 'XXX'
    END    
FROM TestTable
-- AAA
-- DDD
```

---

## MAX関数 + CASE式

単純にMAXを取りたいけど、極端に大きな値が混ざってしまっているために、意図しないMAXを取得してしまう。  
これを解決するためにアレコレ考えたが、MAXでCASE文を使えることを知り、極端な値以下でMAXを取得できないかやってみたら出来たのでまとめる。  

``` sql : テストデータ準備
DROP TABLE IF EXISTS TestTable
CREATE TABLE TestTable(BusinessDate DATETIME,HogeNumber INT)
INSERT INTO TestTable
VALUES
    ('2022-07-19',1),
    ('2022-07-19',2),
    ('2022-07-20',1),
    ('2022-07-20',2),
    ('2022-07-20',3),
    ('2022-07-20',4),
    ('2022-07-20',99998),
    ('2022-07-21',1),
    ('2022-07-21',2),
    ('2022-07-21',3),
    ('2022-07-21',99998),
    ('2022-07-22',99998)
```

``` sql
SELECT 
    BusinessDate,
    MAX(HogeNumber),
    MAX(CASE WHEN HogeNumber < 99998 THEN HogeNumber ELSE 0 END)
FROM TestTable
GROUP BY BusinessDate
ORDER BY BusinessDate DESC

-- BusinessDate             単純MAX    CASE WHEN MAX
-- 2022-07-22 00:00:00.000    99998    0
-- 2022-07-21 00:00:00.000    99998    3
-- 2022-07-20 00:00:00.000    99998    4
-- 2022-07-19 00:00:00.000        2    2
```

因みにこれが分からなかったときに愚直にやった方法は以下の通り。  
単純にWHEREで99998以下にしてしまうと、その日付のレコードが出力されないので、自分自身の日付をDISTINCTして、それに対してLEFT JOINして、サブクエリでWHEREしてMAXして、ないものに関しては0にするという方法。  

``` sql
SELECT 
    [A].[BusinessDate],
    ISNULL([B].[HogeNumber],0)
FROM
    (SELECT DISTINCT [BusinessDate] FROM [TestTable]) AS [A]
    LEFT JOIN (
        SELECT [BusinessDate], MAX([HogeNumber]) AS [HogeNumber] 
        FROM [TestTable] 
        WHERE [HogeNumber] < 99998
        GROUP BY [BusinessDate]
    ) AS [B]
    ON [A].[BusinessDate] = [B].[BusinessDate]
ORDER BY BusinessDate DESC

-- BusinessDate
-- 2022-07-22 00:00:00.000    0
-- 2022-07-21 00:00:00.000    3
-- 2022-07-20 00:00:00.000    4
-- 2022-07-19 00:00:00.000    2
```

[集約関数にCASE式で条件をつける](https://qiita.com/yatto5/items/8c9b8ca6b01d83bd95bc)  

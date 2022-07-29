# SQLメモ

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

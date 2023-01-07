# MERGE(UPSERT)

## 概要

データが存在する場合にはUPDATE、存在しない場合にはINSERT処理が行えるステートメント。  
SQLServer2008からサポートされた機能。  
MERGEでは文末のセミコロンが必須。  

``` sql
MERGE INTO マージ先のテーブル
USING マージ元のテーブルまたはクエリ
ON マージの条件
WHEN MATCHED THEN
    UPDATE SET 更新
WHEN NOT MATCHED THEN
    INSERT VALUES (追加);
```

---

## 例

``` txt
[t1]Table
| a |  b  |
|---+-----|
|1  | AAA |               [t1]Table
|2  | BBB |               | a |  b  |
|3  | CCC |               |---+-----|
|4  | DDD |               |1  | AAA |
             => MERGE =>  |2  | BBB |
[t2]Table                 |3  | XXX | ←UPDATE DATA
| a |  b  |               |4  | DDD |
|---+-----|               |5  | YYY | ←INSERT DATA
|3  | XXX |
|5  | YYY |
```

``` sql
-- t1 テーブルの作成
CREATE TABLE t1
(
    a int,
    b varchar(100)
)
INSERT INTO t1
VALUES
    ( 1, 'AAA' ),
    ( 2, 'BBB' ),
    ( 3, 'CCC' ),
    ( 4, 'DDD' )

-- t2 テーブルの作成
CREATE TABLE t2
(
    a int,
    b varchar(100)
)
INSERT INTO t2
VALUES
    ( 3, 'XXX' ),
    ( 5, 'YYY' )

-- MERGE
MERGE INTO t1
USING t2
ON t1.a = t2.a
WHEN MATCHED THEN
    UPDATE SET t1.b = t2.b
WHEN NOT MATCHED THEN
    INSERT VALUES ( t2.a, t2.b );
```

<http://www.sqlquality.com/books/dev03/11-1.txt>  

---

## 一括インポート時にMERGEを利用

MERGEステートメントでは、OPENROWSET(BULK …)を利用してテキストファイルを一括インポートするときにMERGE処理を実行することもできる。  
OPENROWSET(BULK …)は、SQLServer2005から提供された機能で、bcpコマンドと同じようにテキストファイルのインポートが可能。  
bcpとの違いはフォーマットファイルが必須である点。  

t1テーブル

``` txt : t1テーブル
a | b
--+--
1 | AAA
2 | BBB
3 | XXX
4 | DDD
5 | YYY
```

bulkText1.csvテキストファイル

``` txt : bulkText1.csvテキストファイル

4,FFF
6,GGG
```

bulkTest1.fmt(フォーマットファイル)  

``` txt : bulkTest1.fmt(フォーマットファイル)
13.0
2
1 SQLCHAR 0 12 ","    1 a ""
2 SQLCHAR 0 10 "\r\n" 2 b Japanese_CI_AS
```

実行SQL

``` sql : 実行SQL
MERGE INTO t1
USING OPENROWSET(BULK 'C:\temp\bulkTest1.csv'
                 ,FORMATFILE = 'C:\temp\bulkTest1.fmt') bulk1
ON t1.a = bulk1.a
WHEN MATCHED THEN
    UPDATE SET t1.b = bulk.b
WHEN NOT MATCHED THEN
    INSERT VALUES (bulk.a,bulk1.b);
```

t1テーブルの結果

``` txt : t1テーブルの結果
a | b
--+--
1 | AAA
2 | BBB
3 | XXX
4 | FFF ←UPDATEされたデータ
5 | YYY
6 | GGG ←INSERTされたデータ
```

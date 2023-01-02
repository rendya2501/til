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
|1  | AAA |
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

# CTE(共通テーブル式)

## 概要

CTE(Common Table Expression : 共通テーブル式)は、SELECTステートメントで取得した結果に対して名前を付けることができる機能。  
インラインビューの代わりとして利用できる。  

CTEはSQL99規格(1999年に規格化されたSQL標準)に準拠した機能。  
SQLServer 2005からサポートされた。  

``` sql
WITH 式名 [(列名1, 列名2, …)]
AS
(SELECT ステートメント)
```

---

## インラインビュー(サブクエリ)とCTEの使い分け

インラインビューとCTEは、クエリの内容にもよるが、内部動作が同じになることが多いので、どちらを利用してもパフォーマンスはほとんど変わらない。  
「クエリの読みやすさ」という点では、CTEのほうが上から順に解釈できるのでインラインビューよりも読みやすく、SQL標準という点では、インラインビューはSQL92、CTEはSQL99規格。  

※インラインビュー : サブクエリのこと  
![Alt text](https://image.itmedia.co.jp/ait/articles/1209/14/jo_pic7.jpg)  
[高度な副問合せの構文](https://atmarkit.itmedia.co.jp/ait/articles/1209/14/news146.html#:~:text=%E3%80%8C%E3%82%A4%E3%83%B3%E3%83%A9%E3%82%A4%E3%83%B3%E3%83%BB%E3%83%93%E3%83%A5%E3%83%BC%E3%80%8D%E3%81%A8%E3%81%AF,%E3%81%99%E3%82%8B%E3%81%93%E3%81%A8%E3%82%82%E3%81%A7%E3%81%8D%E3%81%BE%E3%81%99%E3%80%82)  

---

## 自己結合

``` sql
CREATE TABLE 社員
( 
    社員番号 int NOT NULL PRIMARY KEY,
    社員名 varchar(40) NULL,
    上司社員番号 int NULL,
    性別 char(4) NULL 
)
INSERT INTO 社員 VALUES (1001, '山田 太郎', NULL, '男性')
INSERT INTO 社員 VALUES (1002, '鈴木 一郎', NULL, '男性')
INSERT INTO 社員 VALUES (1003, '伊藤 朋子', 1001, '女性')
INSERT INTO 社員 VALUES (1004, '若旅 素子', 1002, '女性')
INSERT INTO 社員 VALUES (1005, '佐藤 啓太', 1001, '男性')
INSERT INTO 社員 VALUES (1006, '川崎 太郎', 1003, '男性')

-- 自己結合
SELECT 社員.*, 上司社員.社員名 AS 上司社員名
 FROM 社員 INNER JOIN 社員 AS 上司社員
   ON 社員.上司社員番号 = 上司社員.社員番号 

-- 再帰クエリで階層のレベルを取得
WITH cte1 (社員番号, 社員名, 上司社員番号, 階層)
AS
(
 -- 上司
 SELECT 社員番号, 社員名, 上司社員番号, 0
  FROM 社員
   WHERE 社員番号 = 1001
        UNION ALL
 -- 部下（再帰）
 SELECT e.社員番号, e.社員名, e.上司社員番号, cte1.階層 + 1
  FROM 社員 AS e
   INNER JOIN cte1
     ON e.上司社員番号 = cte1.社員番号
)
SELECT * FROM cte1

-- HierarchyID で階層のパスを取得
WITH cte1 (path, 社員番号, 社員名, 上司社員番号, 階層)
AS
(
    -- 上司
    SELECT   
    HierarchyID::GetRoot() AS root
    , 社員番号, 社員名, 上司社員番号, 0
    FROM 社員
    WHERE 社員番号 = 1001
    UNION ALL
    -- 部下（再帰）
    SELECT
        CAST( cte1.path.ToString() 
        + CAST(e.社員番号 AS varchar(30))
        + '/' AS HierarchyID )
        ,e.社員番号, e.社員名, e.上司社員番号, cte1.階層+ 1
    FROM 社員 AS e
    INNER JOIN cte1
    ON e.上司社員番号 = cte1.社員番号
)
SELECT path.ToString(), * FROM cte1
```

<http://www.sqlquality.com/books/dev03/11-1.txt>  

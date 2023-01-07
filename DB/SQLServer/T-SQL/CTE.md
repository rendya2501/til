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
「クエリの読みやすさ」という点では、CTEのほうが上から順に解釈できるのでインラインビューよりも読みやすいと思われる。  

- SQL標準  
  - インラインビュー : SQL92規格  
  - CTE : SQL99規格  

※インラインビュー : サブクエリのこと  

![Alt text](https://image.itmedia.co.jp/ait/articles/1209/14/jo_pic7.jpg)  
[高度な副問合せの構文](https://atmarkit.itmedia.co.jp/ait/articles/1209/14/news146.html#:~:text=%E3%80%8C%E3%82%A4%E3%83%B3%E3%83%A9%E3%82%A4%E3%83%B3%E3%83%BB%E3%83%93%E3%83%A5%E3%83%BC%E3%80%8D%E3%81%A8%E3%81%AF,%E3%81%99%E3%82%8B%E3%81%93%E3%81%A8%E3%82%82%E3%81%A7%E3%81%8D%E3%81%BE%E3%81%99%E3%80%82)  

---

## CTEと再帰クエリ

CTEを利用すれば、再帰クエリを実現することができる。  
再帰クエリとは、SELECTステートメントで取得した結果セットに対して、再帰的に繰り返し呼び出すことができるクエリのことを指す。  
階層関係にあるデータのレベル(階層数)を取得することも可能。  

``` sql
-- テストデータ

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
```

テストデータにおける上司と部下の階層関係は以下の通り

``` txt
1001 山田太郎
    ｜
    ├-─> 1003 伊藤 朋子
    ｜            ｜
    ｜            └─-> 1006 川崎 太郎
    ｜
    └─-> 1005 佐藤 啓太
```

この関係(階層レベル)は次のように再帰クエリを利用して取り出すことができる。
CTEでは結果に対して再帰的にクエリを実行する事ができるので、このように階層レベルも取得できる。  

``` sql
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

-- | 社員番号 | 社員名    | 上司社員番号 | 階層
---|----------+-----------+--------------+------
-- | 1001     | 山田 太郎 | NULL         | 0
-- | 1003     | 伊藤 朋子 | 1001         | 1
-- | 1005     | 佐藤 啓太 | 1001         | 1
-- | 1006     | 川崎 太郎 | 1003         | 2
```

## HierarchyID データ型

HierarchyIDは2008からサポートされた機能。  
階層(Hierarchy)のレベルだけでなく、階層のパスも取得/操作が可能なデータ型。  
このデータ型では、`GetRoot`メソッドや`Path`プロパティが用意され、階層を操作できるようになっている。  

``` sql
-- HierarchyID で階層のパスを取得

WITH cte1 (path, 社員番号, 社員名, 上司社員番号, 階層)
AS
(
    -- 上司
    SELECT
        HierarchyID::GetRoot() AS root,
        社員番号, 社員名, 上司社員番号, 0
    FROM 社員
    WHERE 社員番号 = 1001
    UNION ALL
    -- 部下（再帰）
    SELECT
        CAST( 
            CONCAT(
                cte1.path.ToString(),
                CAST(e.社員番号 AS varchar(30)),
                '/'
            ) AS HierarchyID 
        ),
        e.社員番号, e.社員名, e.上司社員番号, cte1.階層+ 1
    FROM 社員 AS e
    INNER JOIN cte1
    ON e.上司社員番号 = cte1.社員番号
)
SELECT path.ToString(), * FROM cte1

-- | (列名なし)  | path         | 社員番号  | 社員名    | 上司社員番号 | 階層
-- +-------------+--------------+-----------+-----------+--------------+------
-- | /           | 0x           | 1001      | 山田 太郎 | NULL         | 0
-- | /1003/      | 0xEE2DC0     | 1003      | 伊藤 朋子 | 1001         | 1
-- | /1005/      | 0xEE2EC0     | 1005      | 佐藤 啓太 | 1001         | 1
-- | /1003/1006/ | 0xEE2DFB8BD0 | 1006      | 川崎 太郎 | 1003         | 2
```

<http://www.sqlquality.com/books/dev03/11-1.txt>  

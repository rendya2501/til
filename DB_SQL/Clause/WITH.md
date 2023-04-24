# WITH句

---

## 概要

ビューみたいな定義ができる構文。  
JOIN句の中では使えないことが判明した。  
なので、単品で使う分でしか無理っぽい。  

[SQL WITH句で同じSQLを１つのSQLに共通化する](https://zukucode.com/2017/09/sql-with.html)  

---

## クエリの途中でWITHを使う

SQLServerでのWITH句は基本的にステートメントの一番上で定義しなければならないが`;`でステートメントを区切ることで途中でもWITHを使うことができる。  

``` sql
-- 前のステートメント
SELECT Column1, Column2
FROM Table1;

-- CTEを使用した新しいステートメント
WITH CTE AS (
  -- ここにCTEのクエリを記述します。
)
SELECT *
FROM CTE;
```

---

## 複数のテーブルを指定する例

``` sql : 複数のテーブルを指定する
WITH sample_with AS (
  SELECT *
  FROM sample
  WHERE COL1 = 'sample'
),
sample2_with AS (
  SELECT *
  FROM sample2
    -- WITH句で定義したテーブルも参照可能
    JOIN sample_with
    ON sample2.COL1 = sample_with.COL1
  WHERE sample2.COL1 = 'sample'
)
SELECT
```

---

## テストデータ的な使い方

いい感じのWITH句と相関副問い合わせのサンプルが出来たのでまとめておく。

``` sql
WITH [with_product] AS (
  SELECT 1 AS [id] ,'りんご' AS [name],'フルーツ' AS [category] ,10 AS [kosuu]
  UNION
  SELECT 2,'みかん','フルーツ',20
  UNION
  SELECT 3,'にんじん','野菜',30
  UNION
  SELECT 4,'大根','野菜',40
)
select [id],[name] 
from [with_product] AS [a]
where [kosuu] = (select max(kosuu) from [with_product] AS [b] where [a].[category] = [b].[category]);
```

---

## WITHから一時テーブルを生成する

WITHはちょっとしたデータを用意する分には便利なのだが、サブクエリの中で動作を確認したい時等、WITHも含めてSELECTしないとエラーとなってしまうためストレスを感じる事が多かった。  
WITHから型定義なしでとりあえず一時テーブルを作る方法はないか考えた。  
`SELECT * INTO dst FROM src`構文が使えるのでは？と思ってやってみたらいけた。  

``` sql
WITH WithTemp AS (
    SELECT 'ABC20200725001000009001434' AS ID
    UNION
    SELECT 'ABC20200725001000011000942'
    UNION
    SELECT 'ABC20200725001000012000176'
    UNION
    SELECT 'ABC20200725001000018001431'
)
SELECT * INTO #TempTable FROM WithTemp

DROP TABLE IF EXISTS #TempTable
```

ただ、テーブルを作るのが面倒くさいと思っていたが、DECLAREと同じ要領でやればいいだけ、と考えれば、CREATETABLEから一時テーブルを作る事なんて大したことではないのかもしれないと思ったり。  
WITHから生成と言いつつ、単純にサブクエリから作っていることに変わりはないけど、こういうこともできるよって、ことだけ残しておく。  

追記  
INSERT INTO VALUES使えばこんなことする必要ない。  

``` sql
SELECT * INTO #TempTable 
FROM (
    VALUES
        ('ABC20200725001000009001434'),
        ('ABC20200725001000011000942'),
        ('ABC20200725001000012000176'),
        ('ABC20200725001000018001431')
) AS t(ID)
```

---

## CTEは

Q.CTEを元に複数のテーブルに対してupdateを実行するみたいな芸当は可能か？
A.無理。  
素直に一時テーブルかテーブル変数を使いましょう。  

``` sql
-- テーブル1 (Table1) を作成
CREATE TABLE Table1 (
  ID INT PRIMARY KEY,
  Name NVARCHAR(50),
  ModifiedDate DATE
);
-- テーブル1 (Table1) にデータを挿入
INSERT INTO Table1 (ID, Name, ModifiedDate)
VALUES (1, 'John', '2023-01-01'),
       (2, 'Jane', '2023-01-01');


-- テーブル2 (Table2) を作成
CREATE TABLE Table2 (
  ID INT PRIMARY KEY,
  Name NVARCHAR(50),
  ModifiedDate DATE
);
-- テーブル2 (Table2) にデータを挿入
INSERT INTO Table2 (ID, Name, ModifiedDate)
VALUES (1, 'John', '2023-01-01'),
       (2, 'Jane', '2023-01-01');


-- CTEの定義
WITH UpdateDatesCTE AS (
  SELECT 1 AS ID, CAST('2023-04-24' AS DATE) AS NewModifiedDate
  UNION ALL
  SELECT 2 AS ID, CAST('2023-04-25' AS DATE) AS NewModifiedDate
)

-- CTEを使用してTable1を更新
UPDATE Table1
SET ModifiedDate = UpdateDatesCTE.NewModifiedDate
FROM Table1
JOIN UpdateDatesCTE ON Table1.ID = UpdateDatesCTE.ID;

-- CTEを使用してTable2を更新
UPDATE Table2
-- マルチパート識別子"UpdateDatesCTE.NewModifiedDate"をバインドできませんでした。
SET ModifiedDate = UpdateDatesCTE.NewModifiedDate
FROM Table2
JOIN UpdateDatesCTE ON Table2.ID = UpdateDatesCTE.ID;
```

``` sql
-- 一時テーブルの作成
CREATE TABLE #UpdateDatesTempTable (
  ID INT PRIMARY KEY,
  NewModifiedDate DATE
);

-- CTEの結果を一時テーブルに挿入
WITH UpdateDatesCTE AS (
  SELECT 1 AS ID, CAST('2023-04-24' AS DATE) AS NewModifiedDate
  UNION ALL
  SELECT 2 AS ID, CAST('2023-04-25' AS DATE) AS NewModifiedDate
)
INSERT INTO #UpdateDatesTempTable
SELECT * FROM UpdateDatesCTE;

-- 一時テーブルを使用してTable1を更新
UPDATE Table1
SET ModifiedDate = #UpdateDatesTempTable.NewModifiedDate
FROM Table1
JOIN #UpdateDatesTempTable ON Table1.ID = #UpdateDatesTempTable.ID;

-- 一時テーブルを使用してTable2を更新
UPDATE Table2
SET ModifiedDate = #UpdateDatesTempTable.NewModifiedDate
FROM Table2
JOIN #UpdateDatesTempTable ON Table2.ID = #UpdateDatesTempTable.ID;

-- 一時テーブルの削除
DROP TABLE #UpdateDatesTempTable;
```

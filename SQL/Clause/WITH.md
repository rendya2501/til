# WITH句

---

## 概要

ビューみたいな定義ができる構文。  
JOIN句の中では使えないことが判明した。  
なので、単品で使う分でしか無理っぽい。  

[SQL WITH句で同じSQLを１つのSQLに共通化する](https://zukucode.com/2017/09/sql-with.html)  

---

## 実装例

``` sql : 使い方例
WITH employee_with AS (
  SELECT *
  FROM
    employee T1
  WHERE
    T1.last_name = '山田'
)
SELECT
  T1.id,
  T1.first_name,
  T1.last_name,
  T1.department_id,
  (
    SELECT
      AVG(SUB1.height)
    FROM
      -- WITH句で指定したテーブルを参照
      employee_with SUB1
    WHERE
      T1.department_id = SUB1.department_id
  ) AS avg_height,
  (
    SELECT
      MAX(SUB1.height)
    FROM
      -- WITH句で指定したテーブルを参照
      employee_with SUB1
    WHERE
      T1.department_id = SUB1.department_id
  ) AS max_height
FROM
  -- WITH句で指定したテーブルを参照
  employee_with T1
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

# WITH句

[SQL WITH句で同じSQLを１つのSQLに共通化する](https://zukucode.com/2017/09/sql-with.html)  

ビューみたいな定義ができる構文。  
JOIN句の中では使えないことが判明した。  
なので、単品で使う分でしか無理っぽい。  

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
WITH employee_with AS (
  SELECT 1 As id ,"りんご" as name,"フルーツ" as category ,10 as kosuu
  UNION
  SELECT 2,"みかん","フルーツ",20
  UNION
  SELECT 3,"にんじん","野菜",30
  UNION
  SELECT 4,"大根","野菜",40
)
select id,name 
from employee_with a
where kosuu = (select max(kosuu) from employee_with b where a.category = b.category);
```

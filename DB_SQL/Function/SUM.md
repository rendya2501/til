# SUM

## NULLのSUM

SUM関数はNULL以外の値の合計値を返す。  
レコードにNULLが混じっていても問題なく計算を行う。  
ただし、すべてのレコードがNULLだった場合、SUM関数はNULLを返す。  

``` sql
SELECT SUM(A) AS A,SUM(B) AS B,SUM(C) AS C,SUM(D) AS D
FROM (
    VALUES
        (1,CONVERT(int,NULL),CONVERT(int,NULL),1),
        (CONVERT(int,NULL),1,CONVERT(int,NULL),1)
) AS T(A,B,C,D)

-- A B C    D
-- 1 1 NULL 2
```

因みに`VALUES`や`WITH`等で定義した時に、CONVERT等で型の情報を入れてあげないと全てNULLだった場合、エラーとなる。  

``` sql
-- メッセージ 8117、レベル 16、状態 1、行 22
-- オペランドのデータ型 NULL は sum 演算子では無効です。
SELECT SUM(A)
FROM (VALUES(NULL),(NULL)) AS T(A)

SELECT ISNULL(SUM(A),0)
FROM (VALUES(NULL),(NULL)) AS T(A)
```

フィールドをISNULLすればエラーにはならない。  
まぁ、それだけではあるが。  

``` sql
SELECT SUM(ISNULL(A,0)) AS A
FROM (VALUES(NULL),(NULL)) AS T(A)

-- A
-- 0
```

[SUM 合計値を返すSQL関数の使い方](https://segakuin.com/oracle/function/sum.html)

---

GROUP BY しなくても合計を求めることができる。  

``` txt
ProductID   Price
1           100
2           200
3           300
```

```sql
-- 300
select SUM(Price)
from ProductTable
where ProductID IN (1,2)
```

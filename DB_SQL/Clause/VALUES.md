# VALUES

INSERT INTOする時に使うのは知っていたが、まさかSELECTでも使えるとは思っていなかった。

---

## 基本

VALUES は SQL 標準。  

VALUESでの以下の構文は、その下のUNION ALLと同じ事をしている。  

``` sql
SELECT column1,column2
FROM (
    VALUES
        (1, 'one'),
        (2, 'two'),
        (3, 'three')
) AS t(column1, column2)
```

``` sql
SELECT 1 AS column1, 'one' AS column2
UNION ALL
SELECT 2, 'two'
UNION ALL
SELECT 3, 'three';
```

---

[VALUES](https://www.postgresql.jp/document/8.2/html/sql-values.html)  
[テーブル値コンストラクター (Transact-SQL) - SQL Server | Microsoft Learn](https://learn.microsoft.com/ja-jp/sql/t-sql/queries/table-value-constructor-transact-sql?view=sql-server-ver16)  

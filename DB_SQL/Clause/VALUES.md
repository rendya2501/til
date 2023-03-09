# VALUES

INSERT INTOする時に使うのは知っていたが、まさかSELECTでも使えるとは思っていなかった。

---

## 基本

VALUES は SQL 標準。  

以下のVALUESは、その下のUNION ALLと同じ事をしている。  

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

## VALUES内でのSELECT

VALUES内でSELECTを使用する場合は、1フィールドにつき 1 SELECTすること。  
複数フィールドのSELECTはエラーとなってしまう。  

■ OK

``` sql
SELECT A,B,C
FROM (
    VALUES
        (1,1,(SELECT 'HOGE')),
        (1,2,(SELECT 'Fuga')),
        (2,1,(SELECT 'Piyo'))
) AS T(A,B,C)
```

■ NG

``` sql
SELECT A,B,C
FROM (
    VALUES
        (SELECT 1,1,'HOGE'),
        (SELECT 1,2,'Fuga'),
        (SELECT 2,1,'Piyo')
) AS T(A,B,C)
-- SELECT付近に不適切な構文があります。
```

``` sql
SELECT A,B,C
FROM (
    VALUES
        ((SELECT 1,1,'HOGE')),
        ((SELECT 1,2,'Fuga')),
        ((SELECT 2,1,'Piyo'))
) AS T(A,B,C)
-- 'T'には、列リストよりも少ない列しか指定されていません。
```

``` sql
SELECT A
FROM (
    VALUES
        ((SELECT 1,1,'HOGE')),
        ((SELECT 1,2,'Fuga')),
        ((SELECT 2,1,'Piyo'))
) AS T(A)
-- メッセージ 116、レベル 16、状態 1、行 4
-- EXISTS を使用しないサブクエリでは、サブクエリの選択リストには、式を 1 つだけしか指定できません。
-- メッセージ 116、レベル 16、状態 1、行 5
-- EXISTS を使用しないサブクエリでは、サブクエリの選択リストには、式を 1 つだけしか指定できません。
-- メッセージ 116、レベル 16、状態 1、行 6
-- EXISTS を使用しないサブクエリでは、サブクエリの選択リストには、式を 1 つだけしか指定できません。
```

■ 解説

Microsoft公式の例をそのまま引用すると、以下のVALUESは問題なくいけそうに見えるが、エラーとなってしまう。  
仕様として単一のスカラー値しか受け付けない模様。  

``` sql
VALUES ('Helmet', 25.50),  
       ('Wheel', 30.00),  
       (SELECT Name, ListPrice FROM Production.Product WHERE ProductID = 720);  
```

なので、複数のフィールドを使いたい場合は、それぞれ単一のSELECTに分解する必要がある。  

``` sql
VALUES ('Helmet', 25.50),  
       ('Wheel', 30.00),  
       ((SELECT Name FROM Production.Product WHERE ProductID = 720),  
        (SELECT ListPrice FROM Production.Product WHERE ProductID = 720));  
```

[テーブル値コンストラクター (Transact-SQL) - SQL Server | Microsoft Learn](https://learn.microsoft.com/ja-jp/sql/t-sql/queries/table-value-constructor-transact-sql?view=sql-server-ver16#limitations-and-restrictions)  

---

[VALUES](https://www.postgresql.jp/document/8.2/html/sql-values.html)  
[テーブル値コンストラクター (Transact-SQL) - SQL Server | Microsoft Learn](https://learn.microsoft.com/ja-jp/sql/t-sql/queries/table-value-constructor-transact-sql?view=sql-server-ver16)  

# GROUP BY した結果の1件目を取得する

子テーブルから親テーブルを作る必要があるときに、コードで絞った結果の1件目だけを親テーブルにINSERTしたい状況に遭遇した時のまとめ。  

「group by 先頭1件」で検索。  
Rank() を使うらしい。  

---

## データ

``` sql
CREATE TABLE [Test]
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,  
    [Code] INT,  
    [Name] nvarchar(255)
);
INSERT INTO [Test]
VALUES
     ( 1 ,'A')
    ,( 1 ,'B')
    ,( 1 ,'C')
    ,( 1 ,'D')
    ,( 2 ,'F')
    ,( 2 ,'G')
    ,( 2 ,'H')
    ,( 3 ,'X')
    ,( 3 ,'Y')
    ,( 3 ,'Z')
```

---

## RANK案

[このサイト](https://oshiete.goo.ne.jp/qa/3819843.html)で紹介されていた案。  

``` sql
SELECT
    [X].[Code],
    [X].[Name]
FROM
(
    SELECT
        RANK() OVER(PARTITION BY [Code] ORDER BY [Name]) AS rk,
        [Code],
        [Name]
    FROM [Test]
) AS [X]
WHERE rk=1

-- 1 A
-- 2 F
-- 3 X
```

---

## FIRST_VALUE案

これでもいける。  

``` sql
SELECT DISTINCT
    [Code],
    FIRST_VALUE([Name]) OVER (PARTITION BY [Code] ORDER BY [Name])
FROM [#Test]
```

しかしRANK案よりコストは高い模様。  
SQLServerの[推定実行プランの表示]では 「36% : 64%」 と表示された。  

---

[SELECT 文　GROUP　での1件目を取得](https://oshiete.goo.ne.jp/qa/3819843.html)  

# COUNT(DISTINCT)

`分子分母を出力するサンプル`で`COUNT(DISTINCT)`出来ない事を発見したが、いい感じのサンプルにまとめることが出来なかった。  

同じことを考えてる人がいたので、この人のサンプルを使わせてもらう。  
[Count distinct over partition by](https://stackoverflow.com/questions/66348498/count-distinct-over-partition-by)  

---

``` sql
DROP TABLE IF EXISTS AAA
CREATE TABLE AAA (
    [Date] DATETIME,
    [Name] VARCHAR(255),
    [Role] VARCHAR(255)
)
INSERT INTO AAA 
VALUES
    ('20000101','Sam','Manager'),
    ('20000201','Sam','Manager'),
    ('20000101','John','Manager'),
    ('20000101','Dan','Manager'),
    ('20000101','Bob','Analyst'),
    ('20000201','Bob','Analyst'),
    ('20000101','Mike','Analyst')
```

Roleで絞った時のNameを集計したい。  
この時重複は排除する。  
下記のSQLではRoleで絞って単純にCountしているだけなのでRoleのレコード数と同じ数が表示されている。  
Managerで絞った時、「Sam,Sam,John,Dan」なので「4」になる。  
これをDISTINCTすれば「Sam,John,Dan」で「3」とすることができる。  

``` sql
SELECT
    [date],
    [Name],
    [Role],
    COUNT([Name]) OVER (PARTITION BY [Role]) AS [Role_Count]
FROM [AAA]
GROUP BY [date], [name], [role]
ORDER BY [Role] DESC,[Name] DESC,[date]

-- date                     Name  Role     Role_Count
-- 2000-01-01 00:00:00.000  Sam   Manager   4
-- 2000-02-01 00:00:00.000  Sam   Manager   4
-- 2000-01-01 00:00:00.000  John  Manager   4
-- 2000-01-01 00:00:00.000  Dan   Manager   4
-- 2000-01-01 00:00:00.000  Mike  Analyst   3
-- 2000-01-01 00:00:00.000  Bob   Analyst   3
-- 2000-02-01 00:00:00.000  Bob   Analyst   3
```

なのでCount句の中でDISTINCTするだけで行けそうに見えるが実際にはエラーとなってしまう。  
SQLServerは確実に無理。  
他は知らないが、ほとんど無理らしい。  

>Unfortunately, COUNT(DISTINCT is not available as a window aggregate.
>COUNT(DISTINCT)残念ながら、SQL Server (および他のデータベースも) はウィンドウ関数としてサポートされていません。
>[Count distinct over partition by](https://stackoverflow.com/questions/66348498/count-distinct-over-partition-by)  

---

``` sql
SELECT
    [date],
    [Name],
    [Role],
    -- メッセージ 10759、レベル 15、状態 1、行 24
    -- OVER 句で DISTINCT を使用することはできません。
    COUNT(DISTINCT [Name]) OVER (PARTITION BY [Role]) AS [Role_Count]
FROM [AAA]
GROUP BY [date], [name], [role]
ORDER BY [Role] DESC,[Name] DESC,[date]
```

---

解決する手段としてメジャーなのが`DENSE_RANK関数`を使った方法らしい。  
パッ見、黒魔術。  
だが、ちゃんと意図したとおりに出力されている。  

`DENSE_RANK`は同じ値があった場合でも、番号を飛ばさないタイプのRANK関数。  
昇順と降順でランク付けした後に-1することでDISTINCTしたCOUNTになっているのを確認できる。  

``` txt
重複を除いた自分以上の組み合わせの数 + 重複を除いた自分以下の組み合わせの数 -1
= 重複を除いた組み合わせの数

DENSE_RANK() OVER (PARTITION BY [Role] ORDER BY [Name] asc) 
+ DENSE_RANK() OVER (PARTITION BY [Role] ORDER BY [Name] desc) 
- 1 = COUNT(DISTINCT [NAME])
```

``` sql
SELECT
    [date], 
    [Name], 
    [Role],
    DENSE_RANK() OVER (PARTITION BY [Role] ORDER BY [Name] ASC) AS [ASC],
    DENSE_RANK() OVER (PARTITION BY [Role] ORDER BY [Name] DESC) AS [DESC],
    (
        DENSE_RANK() OVER (PARTITION BY [Role] ORDER BY [Name] ASC) +
        DENSE_RANK() OVER (PARTITION BY [Role] ORDER BY [Name] DESC) -
        1
    ) AS [Role_Count]
FROM [AAA]
GROUP BY [date], [name], [role]
ORDER BY [Role] DESC,[Name] DESC,[date]

-- date                     Name  Role     ASC  DESC  Role_Count
-- 2000-01-01 00:00:00.000  Sam   Manager   3    1     3
-- 2000-02-01 00:00:00.000  Sam   Manager   3    1     3
-- 2000-01-01 00:00:00.000  John  Manager   2    2     3
-- 2000-01-01 00:00:00.000  Dan   Manager   1    3     3
-- 2000-01-01 00:00:00.000  Mike  Analyst   2    1     2
-- 2000-01-01 00:00:00.000  Bob   Analyst   1    2     2
-- 2000-02-01 00:00:00.000  Bob   Analyst   1    2     2
```

JOINすればやり方はいくらでもあるだろう。

``` sql
SELECT 
    [M].[date], 
    [M].[Name], 
    [M].[Role],
    [T].[Role_Count]
FROM 
    AAA M
    JOIN (
        SELECT 
            [Role],
            COUNT(DISTINCT [Name]) AS Role_Count
        FROM AAA
        GROUP BY [Role]
    ) AS T
    ON EXISTS (SELECT M.[Role] INTERSECT SELECT T.[Role]) 
ORDER BY [Role] DESC,[Name] DESC,[date]
```

---

[Count distinct over partition by](https://stackoverflow.com/questions/66348498/count-distinct-over-partition-by)  
[Partition Function COUNT() OVER possible using DISTINCT](https://stackoverflow.com/questions/11202878/partition-function-count-over-possible-using-distinct)  
[PostgreSQLの分析関数の衝撃6（window関数の応用例）](https://codezine.jp/article/detail/4747)  
[OVERを指定したウィンドウ関数でDISTINCTを使用する](https://www.web-dev-qa-db-ja.com/ja/sql-server/over%E3%82%92%E6%8C%87%E5%AE%9A%E3%81%97%E3%81%9F%E3%82%A6%E3%82%A3%E3%83%B3%E3%83%89%E3%82%A6%E9%96%A2%E6%95%B0%E3%81%A7distinct%E3%82%92%E4%BD%BF%E7%94%A8%E3%81%99%E3%82%8B/l958295335/)  

# MAX

---

## MAX関数 + CASE式

単純にMAXを取りたいけど、極端に大きな値が混ざってしまっているために、意図しないMAXを取得してしまう。  
これを解決するためにアレコレ考えたが、MAXでCASE文を使えることを知り、極端な値以下でMAXを取得できないかやってみたら出来たのでまとめる。  

``` sql : テストデータ準備
DROP TABLE IF EXISTS TestTable
CREATE TABLE TestTable(BusinessDate DATETIME,HogeNumber INT)
INSERT INTO TestTable
VALUES
    ('2022-07-19',1),
    ('2022-07-19',2),
    ('2022-07-20',1),
    ('2022-07-20',2),
    ('2022-07-20',3),
    ('2022-07-20',4),
    ('2022-07-20',99998),
    ('2022-07-21',1),
    ('2022-07-21',2),
    ('2022-07-21',3),
    ('2022-07-21',99998),
    ('2022-07-22',99998)
```

``` sql
SELECT 
    BusinessDate,
    MAX(HogeNumber),
    MAX(CASE WHEN HogeNumber < 99998 THEN HogeNumber ELSE 0 END)
FROM TestTable
GROUP BY BusinessDate
ORDER BY BusinessDate DESC

-- BusinessDate             単純MAX    CASE WHEN MAX
-- 2022-07-22 00:00:00.000    99998    0
-- 2022-07-21 00:00:00.000    99998    3
-- 2022-07-20 00:00:00.000    99998    4
-- 2022-07-19 00:00:00.000        2    2
```

因みにこれが分からなかったときに愚直にやった方法は以下の通り。  
単純にWHEREで99998以下にしてしまうと、その日付のレコードが出力されないので、自分自身の日付をDISTINCTして、それに対してLEFT JOINして、サブクエリでWHEREしてMAXして、ないものに関しては0にするという方法。  

``` sql
SELECT 
    [A].[BusinessDate],
    ISNULL([B].[HogeNumber],0)
FROM
    (SELECT DISTINCT [BusinessDate] FROM [TestTable]) AS [A]
    LEFT JOIN (
        SELECT [BusinessDate], MAX([HogeNumber]) AS [HogeNumber] 
        FROM [TestTable] 
        WHERE [HogeNumber] < 99998
        GROUP BY [BusinessDate]
    ) AS [B]
    ON [A].[BusinessDate] = [B].[BusinessDate]
ORDER BY BusinessDate DESC

-- BusinessDate
-- 2022-07-22 00:00:00.000    0
-- 2022-07-21 00:00:00.000    3
-- 2022-07-20 00:00:00.000    4
-- 2022-07-19 00:00:00.000    2
```

[集約関数にCASE式で条件をつける](https://qiita.com/yatto5/items/8c9b8ca6b01d83bd95bc)  

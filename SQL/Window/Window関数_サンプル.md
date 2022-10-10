# Window関数_サンプル

---

## 分子分母を出力するサンプル

Window関数を初めて知ってから組み上げた実践サンプル。  

1つの予約が複数の時間に跨っている様を分母分子で表示するサンプル  
1つの時間に複数の予約がある場合は「+」を表示させる。  

``` sql : データ準備
DROP TABLE IF EXISTS TestReservation
CREATE TABLE TestReservation
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,  
    [ReservationTime] TIME,  
    [ReservationNo] nvarchar(255)
);
INSERT INTO TestReservation
VALUES
     ('07:00:00.0000000','RES202204160001')
    ,('07:07:00.0000000','RES202204160001')
    ,('07:14:00.0000000','RES202204160001')
    ,('07:21:00.0000000','RES202204160001')
    ,('07:28:00.0000000','RES202204160002')
    ,('07:35:00.0000000','RES202204160002')
    ,('07:42:00.0000000','RES202204160003')
    ,('07:42:00.0000000','RES202204160004')
    ,('07:49:00.0000000','RES202204160003')
    ,('07:56:00.0000000','RES202204160005')
```

``` sql
SELECT
    [ReservationTime],
    [ReservationNo],
    -- その予約の時間の順番(分子:Numerator)
    ROW_NUMBER() OVER(PARTITION BY [ReservationNo] ORDER BY [ReservationTime]) AS [Numerator],
    -- その予約の合計(分母:Denominator)
    COUNT(1) OVER(PARTITION BY [ReservationNo]) AS [Denominator],
    -- 分数表示。その時間に複数の予約がある場合は+を表示する
    CONVERT(nvarchar,ROW_NUMBER() OVER(PARTITION BY [ReservationNo] ORDER BY [ReservationTime]))
    + '/' 
    + CONVERT(nvarchar,COUNT(1) OVER(PARTITION BY [ReservationNo]))
    + CASE WHEN COUNT(*) OVER (PARTITION BY [ReservationTime]) > 1 THEN '+' ELSE '' END
    AS [Result]
FROM [TestReservation]
GROUP BY [ReservationTime],[ReservationNo]

-- ReservationTime   ReservationNo    Numerator  Denominator  Result
-- 07:00:00.0000000  RES202204160001      1          4        1/4   
-- 07:07:00.0000000  RES202204160001      2          4        2/4   
-- 07:14:00.0000000  RES202204160001      3          4        3/4   
-- 07:21:00.0000000  RES202204160001      4          4        4/4   
-- 07:28:00.0000000  RES202204160002      1          2        1/2   
-- 07:35:00.0000000  RES202204160002      2          2        2/2   
-- 07:42:00.0000000  RES202204160003      1          2        1/2+  
-- 07:42:00.0000000  RES202204160004      1          1        1/1+  
-- 07:49:00.0000000  RES202204160003      2          2        2/2   
-- 07:56:00.0000000  RES202204160005      1          1        1/1   
```

Window関数がよくわからなかったときに組んだ事例も備忘録として残しておく。  

``` sql : 超愚直にやるならこう
SELECT
    [A].[ReservationTime],
    [A].[ReservationNo],
    CONVERT(nvarchar,[A].[Numerator]) AS [Numerator],
    CONVERT(nvarchar,[B].[Denominator]) AS [Denominator],
    CASE WHEN COUNT(*) OVER (PARTITION BY [ReservationTime]) > 1 THEN '+' ELSE '' END AS [Emphasis],
    CONVERT(nvarchar,[A].[Numerator])
    + '/'
    + CONVERT(nvarchar,[B].[Denominator])
    + CASE WHEN COUNT(*) OVER (PARTITION BY [ReservationTime]) > 1 THEN '+' ELSE '' END AS [Result]
FROM (
    -- その予約の時間の順番(分子:Numerator)
    SELECT
        [ReservationTime],
        [ReservationNo],
        ROW_NUMBER() OVER(PARTITION BY [ReservationNo] ORDER BY [ReservationTime]) AS [Numerator]
    FROM 
        [TestReservation]
    GROUP BY
        [ReservationTime],[ReservationNo]
    ) AS [A]
JOIN (
    -- その予約の合計(分母:Denominator)
    SELECT
        [ReservationNo],
        COUNT(DISTINCT [ReservationTime]) AS [Denominator]
    FROM 
        [TestReservation]
    GROUP BY 
        [ReservationNo]
) AS [B]
ON [A].[ReservationNo] = [B].[ReservationNo]

-- ReservationTime   ReservationNo    Numerator  Denominator  Emphasis  Result
-- 07:00:00.0000000  RES202204160001    1             4                 1/4
-- 07:07:00.0000000  RES202204160001    2             4                 2/4
-- 07:14:00.0000000  RES202204160001    3             4                 3/4
-- 07:21:00.0000000  RES202204160001    4             4                 4/4
-- 07:28:00.0000000  RES202204160002    1             2                 1/2
-- 07:35:00.0000000  RES202204160002    2             2                 2/2
-- 07:42:00.0000000  RES202204160003    1             2          +      1/2+
-- 07:42:00.0000000  RES202204160004    1             1          +      1/1+
-- 07:49:00.0000000  RES202204160003    2             2                 2/2
-- 07:56:00.0000000  RES202204160005    1             1                 1/1
```

---

## COUNT(DISTINCT)

`分子分母を出力するサンプル`で`COUNT(DISTINCT)`出来ない事を発見したが、いい感じのサンプルにまとめることが出来なかった。  

同じことを考えてる人がいたので、この人のサンプルを使わせてもらう。  
[Count distinct over partition by](https://stackoverflow.com/questions/66348498/count-distinct-over-partition-by)  

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
[Count distinct over partition by](https://stackoverflow.com/questions/66348498/count-distinct-over-partition-by)  

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

[Partition Function COUNT() OVER possible using DISTINCT](https://stackoverflow.com/questions/11202878/partition-function-count-over-possible-using-distinct)  
[PostgreSQLの分析関数の衝撃6（window関数の応用例）](https://codezine.jp/article/detail/4747)  
[OVERを指定したウィンドウ関数でDISTINCTを使用する](https://www.web-dev-qa-db-ja.com/ja/sql-server/over%E3%82%92%E6%8C%87%E5%AE%9A%E3%81%97%E3%81%9F%E3%82%A6%E3%82%A3%E3%83%B3%E3%83%89%E3%82%A6%E9%96%A2%E6%95%B0%E3%81%A7distinct%E3%82%92%E4%BD%BF%E7%94%A8%E3%81%99%E3%82%8B/l958295335/)  

---

## 特定の行の値を他の行でも使う

こういうデータを用意する。

``` sql
drop table if exists MainTable;
create table MainTable(MainKey varchar(32) primary key,SubKey varchar(32));
insert into MainTable values('Key001','AAA');
insert into MainTable values('Key002','DDD');

drop table if exists SubTable;
create table SubTable(MainKey varchar(32),SubKey varchar(32),TestNumber varchar(5) CONSTRAINT [PK_SubTable] PRIMARY KEY (MainKey,SubKey));
insert into SubTable values('Key001','AAA','0001');
insert into SubTable values('Key001','BBB','0002');
insert into SubTable values('Key002','CCC','0003');
insert into SubTable values('Key002','DDD','0004');
insert into SubTable values('Key002','EEE','0005');
```

こうやって表示させたい。

``` txt : 表示させたい結果
MainKey  SubKey Repre  TestNumber
Key001   AAA    0001   0001
Key001   AAA    0001   0002
Key002   DDD    0004   0003
Key002   DDD    0004   0004
Key002   DDD    0004   0005
```

肝はRepre列。  
MainKeyとSubKeyが一致した行のTestNumberを他の行でも表示させたい。  

一致した行だけを表示すると以下のようになってしまう。  
歯抜け部分をどのように補えばいいか悩んだ末に出来たのでまとめる。  

``` sql
SELECT
    [MainTable].[MainKey],
    [MainTable].[SubKey],
    CASE WHEN [MainTable].[SubKey] = [SubTable].[SubKey]
        THEN [SubTable].[TestNumber] 
        ELSE ''
    END AS [Repre],
    [SubTable].[TestNumber]
FROM [MainTable]
JOIN [SubTable]
ON [MainTable].[MainKey] = [SubTable].[MainKey]

-- MainKey  SubKey  Repre  TestNumber
-- Key001   AAA     0001   0001
-- Key001   AAA            0002
-- Key002   DDD            0003
-- Key002   DDD     0004   0004
-- Key002   DDD            0005
```

■**案1 FIRST_VALUE案**  

`FIRST_VALUE() OVER (PARTITION BY ORDER BY)`構文を使った方法。  

1. MainKeyとSubKeyでPARTITON BYする。  
2. ORDER BY は CASE文でMainKeyとSubKeyが一致する行を先頭とし、後は適当に並べて、DESCする。(この書き方が一番の肝)  
3. この地点で`0001`や`0004`が先頭に来ているので、それをFIRST_VALUEで回収する。  

``` sql
SELECT
    [MainTable].[MainKey],
    [MainTable].[SubKey],
    FIRST_VALUE([SubTable].[TestNumber]) OVER (
        PARTITION BY [MainTable].[MainKey],[MainTable].[SubKey]
        ORDER BY (
            CASE WHEN [MainTable].[SubKey] = [SubTable].[SubKey]
                THEN 1
                ELSE 0
            END
        ) DESC
    ) AS [Repre],
    [SubTable].[TestNumber]
FROM [MainTable]
JOIN [SubTable]
ON [MainTable].[MainKey] = [SubTable].[MainKey]
```

■**案2 MAX案**  

`MAX() OVER()`構文を使った方法。  

1. CASE文でSubKeyと一致した行のTestNumberを取得し、それ以外は空白とする。  
2. `MAX() OVER()`構文でCASE文のMAXを取得する。  
3. OVERの条件はPARTITION BYでメインキー2種が安定。  

``` sql
SELECT
    [MainTable].[MainKey],
    [MainTable].[SubKey],
    MAX(CASE WHEN [MainTable].[SubKey] = [SubTable].[SubKey] THEN [SubTable].[TestNumber] ELSE '' END) OVER (PARTITION BY [MainTable].[MainKey],[MainTable].[SubKey]) AS [Repre],
    [SubTable].[TestNumber]
FROM [MainTable]
JOIN [SubTable]
ON [MainTable].[MainKey] = [SubTable].[MainKey]
```

OrderByやPARTITION BYの条件次第では、他にも目的の結果になってくれる条件はあるが、キーで絞るのが安定だと思われる。  
SubTableのSub_Key等でOrderByしない限りは目的のデータになってくれる。  

``` sql
SELECT
    [MainTable].[MainKey],
    [MainTable].[SubKey],
    CASE WHEN [MainTable].[SubKey] = [SubTable].[SubKey]
        THEN [SubTable].[TestNumber] 
        ELSE ''
    END AS [Repre1],
    MAX(CASE WHEN [MainTable].[SubKey] = [SubTable].[SubKey]
        THEN [SubTable].[TestNumber] 
        ELSE ''
    END) OVER (ORDER BY [SubTable].[MainKey]) AS [ORDER_Main],
    MAX(CASE WHEN [MainTable].[SubKey] = [SubTable].[SubKey]
        THEN [SubTable].[TestNumber] 
        ELSE ''
    END) OVER (ORDER BY [SubTable].[SubKey]) AS [ORDER_Sub],
    MAX(CASE WHEN [MainTable].[SubKey] = [SubTable].[SubKey]
        THEN [SubTable].[TestNumber] 
        ELSE ''
    END) OVER (PARTITION BY [MainTable].[MainKey]) AS [PAR_Main],
    MAX(CASE WHEN [MainTable].[SubKey] = [SubTable].[SubKey]
        THEN [SubTable].[TestNumber] 
        ELSE ''
    END) OVER (PARTITION BY [MainTable].[SubKey]) AS [PAR_Sub],
    MAX(CASE WHEN [MainTable].[SubKey] = [SubTable].[SubKey]
        THEN [SubTable].[TestNumber] 
        ELSE ''
    END) OVER (PARTITION BY [MainTable].[MainKey],[MainTable].[SubKey]) AS [PAR_Keys],
    [SubTable].[TestNumber]
FROM [MainTable]
JOIN [SubTable]
ON [MainTable].[MainKey] = [SubTable].[MainKey]

-- MainKey  SubKey Repre  ORDER_Main  ORDER_Sub  PAR_Main  PAR_Sub  PAR_Keys  TestNumber
-- Key001   AAA    0001   0001        0001       0001      0001     0001      0001
-- Key001   AAA           0001        0001       0001      0001     0001      0002
-- Key002   DDD           0004        0001       0004      0004     0004      0003
-- Key002   DDD    0004   0004        0004       0004      0004     0004      0004
-- Key002   DDD           0004        0004       0004      0004     0004      0005
```

■**案3 自己結合**  

安直に実現するなら自分自身をJoinすればいける。  
ただ、2回も同じ情報を結合したくなかったので今回の検証をしたので、この回答は最低限である。  

``` sql
SELECT
    [MainTable].[MainKey],
    [MainTable].[SubKey],
    [Self].[TestNumber] AS [Repre],
    [SubTable].[TestNumber]
FROM [MainTable] 
JOIN [SubTable]
ON [MainTable].[MainKey] = [SubTable].[MainKey]
JOIN [SubTable] AS [Self]
ON [MainTable].[MainKey] = [Self].[MainKey]
AND [MainTable].[SubKey] = [Self].[SubKey]
```

■試行錯誤の跡

``` sql
SELECT
    ISNULL([MainTable].[MainKey],[SubTable].[MainKey]),
    ISNULL([MainTable].[SubKey],[SubTable].[SubKey]),
    FIRST_VALUE(CASE WHEN [MainTable].[SubKey] = [SubTable].[SubKey]
        THEN [SubTable].[TestNumber] 
        ELSE ''
    END) OVER (PARTITION BY [MainTable].[MainKey],[MainTable].[SubKey] ORDER BY [SubTable].[MainKey]) AS [Repre],
    [SubTable].[TestNumber]
FROM [SubTable]
LEFT JOIN [MainTable]
ON [SubTable].[MainKey] = [MainTable].[MainKey]
AND  [SubTable].[SubKey] = [MainTable].[SubKey]
```

---

## GROUP BY した結果の1件目を取得する

子テーブルから親テーブルを作る必要があるときに、コードで絞った結果の1件目だけを親テーブルにINSERTしたい状況に遭遇した時のまとめ。  

「group by 先頭1件」で検索。  
Rank() を使うらしい。  

[SELECT 文　GROUP　での1件目を取得](https://oshiete.goo.ne.jp/qa/3819843.html)  

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

--1 A
--2 F
--3 X
```

---

## 最大/最小の値のレコードを取得する

`GROUP BY した結果の1件目を取得する`の亜種になるだろうか。  
Window関数を知ってからは、そんなのWindow関数を使えば一発じゃんって思う。  
愚直にサブクエリでMAXかMINで抽出して、その中でさらに条件を絞って、最終的に出力するみたいな感じになるみたい。  
またはEXITSのような相関副問い合わせやJEFT JOIN を組み合わせるやり方など、やり方はたくさんあるみたい。  

全面的にここの記事を採用しつつアレンジしている。  
[SQLで同一グループの中で最大/最小のレコードを取得する](https://qiita.com/inouet/items/4f1d7f299725597d8407)  

他参考の方々
[SQL ある列の値が最大もしくは最小の値のレコードを取得する](https://zukucode.com/2017/08/sql-row_number-technique.html)  
[特定のカラムのグループごとの最大値が格納されている行](https://dev.mysql.com/doc/refman/5.6/ja/example-maximum-column-group-row.html)  

``` sql : データ準備
create table items (
  id int not null primary key,
  name varchar(100),
  price int,
  category varchar(100)
);

insert into items values(1, 'りんご', 190, 'くだもの');
insert into items values(2, 'みかん', 100, 'くだもの');
insert into items values(3, 'きゅうり', 80, '野菜');
insert into items values(4, '人参', 110, '野菜');
insert into items values(5, 'キャベツ', 110, '野菜');
insert into items values(6, '豚肉', 300, '肉');
insert into items values(7, '牛肉', 400, '肉');
```

こうやって表示させたい。

``` txt : 表示させたい結果
id  name    price  category
1   りんご  190    くだもの
7   牛肉    400    肉    
4   人参    110    野菜  
```

■**window関数案**

一番最初に自分が思いついた案。  
categoryで区切って、priceをDescしてidは早いほうを採用。  
サブクエリからrk = 1のモノを取得すればいける。  

``` sql : window関数を使った例
SELECT
    *
FROM (
    SELECT
        *,
        ROW_NUMBER() OVER(PARTITION BY category ORDER BY price DESC,id) rk
    FROM
        [items]
) AS [A]
WHERE
    [A].[rk] = 1
```

■**LEFT OUTER案**  

自分自身より金額が低いものをJoinする。  
若しくは同じ金額でidが大きいモノをJoinする。  
そうすると金額が高くてidが小さいレコードはNULLとなる。  
WHEREでNULLのレコードを取得すれば結果となる。  

``` sql : left join
SELECT s1.*,s2.*
FROM [items] s1
LEFT JOIN [items] s2 
ON s1.category = s2.category
AND (s1.price < s2.price OR (s1.price = s2.price AND s1.id > s2.id))
WHERE s2.id IS NULL
```

---

## 重複レコードの対処

[重複データの抽出／削除をウィンドウ関数で](https://kenpg.bitbucket.io/blog/201607/06.html)  

# Window関数

[分析関数（ウインドウ関数）をわかりやすく説明してみた](https://qiita.com/tlokweng/items/fc13dc30cc1aa28231c5)  

---

## ROW_NUMBER

連番を割り振る関数。  
連番の順番と区分ごとに連番を生成できるらしい。  

[SQL Fiddle](http://sqlfiddle.com/#!18/7374f/71)  
[よく使われる順位付け関数 1 - ROW_NUMBER](https://sql55.com/t-sql/sql-server-built-in-ranking-function-1.php)  
<https://style.potepan.com/articles/23566.html>  

``` SQL : 基本
ROW_NUMBER () OVER ( [ PARTITION BY [パティションカラム 1 ], [パティションカラム 2], ... ] ORDER BY [ソートカラム 1], [ソートカラム 2], ...  )
```

``` SQL
-- こんな感じで使うらしいよ。
select ROW_NUMBER() OVER (PARTITION BY Code ORDER BY UpdateDateTime DESC), * from [Table]

-- 関数 'ROW_NUMBER' には ORDER BY 句を伴う OVER 句が必要です。
-- こうやって怒られるので、最低限、ORDER BY は必要見たい。
select ROW_NUMBER() OVER (), * from [Table]
```

---

## PARTITION BY

[SQL PARTITION BYの基本と効率的に集計する便利な方法](https://zukucode.com/2017/08/sql-over-partition-by.html)  

``` txt : employee（社員）
id  first_name  last_name  department_id  height
1   一郎         山田       1              170
2   次郎         佐藤       2              175
3   三郎         田中       1              185
4   四郎         鈴木       2              155
```

``` sql
SELECT
  last_name,
  --全体の総件数
  COUNT(1) OVER() total_count,
  --部門ごとの件数
  COUNT(1) OVER(PARTITION BY department_id) section_count,
  --部門ごとの最大身長
  MAX(height) OVER(PARTITION BY department_id) section_max_height,
  --部門ごとの身長順（身長順に並び替えたときの行番号）
  ROW_NUMBER() OVER(PARTITION BY department_id ORDER BY height DESC) section_height_order,
  --全体の身長順（身長順に並び替えたときの行番号）
  ROW_NUMBER() OVER(ORDER BY height DESC) height_order
FROM
  employee
ORDER BY
  id
```

``` txt : 取得結果
last_name  total_count  section_count  section_max_height  section_height_order  height_order
山田        4            2              185                 2                     3
佐藤        4            2              175                 1                     2
田中        4            2              185                 1                     1
鈴木        4            2              175                 2                     4
```

---

## 分子分母を出力するサンプル

Window関数を初めて知ってから組み上げた個人的な実践サンプル。  

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

## COUNT(DISTINCT)は出来ない

時間の中に複数の予約があったら、「+」を表示しようとして、COUNT(DISTINCT GroupNo)なんてしようとしたけど、出来なくてなんで？ってなった気がする。  
だけど、時間の中に複数のグループがいたらって判定は普通にCOUNT() OVER()で行けた。  
まぁ、黒魔術みたいな呪文で似たようなことで切ることを発見したので、それはそれでまとめる。  

``` sql
SELECT
    [ReservationTime],
    [ReservationNo],
    CONVERT(nvarchar,ROW_NUMBER() OVER(PARTITION BY [ReservationNo] ORDER BY [ReservationTime]))
    + '/' 
    + CONVERT(nvarchar,COUNT(1) OVER(PARTITION BY [ReservationNo]))
    -- 黒魔術
    + CASE WHEN (
        DENSE_RANK() OVER (PARTITION BY [ReservationTime] ORDER BY [ReservationNo]) 
        + DENSE_RANK() OVER (PARTITION BY [ReservationTime] ORDER BY [ReservationNo] DESC) 
        - 1
        ) > 1 
        THEN '+'
        ELSE '' 
    END
    AS [Result]
FROM [TestReservation]
GROUP BY [ReservationTime],[ReservationNo]

-- ReservationTime   ReservationNo    Result
-- 07:00:00.0000000  RES202204160001  1/4   
-- 07:07:00.0000000  RES202204160001  2/4   
-- 07:14:00.0000000  RES202204160001  3/4   
-- 07:21:00.0000000  RES202204160001  4/4   
-- 07:28:00.0000000  RES202204160002  1/2   
-- 07:35:00.0000000  RES202204160002  2/2   
-- 07:42:00.0000000  RES202204160003  1/2+  
-- 07:42:00.0000000  RES202204160004  1/1+  
-- 07:49:00.0000000  RES202204160003  2/2   
-- 07:56:00.0000000  RES202204160005  1/1   
```

[Partition Function COUNT() OVER possible using DISTINCT](https://stackoverflow.com/questions/11202878/partition-function-count-over-possible-using-distinct)  

---

## FIRST_VALUEサンプル

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

■**案1 FIRST_VALUE OVER**  

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

■**案2 MAX OVER**  

`MAX() OVER()`構文を使った方法。  

1. CASE文でSubKeyと一致した行のTestNumberを取得し、それ以外は空白とする。  
2. `MAX() OVER()`構文でCASE文のMAXを取得する。  
3. OVERの条件はPARTITION BYでメインキー2種が安定。  

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

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

OVER PARTITIONを使った実践的なサンプル。  
プレーヤーが複数の時間に跨いで予約を取っている状況において、その時間にとってその人が何番目なのかを分母分子で表示するサンプル  

``` sql : データ準備
CREATE TABLE TestReservation
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,  
    [ReservationTime] TIME,  
    [SeatNo] INT,
    [PlayerNo] nvarchar(255),
    [GroupNo] INT
);

INSERT INTO TestReservation
VALUES
     ('07:00:00.0000000',1,'Player202204160001',1)
    ,('07:00:00.0000000',2,'Player202204160002',1)
    ,('07:00:00.0000000',3,'Player202204160003',2)
    ,('07:00:00.0000000',4,'Player202204160004',2)
    ,('07:07:00.0000000',1,'Player202204160001',1)
    ,('07:07:00.0000000',2,'Player202204160002',1)
    ,('07:14:00.0000000',1,'Player202204160001',1)
    ,('07:14:00.0000000',2,'Player202204160004',2)
    ,('07:14:00.0000000',3,'Player202204160005',3)
```

``` sql
SELECT
    [ReservationTime],
    [PlayerNo],
    -- 時間毎のプレーヤー順(分子)
    ROW_NUMBER() OVER(PARTITION BY [ReservationTime] ORDER BY [PlayerNo]) AS [Numerator],
    -- 時間毎の件数(分母)
    COUNT(1) OVER(PARTITION BY [ReservationTime]) AS [Denominator]
FROM [TestReservation]


-- 07:00:00.0000000    Player202204160001    1    4
-- 07:00:00.0000000    Player202204160002    2    4
-- 07:00:00.0000000    Player202204160003    3    4
-- 07:00:00.0000000    Player202204160004    4    4
-- 07:07:00.0000000    Player202204160001    1    2
-- 07:07:00.0000000    Player202204160002    2    2
-- 07:14:00.0000000    Player202204160001    1    3
-- 07:14:00.0000000    Player202204160004    2    3
-- 07:14:00.0000000    Player202204160005    3    3
```

[Partition Function COUNT() OVER possible using DISTINCT](https://stackoverflow.com/questions/11202878/partition-function-count-over-possible-using-distinct)  

``` sql
-- COUNT(DISTINCT GroupNo)が出来ない。時間の中に複数のグループがいることを確認したいだけなのに。

SELECT
    [ReservationTime],
    [PlayerNo],
    -- 時間毎のプレーヤー順(分子)
    ROW_NUMBER() OVER(PARTITION BY [ReservationTime] ORDER BY [PlayerNo]) AS [Numerator],
    -- 時間毎の件数(分母)
    COUNT(1) OVER(PARTITION BY [ReservationTime]) AS [Denominator],

    else 0 end) over (order by GroupNo)
    --ROW_NUMBER() OVER(PARTITION BY [ReservationTime] ORDER BY [PlayerNo])
    --+ '/' 
    --+ COUNT(1) OVER(PARTITION BY [ReservationTime])
    --+ CASE WHEN COUNT(GroupNo) OVER(PARTITION BY [ReservationTime]) > 1 THEN '+' ELSE '' END  AS [FrameCount]
FROM [TestReservation]


SELECT
    [ReservationTime],
    [PlayerNo],
    -- 時間毎のプレーヤー順(分子)
    ROW_NUMBER() OVER(PARTITION BY [ReservationTime] ORDER BY [PlayerNo]) AS [Numerator],
    -- 時間毎の件数(分母)
    COUNT(1) OVER(PARTITION BY [ReservationTime]) AS [Denominator],
    -- その時間にどれくらいの
    dense_rank() over (partition by ReservationTime order by GroupNo) + dense_rank() over (partition by ReservationTime order by GroupNo desc) - 1,
    CONVERT(nvarchar,ROW_NUMBER() OVER(PARTITION BY [ReservationTime] ORDER BY [PlayerNo]))
    + '/' 
    + CONVERT(nvarchar,COUNT(1) OVER(PARTITION BY [ReservationTime]))
    + CASE WHEN (dense_rank() over (partition by ReservationTime order by GroupNo) + dense_rank() over (partition by ReservationTime order by GroupNo desc) - 1) > 1 THEN '+' ELSE '' END  AS [FrameCount]
FROM [TestReservation]

ORDER BY ReservationTime,PlayerNo


-- ReservationTime     PlayerNo   Numerator  Denominator  FrameByGroupCount    FrameCount
-- 07:00:00.0000000    Player202204160001    1    4    2                         1/4+
-- 07:00:00.0000000    Player202204160002    2    4    2                         2/4+
-- 07:00:00.0000000    Player202204160003    3    4    2                         3/4+
-- 07:00:00.0000000    Player202204160004    4    4    2                         4/4+
-- 07:07:00.0000000    Player202204160001    1    2    1                         1/2
-- 07:07:00.0000000    Player202204160002    2    2    1                         2/2
-- 07:14:00.0000000    Player202204160001    1    3    3                         1/3+
-- 07:14:00.0000000    Player202204160004    2    3    3                         2/3+
-- 07:14:00.0000000    Player202204160005    3    3    3                         3/3+
```

``` sql : 超愚直にやるならこう
SELECT
    [SQ_Numerator].[ReservationTime],
    [SQ_Numerator].[PlayerNo],
    CONVERT(nvarchar,[SQ_Numerator].[Numerator]) AS [Numerator],
    CONVERT(nvarchar,[SQ_Denominator].[Denominator]) AS [Denominator]
FROM (
    -- 時間を上から見ていった時のシーケンス(分子:Numerator)
    SELECT
        [ReservationTime],
        [PlayerNo],
        ROW_NUMBER() OVER(PARTITION BY [PlayerNo] ORDER BY [ReservationTime]) AS Numerator
    FROM 
        [TestReservation]
    GROUP BY
        [ReservationTime],[PlayerNo]
    ) AS [SQ_Numerator]
JOIN (
    -- プレーヤーが持っている時間の数(分母:Denominator)
    SELECT
        [PlayerNo],
        COUNT(DISTINCT [ReservationTime]) AS [Denominator]
    FROM 
        [TestReservation]
    GROUP BY 
        [PlayerNo]
) AS [SQ_Denominator]
ON [SQ_Numerator].[PlayerNo] = [SQ_Denominator].[PlayerNo]
```

---

## FIRST_VALUEサンプル

MainTableとSubTableのような親子関係のあるテーブルがあるとする。  
MainKeyだけでJoinしたとして、そのMainKeyに対するSubKeyが持つ、TestNumberを代表(Repre)として特定して表示させたい という課題に対応するとき、Window関数とFIRST_VALUEを用いてうまいことできたのでまとめる。

後日、後で気が付いたが、これでうまく行ったのは、順番の列が定義されていたからだ。  

``` txt : MainTable
MainKey  SubKey
Key001   AAA
Key002   DDD
```

``` txt : SubTable
MainKey  SubKey  TestNumber  OrderNumber
Key001   AAA     0001        1
Key001   BBB     0002        2
Key002   CCC     0003        2
Key002   DDD     0004        1
Key002   EEE     0005        3
```

``` txt : 表示させたい結果
MainKey  SubKey Repre  TestNumber
Key001   AAA    0001   0001
Key001   AAA    0001   0002
Key002   DDD    0004   0003
Key002   DDD    0004   0004
Key002   DDD    0004   0005
```

MainKeyとSubKeyでグループ化すしてOrderNumberでOrderByする。  
目的のTestNumberが一番上にある状態なので、FIRST_VALUEで先頭のTestNumberを取得する。  

``` sql
SELECT
    [MainTable].[MainKey],
    [MainTable].[SubKey],
    FIRST_VALUE([SubTable].[TestNumber]) OVER (PARTITION BY [MainTable].[MainKey],[MainTable].[SubKey] ORDER BY [SubTable].[OrderNumber]) AS [Repre],
    [SubTable].[TestNumber]
FROM [MainTable]
JOIN [SubTable]
ON [MainTable].[MainKey] = [SubTable].[MainKey]
```

愚直にやるなら自分自身をJoinすればいける。  

``` sql
SELECT
    [MainTable].[MainKey],
    [MainTable].[SubKey],
    [Self].[TestNumber] AS [Repre],
    [SubTable].[TestNumber]
FROM [MainTable] 
JOIN [SubTable]
ON [MainTable].[MainKey] = [SubTable].[MainKey]
-- 自分自身を各々のキーでJoinすれば、代表となるNumberを特定できる
JOIN [SubTable] AS [Self]
ON [MainTable].[MainKey] = [Self].[MainKey]
AND [MainTable].[SubKey] = [Self].[SubKey]
```

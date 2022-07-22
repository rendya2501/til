# ROW_NUMBER()

[SQL Fiddle](http://sqlfiddle.com/#!18/7374f/71)  
[よく使われる順位付け関数 1 - ROW_NUMBER](https://sql55.com/t-sql/sql-server-built-in-ranking-function-1.php)  
<https://style.potepan.com/articles/23566.html>  
連番を割り振る関数。連番の順番と区分ごとに連番を生成できるらしい。  

``` SQL
ROW_NUMBER ( ) OVER ( [ PARTITION BY [パティションカラム 1 ], [パティションカラム 2], ... ] ORDER BY [ソートカラム 1], [ソートカラム 2], ...  )
```

``` SQL
-- こんな感じで使うらしいよ。
select ROW_NUMBER() OVER (PARTITION BY Code ORDER BY UpdateDateTime DESC), * from [Table]

-- 関数 'ROW_NUMBER' には ORDER BY 句を伴う OVER 句が必要です。
-- こうやって怒られるので、最低限、ORDER BY は必要見たい。
select ROW_NUMBER() OVER (), * from [Table]
```

---

## PARTITION BYの組み合わせ

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

プレーヤーが複数の時間に跨いで予約を取っている状況において、その人にとってその時間が何番目なのかを分母分子で表示するサンプル  

``` sql : データ準備
CREATE TABLE TestReservation
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,  
    [ReservationTime] time,  
    [PlayerNo] nvarchar(255)
);

INSERT INTO TestReservation
VALUES
     ('07:00:00.0000000','Player202204160001')
    ,('07:07:00.0000000','Player202204160001')
    ,('07:14:00.0000000','Player202204160001')
    ,('07:00:00.0000000','Player202204160002')
    ,('07:07:00.0000000','Player202204160002')
    ,('07:00:00.0000000','Player202204160003')
    ,('07:00:00.0000000','Player202204160004')
    ,('07:14:00.0000000','Player202204160004')
```

``` sql
SELECT
    [ReservationTime],
    [PlayerNo],
    -- プレーヤー毎の時間順(分子)
    ROW_NUMBER() OVER(PARTITION BY [PlayerNo] ORDER BY [ReservationTime]) AS [Numerator],
    -- プレーヤー毎の件数(分母)
    COUNT(1) OVER(PARTITION BY [PlayerNo]) AS [Denominator]
FROM [TestReservation]

-- 07:00:00.0000000    Player202204160001    1    3
-- 07:07:00.0000000    Player202204160001    2    3
-- 07:14:00.0000000    Player202204160001    3    3
-- 07:00:00.0000000    Player202204160002    1    2
-- 07:07:00.0000000    Player202204160002    2    2
-- 07:00:00.0000000    Player202204160003    1    1
-- 07:00:00.0000000    Player202204160004    1    2
-- 07:14:00.0000000    Player202204160004    2    2
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

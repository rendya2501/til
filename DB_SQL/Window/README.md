# Window関数

## 概要

何らかの条件で並んでいるデータに対して、特定の範囲のデータのみ参照できるようにする機能が用意されており、これをWindowと呼ぶ。  
このWindowに対して、最大、最小、合計、平均などの集計を行う関数が用意されており、これらのことをWindow関数と呼ぶ。  

- 主要なデータベース(PostgreSQL,MySQL,SQLite,SQLServer,Oracle)には Window関数が備わっている。  
- select 文で返される1行1行について、Window関数が実行される。  
- Window関数は、必ずOVERキーワードと一緒に使う。  
  - `Window関数() OVER()`  
- OVER() のカッコの中に、Partition by や Order by を指定することで、グルーピング（＝パーティション）した中身をソートし、それぞれに対してWindows関数が実行される。  

---

## Window関数の基本構文

``` SQL
Window関数 () OVER ( PARTITION BY [カラム 1 ], [カラム 2], ... ORDER BY [カラム 1], [カラム 2], ...)
```

---

## PARTITION BY によるグルーピング

OVER の中に Partition by を書くことで、指定した項目でグループ化することが可能  

`Window関数() OVER (PARTITION BY カラム1,カラム2, ...)`

---

## Window関数早見表

``` txt
AVG(カラム名)
指定したカラムの平均を求める。

COUNT(* or カラム名)
件数 を求める。

MAX( カラム名 )
指定したカラムの 最大 を求める。

MIN( カラム名 )
指定したカラムの 最小 を求める。

SUM( カラム名 )
指定したカラムの 合計 を求める。

LAG( カラム名 , [オフセット] , [初期値])
指定したカラムにおいて、前の行の値を取得する。

[オフセット]
さかのぼる行数。省略した場合は1

[初期値]
値が存在しなかった場合に返される値。
省略時はNULL

LEAD( カラム名 , [ オフセット ] , [ 初期値 ])
指定したカラムにおいて、次の行の値を取得する。
[オフセット]
  先を見る行数。省略した場合は1
[初期値]
  値が存在しなかった場合に返される値。
  省略時はNULL

FIRST_VALUE( カラム名 )
最初の行の値を取得する。

LAST_VALUE( カラム名 )
最後の行の値を取得する。

ROW_NUMBER()
ソート順で1からの行番号を取得する

RANK()
ソート順でランク付け（1からの連番）を取得する。
同じ値がある場合、ランクは飛び番になる。
例：1,2,3,3,5,5,5,8

DENSE_RANK()
ソート順でランク付け（1からの連番）を取得する。
同じ値がある場合、ランクは飛ばない。
例：1,2,3,3,4,5,5,5,6
```

---

## ROW_NUMBER

連番を割り振る関数。  
連番の順番と区分ごとに連番を生成できる。  

``` SQL
-- 関数 'ROW_NUMBER' には ORDER BY 句を伴う OVER 句が必要です。
-- こうやって怒られるので、最低限、ORDER BY は必要見たい。
select ROW_NUMBER() OVER (), * from [Table]
```

[SQL Fiddle](http://sqlfiddle.com/#!18/7374f/71)  
[よく使われる順位付け関数 1 - ROW_NUMBER](https://sql55.com/t-sql/sql-server-built-in-ranking-function-1.php)  
<https://style.potepan.com/articles/23566.html>  

---

## パフォーマンスに注意

順位付け関数(ROW_NUMBER等)は、ORDER BY句を指定していることからも分かるように、内部的には並べ替え処理が行われている。  
また、PARTITION BY句を利用した場合は、内部的にはグループ化処理(GROUP BY演算とほとんど同じ処理)が行われる。  
これらは、データべース サーバーによっては、非常に不可の高い処理(特にメモリとディスクへの高負荷、グループ化で指定する列が多い場合にはCPUへも高負荷)なので、使いすぎに注意する必要がある。  

基本的には、必要な場合のみ、最低限の場所でのみ利用すること。  

[SQLServer 2016の教科書]より  

---

## WINDOW句

OVER句の()括弧内の記述に対して、エイリアスを設定することができる機能。  
長ったらしい記述を使いまわすことができるので便利。  

``` sql
-- ノーマル
SELECT 
  a,b, 
  ROW_NUMBER() OVER (PARTITION BY a ORDER BY a,b) AS A
FROM Test

-- WINDOW句
SELECT 
  a,b, 
  ROW_NUMBER() OVER PART AS A
FROM Test
WINDOW PART AS (PARTITION BY a ORDER BY a,b)
```

[分析関数（OVER句,WINDOW句）｜SQL入門](https://excel-ubara.com/vba_sql/vba_SQL023.html)  

## FILTER句

OVER句の前にFILTER句を記述することで、条件を満たしたレコードのみで集計処理を行うもの。  

`分析関数 FILTER(WHERE 条件式) OVER(・・・)`  

PostgreSQLでは以下のように記述可能。  

``` sql
WITH CTE AS (
    SELECT a, b
    FROM (
        VALUES
            (1, 1), (1, 2), (1, 3),
            (2, 1), (2, 2), (2, 3), (2, 4)
    ) AS t(a, b)
)
SELECT
    a, b,
    string_agg(b::text, ',') FILTER (WHERE b != 2) OVER (PARTITION BY a ORDER BY a,b) as [filter]
    string_agg(b::text, ',') OVER (PARTITION BY a ORDER BY a,b) as [not_filter]
FROM test;
/*
  a | b | filter |not_filter 
 ---+---+--------+-----------
  1 | 1 | 1      | 1         
  1 | 2 | 1      | 1,2       
  1 | 3 | 1,3    | 1,2,3     
  2 | 1 | 1      | 1         
  2 | 2 | 1      | 1,2       
  2 | 3 | 1,3    | 1,2,3     
  2 | 4 | 1,3,4  | 1,2,3,4   
*/
```

SQLServerで同じようにやるならこうなる。

``` sql
WITH CTE AS (
    SELECT a, b
    FROM (
        VALUES
            (1, 1), (1, 2), (1, 3),
            (2, 1), (2, 2), (2, 3), (2, 4)
    ) AS t(a, b)
)
SELECT DISTINCT
    CTE.a,
    CTE.b,
    STUFF((
        SELECT ',' + CAST(CTE2.b AS VARCHAR(MAX))
        FROM CTE AS CTE2
        WHERE CTE2.a = CTE.a AND CTE2.b <= CTE.b AND CTE2.b <> 2
        FOR XML PATH('')
    ), 1, 1, '') AS [filter],
    STUFF((
        SELECT ',' + CAST(CTE2.b AS VARCHAR(MAX))
        FROM CTE AS CTE2
        WHERE CTE2.a = CTE.a AND CTE2.b <= CTE.b
        FOR XML PATH('')
    ), 1, 1, '') AS [not_filter]
FROM CTE
ORDER BY a, b
/*
  a | b | filter |not_filter 
 ---+---+--------+-----------
  1 | 1 | 1      | 1         
  1 | 2 | 1      | 1,2       
  1 | 3 | 1,3    | 1,2,3     
  2 | 1 | 1      | 1         
  2 | 2 | 1      | 1,2       
  2 | 3 | 1,3    | 1,2,3     
  2 | 4 | 1,3,4  | 1,2,3,4   
*/
```

[分析関数（OVER句,WINDOW句）｜SQL入門](https://excel-ubara.com/vba_sql/vba_SQL023.html)  

---

## WINDOWS句とFILTER句のサポート状況

ChatGPTに聞いてみた結果。　　

|データベース|Window句|Filter句|備考|
|---|---|---|---|
|PostgreSQL|○|○||
|MariaDB|○|○||
|IBM Db2|○|○||
|Oracle Database|○|×|Filter句の代わりにCASE式を使用可能|
|SQLServer|×|×||

---

[分析関数（ウインドウ関数）をわかりやすく説明してみた](https://qiita.com/tlokweng/items/fc13dc30cc1aa28231c5)  
[【ひたすら図で説明】一番やさしい SQL window 関数（分析関数） の使い方](https://resanaplaza.com/2021/10/17/%E3%80%90%E3%81%B2%E3%81%9F%E3%81%99%E3%82%89%E5%9B%B3%E3%81%A7%E8%AA%AC%E6%98%8E%E3%80%91%E4%B8%80%E7%95%AA%E3%82%84%E3%81%95%E3%81%97%E3%81%84-sql-window-%E9%96%A2%E6%95%B0%EF%BC%88%E5%88%86/)  

[【SQLの神髄】第５回 PARTITIONとROWS BETWEENを使ったレコード間比較](https://homegrowin.jp/all/4320/)  

[Window関数のFILTER句を極める](https://masahikosawada.github.io/2018/12/17/Window-Function-Filter-Clause/)  

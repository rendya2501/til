# SQL基本

## INSERT

<https://itsakura.com/sql-insert>  

大別して2パターンの組み合わせがわかっていればよい。  

- 列名指定あり・なし  
- VALUESかSELECTか  

``` sql : 基本
-- ●列名指定なし + VALUES
INSERT INTO テーブル名 VALUES ( '値1' [ , '値2' ]・・・);

-- ●列名指定あり + VALUES
INSERT INTO テーブル名 ( テーブルの列名1 [ , テーブルの列名2 ]・・・) VALUES ( '値1' [ , '値2' ]・・・);

-- ●列名指定なし + SELECT (selectの結果をinsertする)
INSERT INTO テーブル名 SELECT 項目名 FROM テーブル名

-- ●列名指定あり + SELECT (selectの指定した結果だけをinsertする)
INSERT INTO テーブル名 ( [テーブルの列名1], [テーブルの列名2]... ) SELECT [項目名1],[項目名2]... FROM 別テーブル名
```

``` sql : SELECTした結果をINSERTできたよなぁーと思ってやったら一発で行けたので備忘録として載せておく。
INSERT INTO Round3SysC3.dbo.TSm_ReportFileSetting
(WindowName,TemplateName,ReportName,ValidFlag,Sort,ApiUri,Remarks)
SELECT WindowName,TemplateName,ReportName,ValidFlag,Sort,ApiUri,Remarks
FROM Round3Sys_Test.dbo.TSm_ReportFileSetting
WHERE WindowName = 'RN3.Wpf.Front.CheckOut.Views.CheckOutWindow'
AND TemplateName = 'RN3.Wpf.Front.CheckOut.SettlementH.rdlx'
```

### 列名指定 + VALUESタイプ で列名をあべこべに設定したらどうなるか

`INSERT INTO テーブル名 ([列1],[列2]) VALUES ('値1' AS [列2], '値2' AS [列1])`  

A. 構文エラーになる。  
VALUES句の中でAS句は使えない模様。  

実行結果  
`INSERT INTO TMa_Route2 (OfficeCD,RouteCD) VALUES ('ALP' AS [AAA],100 AS [BBB])`  
→  
`メッセージ 156、レベル 15、状態 1、行 5 キーワード 'AS' 付近に不適切な構文があります。`  

### 列名指定 + SELECTタイプ で列名に対する別名をあべこべに定義した場合

・構文エラーになることはないが、別名は関係なく、左から順番通りに入る。  
・定義していない列にはデフォルト値が入る  

`INSERT INTO TMa_Route2 (OfficeCD,RouteCD) SELECT 'ALP' AS [AAA],100 AS [BBB]`  
→  

``` txt
OfficeCD    RouteCD    RouteName    RouteShortName    ClsCD    Color    BackgroundColor    Sort    ValidFlag    SearchKey    InsertDateTime    InsertStaffCD    InsertStaffName    InsertProgram    InsertTerminal    UpdateDateTime    UpdateStaffCD    UpdateStaffName    UpdateProgram    UpdateTerminal
ALP    100    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL
```

### 列名指定なし + VALUESタイプ では列の数が一致しないとエラーになる

`INSERT INTO TMa_Route2 VALUES ('ALP',102)`  
→  
`メッセージ 213、レベル 16、状態 1、行 5 列名または指定された値の数がテーブルの定義と一致しません。`  

列名を指定しない場合は、列数と同じデータを用意しないとエラーになる。  

### 列名指定 + VALUESタイプ は列数とVALUESの列数が一致している必要がある

`INSERT INTO TMa_Route2 (OfficeCD,RouteCD) VALUES ('ALP',100,22)`  
→  
`メッセージ 110、レベル 15、状態 1、行 5`  
`VALUES 句で指定された値よりも INSERT ステートメントの列数が少なすぎます。VALUES 句の値の数は、INSERT ステートメントで指定される列数と一致させてください。`  

---

## UPDATE

<https://qiita.com/ryota_i/items/d17c7630bacb36d26864>  
>特定テーブルにおける、条件に当てはまるレコードの特定のカラムの値を任意の値に書き換える。  

``` sql : 基本
UPDATE テーブル名
SET 列名1 = 値1 [,列名2 = 値2]・・・
WHERE (条件);
```

### 副問い合わせを使ったUPDATE

``` sql : 副問い合わせ
-- ●UPDATE文のset句で副問合せを使用する
UPDATE syain
SET name = (
    select name
    from test
    where id = 2)
WHERE id = 2;

-- テーブルの値を別テーブルの値でUPDATEする(其の弐)
UPDATE
    [Round3Dat_Test].[dbo].[TMa_ProductCls]
SET
    [Round3Dat_Test].[dbo].[TMa_ProductCls].[DepartmentCD] = (
        SELECT
            [DepartmentCD]
        FROM
            [Round3Dat_20210205].[dbo].[TMa_ProductCls]
        WHERE
            [Round3Dat_Test].[dbo].[TMa_ProductCls].[ProductClsCD] = [Round3Dat_20210205].[dbo].[TMa_ProductCls].[ProductClsCD]
    )

-- ●UPDATE文のwhere句で副問合せを使用する(where in)
-- テーブルの更新条件を副問い合わせで取ってくる
-- https://sqlazure.jp/r/sql-server/403/
UPDATE syain
SET name = 'テスト'
WHERE id in (
    select id from test
    where name = 'TEST'
);
```

### SQLServerにおけるUPDATE JOIN

**SQLServerでJoinしつつUpdateするならパターン1のように書く必要がある**。  
肝はUPDATE文に指定するテーブル名は別名でなければいけないということ。  

後はFROM JOIN WHERE の流れは普通のSELECT文と同じ。  
UPDATEしようとしているテーブルの情報をサブクエリに使うこともできたのね。

[UPDATE と JOIN を使ってデータを更新する](https://sql55.com/t-sql/t-sql-update-1.php)  
[SQL Serverで、SELECT結果でUPDATEする方法](https://sqlazure.jp/r/sql-server/403/)  

``` sql : パターン1
UPDATE
    [Alias]
SET
    [Alias].col1 = [other_table].col1,
    [Alias].col2 = [other_table].col2
FROM
    [Alias] AS [Alias]
    JOIN [other_table]
    ON [Alias].id = [other_table].id
```

[SQLServer にて他のテーブルのSELECT結果を利用したUPDATE](https://pg.4696.info/db/mssql/sqlserver-sql.html)  
別名でなくてもいける？  
もう少し深堀する必要がありけり。  

``` sql :
UPDATE [会員テーブル]
SET 
    [とあるコード]= sub1.[とあるコード]
    ,[更新日時] = SYSDATETIME ()
FROM 
    [会員テーブル]
    inner join 
    (
        select
            e1.ID
            ,e1.[とあるコード]
        from
            [エントリーテーブル] as e1
        inner join 
        (
            select 
                ID
                ,MAX([登録日時]) as '最大登録日時'
            from
                [エントリーテーブル]
            where
                [とあるコード] is not null
                and [とあるコード] != '0'
            group by
                ID
        ) AS e2
        ON 
            e1.ID = e2.ID
            AND e1.[登録日時] = e2.'最大登録日時'
    ) AS sub1
    ON 
        [会員テーブル].ID = sub1.ID
where 
    [会員テーブル].[とあるコード] = '';
```

### UPDATE JOIN の他のパターン

SQLServerではパターン2,パターン3の形式は使えない。  

``` sql : パターン2
UPDATE
    テーブル名1 
    INNER JOIN テーブル名2 
    ON テーブル名1.列名X = テーブル名2.列名X
SET
    テーブル名1.列名1 = テーブル名2.列名2;
```

``` sql : パターン3
-- パターン2と同じ意味
UPDATE
    テーブル名1,テーブル名2
SET
    テーブル名1.列名1 = テーブル名2.列名2
WHERE
    テーブル名1.列名X = テーブル名2.列名X;
```

### UPDATEでCASE式

``` sql : UPDATE文でCASE式を使用する
UPDATE
    syain
SET
    name = case id
        when 1 then '鈴木一郎'
        when 2 then '田中二郎'
        when 3 then '佐藤三郎'
    end,
    romaji = case
        when id > 1 then 'tokumei'
        ELSE 'i.suzuki'
    end
WHERE
    id in(1, 2, 3)
```

### UPDATEでWITH句を使う

[SQLでwith句とupdateを使う方法 サブクエリを共通テーブル式で置き換えるサンプルコード](https://style.potepan.com/articles/30390.html)  

OrderByして最初のレコードだけを更新対象としたいときに、どうしても一回 SELECT FROM 別名 WHERE Seq = 1 で取らないと行けなかった。  
ならサブクエリの部分をWITHに書けば、わざわざ一段階SELECTを挟まないで直接UPDATEまでできるのではないかと考えた。  

結果的に出来たが、移行クエリでやることなので、パット見わかりにくいかなと思ったのと、SELECTで一回取得しても大した量ではなかったのでやめておいた。  
しかし、出来た事に変わりはないので備忘録として残す。  

Withのほうがインデントが浅くなる。  
それだけかもしれない。  
普通に書いたほうが意味も通じる。

``` sql : 実務でうまくいったパターン
WITH SQ AS
(
    SELECT
        ROW_NUMBER() OVER(PARTITION BY [Reserve].[ReservationNo] ORDER BY [Player].[PlayerNo]) AS Seq,
        [Player].*
    FROM
        TRe_Reservation AS [Reserve]
        JOIN TRe_ReservationPlayer AS [Player]
        ON [Reserve].[ReservationNo] = [Player].[ReservationNo]
        AND [Reserve].[RepreCustomerCD] = [Player].[CustomerCD]
        AND ISNULL([Reserve].[ReservationCancelFlag],1) = 0
        AND REPLACE(REPLACE(ISNULL([Reserve].[RepreCustomerCD],''),' ',''),'　','') <> ''
)
UPDATE SQ
SET    ReservationRepreFlag =1
WHERE  SQ.Seq = 1;
```

普通に書いてもそれなりに分かる。

``` sql
UPDATE [SQ]
SET [SQ].[ReservationRepreFlag] = 1
FROM
    (
        SELECT
            ROW_NUMBER() OVER(PARTITION BY [Reserve].[ReservationNo] ORDER BY [Player].[PlayerNo]) AS Seq,
            [Player].*
        FROM
            [TRe_Reservation] AS [Reserve]
            JOIN [TRe_ReservationPlayer_Test] AS [Player]
            ON [Reserve].[ReservationNo] = [Player].[ReservationNo]
            AND [Reserve].[RepreCustomerCD] = [Player].[CustomerCD]
            AND ISNULL([Reserve].[ReservationCancelFlag],1) = 0
            AND REPLACE(REPLACE(ISNULL([Reserve].[RepreCustomerCD],''),' ',''),'　','') <> ''
    ) AS [SQ]
WHERE
    [SQ].[Seq] = 1
```

[SQL ROW_NUMBER with INNER JOIN](https://stackoverflow.com/questions/9752361/sql-row-number-with-inner-join)  
[How to Select the First Row in Each GROUP BY Group](https://learnsql.com/cookbook/how-to-select-the-first-row-in-each-group-by-group/)  
ROW_NUMBER join

そもそも、JOINの条件にROW_NUMBERを使えないかも調べた。  
そしたらWITHで保持する方法を発見したんだ。  
ROW_NUMBERはSELECTでしか使えない見たい。  
その制限がある限り、絶対にワンクッションSELECTしないといけない。  

``` sql
-- 結局、こうやるしかないみたい。
WITH added_row_number AS (
  SELECT
    *,
    ROW_NUMBER() OVER(PARTITION BY year ORDER BY result DESC) AS row_number
  FROM exam_results
)
SELECT *
FROM added_row_number
WHERE row_number = 1;
```

### 行けそうだけどいけないUPDATE

ネットの記事を参考にして色々やってみたのだが、sqlserverでは受け付けてもらえなかった。  
でも、なんか行けそうなので備忘録として残しておく。  

2022/05/22 Sun 追記  
21日SQLServerにおけるJoinサンプルを実現させた後から見ると、UPDATEのテーブル指定に別名を指定していないからではないかと思われる。  
パターン2のUPDATE中にJoin句を書くのはSQLServerでは受け付けてくれない。  

``` sql : 例1
UPDATE [TB_会員]
SET
    日付0 = MAX(CASE WHEN [AA].ItemCD = '101' THEN [AA].[Date] ELSE NULL END),
    日付1 = MAX(CASE WHEN [AA].ItemCD = '102' THEN [AA].[Date] ELSE NULL END),
    日付2 = MAX(CASE WHEN [AA].ItemCD = '103' THEN [AA].[Date] ELSE NULL END),
    日付3 = MAX(CASE WHEN [AA].ItemCD = '104' THEN [AA].[Date] ELSE NULL END),
    日付4 = MAX(CASE WHEN [AA].ItemCD = '105' THEN [AA].[Date] ELSE NULL END),
    日付5 = MAX(CASE WHEN [AA].ItemCD = '106' THEN [AA].[Date] ELSE NULL END),
    日付6 = MAX(CASE WHEN [AA].ItemCD = '107' THEN [AA].[Date] ELSE NULL END),
    日付7 = MAX(CASE WHEN [AA].ItemCD = '108' THEN [AA].[Date] ELSE NULL END),
    日付8 = MAX(CASE WHEN [AA].ItemCD = '109' THEN [AA].[Date] ELSE NULL END),
    日付9 = MAX(CASE WHEN [AA].ItemCD = '110' THEN [AA].[Date] ELSE NULL END),
    数値0 = MAX(CASE WHEN [AA].ItemCD = '111' THEN [AA].[Number] ELSE NULL END),
    数値1 = MAX(CASE WHEN [AA].ItemCD = '112' THEN [AA].[Number] ELSE NULL END),
    数値2 = MAX(CASE WHEN [AA].ItemCD = '113' THEN [AA].[Number] ELSE NULL END),
    数値3 = MAX(CASE WHEN [AA].ItemCD = '114' THEN [AA].[Number] ELSE NULL END),
    数値4 = MAX(CASE WHEN [AA].ItemCD = '115' THEN [AA].[Number] ELSE NULL END),
    数値5 = MAX(CASE WHEN [AA].ItemCD = '116' THEN [AA].[Number] ELSE NULL END),
    数値6 = MAX(CASE WHEN [AA].ItemCD = '117' THEN [AA].[Number] ELSE NULL END),
    数値7 = MAX(CASE WHEN [AA].ItemCD = '118' THEN [AA].[Number] ELSE NULL END),
    数値8 = MAX(CASE WHEN [AA].ItemCD = '119' THEN [AA].[Number] ELSE NULL END),
    数値9 = MAX(CASE WHEN [AA].ItemCD = '120' THEN [AA].[Number] ELSE NULL END),
    名称0 = MAX(CASE WHEN [AA].ItemCD = '121' THEN [AA].[Text] ELSE '' END),
    名称1 = MAX(CASE WHEN [AA].ItemCD = '122' THEN [AA].[Text] ELSE '' END),
    名称2 = MAX(CASE WHEN [AA].ItemCD = '123' THEN [AA].[Text] ELSE '' END),
    名称3 = MAX(CASE WHEN [AA].ItemCD = '124' THEN [AA].[Text] ELSE '' END),
    名称4 = MAX(CASE WHEN [AA].ItemCD = '125' THEN [AA].[Text] ELSE '' END),
    名称5 = MAX(CASE WHEN [AA].ItemCD = '126' THEN [AA].[Text] ELSE '' END),
    名称6 = MAX(CASE WHEN [AA].ItemCD = '127' THEN [AA].[Text] ELSE '' END),
    名称7 = MAX(CASE WHEN [AA].ItemCD = '128' THEN [AA].[Text] ELSE '' END),
    名称8 = MAX(CASE WHEN [AA].ItemCD = '129' THEN [AA].[Text] ELSE '' END),
    名称9 = MAX(CASE WHEN [AA].ItemCD = '130' THEN [AA].[Text] ELSE '' END)
FROM
    [TB_会員]
    INNER JOIN Round3DatBRK_20220308.dbo.TMc_CustomerGenericInfoContent AS [AA]
    ON [TB_会員].[顧客CD] = REPLACE([AA].[CustomerCD],'BRK','')
    AND [AA].[UpdateProgram] LIKE 'RN3.WPF%'
    GROUP BY [TB_会員].[顧客CD]
```

``` sql : 例2
-- こいつは確実に無理。
-- UPDATE句のなかでINNER JOINするパターンはSQL Serverは許可していないはず。
UPDATE
    [TB_会員]
    INNER JOIN Round3DatBRK_20220308.dbo.TMc_CustomerGenericInfoContent AS [RN3_Cus] 
    ON [TB_会員].顧客CD = REPLACE([RN3_Cus].[CustomerCD],'BRK','')
    AND [RN3_Cus].[UpdateProgram] LIKE 'RN3.WPF%'
SET
    日付0 = MAX(CASE WHEN ItemCD = '101' THEN [Date] ELSE NULL END) AS [日付0],
    日付1 = MAX(CASE WHEN ItemCD = '102' THEN [Date] ELSE NULL END) AS [日付1],
    日付2 = MAX(CASE WHEN ItemCD = '103' THEN [Date] ELSE NULL END) AS [日付2],
    日付3 = MAX(CASE WHEN ItemCD = '104' THEN [Date] ELSE NULL END) AS [日付3],
    日付4 = MAX(CASE WHEN ItemCD = '105' THEN [Date] ELSE NULL END) AS [日付4],
    日付5 = MAX(CASE WHEN ItemCD = '106' THEN [Date] ELSE NULL END) AS [日付5],
    日付6 = MAX(CASE WHEN ItemCD = '107' THEN [Date] ELSE NULL END) AS [日付6],
    日付7 = MAX(CASE WHEN ItemCD = '108' THEN [Date] ELSE NULL END) AS [日付7],
    日付8 = MAX(CASE WHEN ItemCD = '109' THEN [Date] ELSE NULL END) AS [日付8],
    日付9 = MAX(CASE WHEN ItemCD = '110' THEN [Date] ELSE NULL END) AS [日付9],
    数値0 = MAX(CASE WHEN ItemCD = '111' THEN [Number] ELSE NULL END) AS [数値0],
    数値1 = MAX(CASE WHEN ItemCD = '112' THEN [Number] ELSE NULL END) AS [数値1],
    数値2 = MAX(CASE WHEN ItemCD = '113' THEN [Number] ELSE NULL END) AS [数値2],
    数値3 = MAX(CASE WHEN ItemCD = '114' THEN [Number] ELSE NULL END) AS [数値3],
    数値4 = MAX(CASE WHEN ItemCD = '115' THEN [Number] ELSE NULL END) AS [数値4],
    数値5 = MAX(CASE WHEN ItemCD = '116' THEN [Number] ELSE NULL END) AS [数値5],
    数値6 = MAX(CASE WHEN ItemCD = '117' THEN [Number] ELSE NULL END) AS [数値6],
    数値7 = MAX(CASE WHEN ItemCD = '118' THEN [Number] ELSE NULL END) AS [数値7],
    数値8 = MAX(CASE WHEN ItemCD = '119' THEN [Number] ELSE NULL END) AS [数値8],
    数値9 = MAX(CASE WHEN ItemCD = '120' THEN [Number] ELSE NULL END) AS [数値9],
    名称0 = MAX(CASE WHEN ItemCD = '121' THEN [Text] ELSE NULL END) AS [名称0],
    名称1 = MAX(CASE WHEN ItemCD = '122' THEN [Text] ELSE NULL END) AS [名称1],
    名称2 = MAX(CASE WHEN ItemCD = '123' THEN [Text] ELSE NULL END) AS [名称2],
    名称3 = MAX(CASE WHEN ItemCD = '124' THEN [Text] ELSE NULL END) AS [名称3],
    名称4 = MAX(CASE WHEN ItemCD = '125' THEN [Text] ELSE NULL END) AS [名称4],
    名称5 = MAX(CASE WHEN ItemCD = '126' THEN [Text] ELSE NULL END) AS [名称5],
    名称6 = MAX(CASE WHEN ItemCD = '127' THEN [Text] ELSE NULL END) AS [名称6],
    名称7 = MAX(CASE WHEN ItemCD = '128' THEN [Text] ELSE NULL END) AS [名称7],
    名称8 = MAX(CASE WHEN ItemCD = '129' THEN [Text] ELSE NULL END) AS [名称8],
    名称9 = MAX(CASE WHEN ItemCD = '130' THEN [Text] ELSE NULL END) AS [名称9]
WHERE
    [TB_会員].顧客CD = REPLACE([RN3_Cus].[CustomerCD],'BRK','')
```

``` sql : 例3
UPDATE [TB_会員]
SET
    [日付0] = [SQ].[日付0],
    [日付1] = [SQ].[日付1],
    [日付2] = [SQ].[日付2],
    [日付3] = [SQ].[日付3],
    [日付4] = [SQ].[日付4],
    [日付5] = [SQ].[日付5],
    [日付6] = [SQ].[日付6],
    [日付7] = [SQ].[日付7],
    [日付8] = [SQ].[日付8],
    [日付9] = [SQ].[日付9],
    [数値0] = [SQ].[数値0],
    [数値1] = [SQ].[数値1],
    [数値2] = [SQ].[数値2],
    [数値3] = [SQ].[数値3],
    [数値4] = [SQ].[数値4],
    [数値5] = [SQ].[数値5],
    [数値6] = [SQ].[数値6],
    [数値7] = [SQ].[数値7],
    [数値8] = [SQ].[数値8],
    [数値9] = [SQ].[数値9],
    [名称0] = [SQ].[名称0],
    [名称1] = [SQ].[名称1],
    [名称2] = [SQ].[名称2],
    [名称3] = [SQ].[名称3],
    [名称4] = [SQ].[名称4],
    [名称5] = [SQ].[名称5],
    [名称6] = [SQ].[名称6],
    [名称7] = [SQ].[名称7],
    [名称8] = [SQ].[名称8],
    [名称9] = [SQ].[名称9]
FROM( 
    SELECT
        [TB_会員].[顧客CD],
        MAX(CASE WHEN [AA].[ItemCD] = '101' THEN [AA].[Date] ELSE NULL END) AS [日付0],
        MAX(CASE WHEN [AA].[ItemCD] = '102' THEN [AA].[Date] ELSE NULL END) AS [日付1],
        MAX(CASE WHEN [AA].[ItemCD] = '103' THEN [AA].[Date] ELSE NULL END) AS [日付2],
        MAX(CASE WHEN [AA].[ItemCD] = '104' THEN [AA].[Date] ELSE NULL END) AS [日付3],
        MAX(CASE WHEN [AA].[ItemCD] = '105' THEN [AA].[Date] ELSE NULL END) AS [日付4],
        MAX(CASE WHEN [AA].[ItemCD] = '106' THEN [AA].[Date] ELSE NULL END) AS [日付5],
        MAX(CASE WHEN [AA].[ItemCD] = '107' THEN [AA].[Date] ELSE NULL END) AS [日付6],
        MAX(CASE WHEN [AA].[ItemCD] = '108' THEN [AA].[Date] ELSE NULL END) AS [日付7],
        MAX(CASE WHEN [AA].[ItemCD] = '109' THEN [AA].[Date] ELSE NULL END) AS [日付8],
        MAX(CASE WHEN [AA].[ItemCD] = '110' THEN [AA].[Date] ELSE NULL END) AS [日付9],
        MAX(CASE WHEN [AA].[ItemCD] = '111' THEN [AA].[Number] ELSE NULL END) AS [数値0],
        MAX(CASE WHEN [AA].[ItemCD] = '112' THEN [AA].[Number] ELSE NULL END) AS [数値1],
        MAX(CASE WHEN [AA].[ItemCD] = '113' THEN [AA].[Number] ELSE NULL END) AS [数値2],
        MAX(CASE WHEN [AA].[ItemCD] = '114' THEN [AA].[Number] ELSE NULL END) AS [数値3],
        MAX(CASE WHEN [AA].[ItemCD] = '115' THEN [AA].[Number] ELSE NULL END) AS [数値4],
        MAX(CASE WHEN [AA].[ItemCD] = '116' THEN [AA].[Number] ELSE NULL END) AS [数値5],
        MAX(CASE WHEN [AA].[ItemCD] = '117' THEN [AA].[Number] ELSE NULL END) AS [数値6],
        MAX(CASE WHEN [AA].[ItemCD] = '118' THEN [AA].[Number] ELSE NULL END) AS [数値7],
        MAX(CASE WHEN [AA].[ItemCD] = '119' THEN [AA].[Number] ELSE NULL END) AS [数値8],
        MAX(CASE WHEN [AA].[ItemCD] = '120' THEN [AA].[Number] ELSE NULL END) AS [数値9],
        MAX(CASE WHEN [AA].[ItemCD] = '121' THEN [AA].[Text] ELSE '' END) AS [名称0],
        MAX(CASE WHEN [AA].[ItemCD] = '122' THEN [AA].[Text] ELSE '' END) AS [名称1],
        MAX(CASE WHEN [AA].[ItemCD] = '123' THEN [AA].[Text] ELSE '' END) AS [名称2],
        MAX(CASE WHEN [AA].[ItemCD] = '124' THEN [AA].[Text] ELSE '' END) AS [名称3],
        MAX(CASE WHEN [AA].[ItemCD] = '125' THEN [AA].[Text] ELSE '' END) AS [名称4],
        MAX(CASE WHEN [AA].[ItemCD] = '126' THEN [AA].[Text] ELSE '' END) AS [名称5],
        MAX(CASE WHEN [AA].[ItemCD] = '127' THEN [AA].[Text] ELSE '' END) AS [名称6],
        MAX(CASE WHEN [AA].[ItemCD] = '128' THEN [AA].[Text] ELSE '' END) AS [名称7],
        MAX(CASE WHEN [AA].[ItemCD] = '129' THEN [AA].[Text] ELSE '' END) AS [名称8],
        MAX(CASE WHEN [AA].[ItemCD] = '130' THEN [AA].[Text] ELSE '' END) AS [名称9]
    FROM [TB_会員]
    INNER JOIN [Round3DatBRK_20220308].[dbo].[TMc_CustomerGenericInfoContent] AS [AA]
    ON [AA].[OfficeCD]+[TB_会員].[顧客CD] = [AA].[CustomerCD]
    AND [AA].[UpdateProgram] LIKE 'RN3.WPF%'
    GROUP BY [TB_会員].[顧客CD]
) AS [SQ]
WHERE
    [TB_会員].顧客CD = [SQ].[顧客CD]



-- こっちなら行けた。
-- GROUP BYしてCASEでMAXを取得する以上、そういうテーブルを定義するしかないのだろうねぇ。
UPDATE [TB_会員_Test]
SET
    [日付0] = [SQ].[日付0],
    [日付1] = [SQ].[日付1],
    [日付2] = [SQ].[日付2],
    [日付3] = [SQ].[日付3],
    [日付4] = [SQ].[日付4],
    [日付5] = [SQ].[日付5],
    [日付6] = [SQ].[日付6],
    [日付7] = [SQ].[日付7],
    [日付8] = [SQ].[日付8],
    [日付9] = [SQ].[日付9],
    [数値0] = [SQ].[数値0],
    [数値1] = [SQ].[数値1],
    [数値2] = [SQ].[数値2],
    [数値3] = [SQ].[数値3],
    [数値4] = [SQ].[数値4],
    [数値5] = [SQ].[数値5],
    [数値6] = [SQ].[数値6],
    [数値7] = [SQ].[数値7],
    [数値8] = [SQ].[数値8],
    [数値9] = [SQ].[数値9],
    [名称0] = [SQ].[名称0],
    [名称1] = [SQ].[名称1],
    [名称2] = [SQ].[名称2],
    [名称3] = [SQ].[名称3],
    [名称4] = [SQ].[名称4],
    [名称5] = [SQ].[名称5],
    [名称6] = [SQ].[名称6],
    [名称7] = [SQ].[名称7],
    [名称8] = [SQ].[名称8],
    [名称9] = [SQ].[名称9]
FROM
    [TB_会員_Test]
    JOIN
    ( 
        SELECT
            [Kaiin].[顧客CD],
            MAX(CASE WHEN [AA].[ItemCD] = '101' THEN [AA].[Date] ELSE NULL END) AS [日付0],
            MAX(CASE WHEN [AA].[ItemCD] = '102' THEN [AA].[Date] ELSE NULL END) AS [日付1],
            MAX(CASE WHEN [AA].[ItemCD] = '103' THEN [AA].[Date] ELSE NULL END) AS [日付2],
            MAX(CASE WHEN [AA].[ItemCD] = '104' THEN [AA].[Date] ELSE NULL END) AS [日付3],
            MAX(CASE WHEN [AA].[ItemCD] = '105' THEN [AA].[Date] ELSE NULL END) AS [日付4],
            MAX(CASE WHEN [AA].[ItemCD] = '106' THEN [AA].[Date] ELSE NULL END) AS [日付5],
            MAX(CASE WHEN [AA].[ItemCD] = '107' THEN [AA].[Date] ELSE NULL END) AS [日付6],
            MAX(CASE WHEN [AA].[ItemCD] = '108' THEN [AA].[Date] ELSE NULL END) AS [日付7],
            MAX(CASE WHEN [AA].[ItemCD] = '109' THEN [AA].[Date] ELSE NULL END) AS [日付8],
            MAX(CASE WHEN [AA].[ItemCD] = '110' THEN [AA].[Date] ELSE NULL END) AS [日付9],
            MAX(CASE WHEN [AA].[ItemCD] = '111' THEN [AA].[Number] ELSE NULL END) AS [数値0],
            MAX(CASE WHEN [AA].[ItemCD] = '112' THEN [AA].[Number] ELSE NULL END) AS [数値1],
            MAX(CASE WHEN [AA].[ItemCD] = '113' THEN [AA].[Number] ELSE NULL END) AS [数値2],
            MAX(CASE WHEN [AA].[ItemCD] = '114' THEN [AA].[Number] ELSE NULL END) AS [数値3],
            MAX(CASE WHEN [AA].[ItemCD] = '115' THEN [AA].[Number] ELSE NULL END) AS [数値4],
            MAX(CASE WHEN [AA].[ItemCD] = '116' THEN [AA].[Number] ELSE NULL END) AS [数値5],
            MAX(CASE WHEN [AA].[ItemCD] = '117' THEN [AA].[Number] ELSE NULL END) AS [数値6],
            MAX(CASE WHEN [AA].[ItemCD] = '118' THEN [AA].[Number] ELSE NULL END) AS [数値7],
            MAX(CASE WHEN [AA].[ItemCD] = '119' THEN [AA].[Number] ELSE NULL END) AS [数値8],
            MAX(CASE WHEN [AA].[ItemCD] = '120' THEN [AA].[Number] ELSE NULL END) AS [数値9],
            MAX(CASE WHEN [AA].[ItemCD] = '121' THEN [AA].[Text] ELSE '' END) AS [名称0],
            MAX(CASE WHEN [AA].[ItemCD] = '122' THEN [AA].[Text] ELSE '' END) AS [名称1],
            MAX(CASE WHEN [AA].[ItemCD] = '123' THEN [AA].[Text] ELSE '' END) AS [名称2],
            MAX(CASE WHEN [AA].[ItemCD] = '124' THEN [AA].[Text] ELSE '' END) AS [名称3],
            MAX(CASE WHEN [AA].[ItemCD] = '125' THEN [AA].[Text] ELSE '' END) AS [名称4],
            MAX(CASE WHEN [AA].[ItemCD] = '126' THEN [AA].[Text] ELSE '' END) AS [名称5],
            MAX(CASE WHEN [AA].[ItemCD] = '127' THEN [AA].[Text] ELSE '' END) AS [名称6],
            MAX(CASE WHEN [AA].[ItemCD] = '128' THEN [AA].[Text] ELSE '' END) AS [名称7],
            MAX(CASE WHEN [AA].[ItemCD] = '129' THEN [AA].[Text] ELSE '' END) AS [名称8],
            MAX(CASE WHEN [AA].[ItemCD] = '130' THEN [AA].[Text] ELSE '' END) AS [名称9]
        FROM [TB_会員_Test] AS [Kaiin]
        INNER JOIN Round3DatKHG_Ibayashi.[dbo].[TMc_CustomerGenericInfoContent] AS [AA]
        ON [AA].[OfficeCD]+[Kaiin].[顧客CD] = [AA].[CustomerCD]
        AND [AA].[UpdateProgram] LIKE 'CONV'
        GROUP BY [Kaiin].[顧客CD]
    ) AS [SQ]
    ON [TB_会員_Test].[顧客CD] = [SQ].[顧客CD]

```

---

## DELETE

```sql : DELTE 基本
DELETE FROM テーブル名 WHERE (条件);
```

``` sql : join
-- ●join
-- https://stackoverflow.com/questions/16481379/how-can-i-delete-using-inner-join-with-sql-server
-- DELETE FROM JOIN WHEREの流れは基本的に同じ見たい。それさえ分かれば後は十分だろう。
-- joinの場合、削除するテーブルを指定する必要があるので、DELETEの後ろにエイリアスを指定する(テーブル名ではダメ見たい)。
DELETE w
FROM WorkRecord2 w
INNER JOIN Employee e
ON EmployeeRun=EmployeeNo
WHERE Company = '1' AND Date = '2013-05-06'
```

### 複数テーブルの削除

[【MySQL】共通のIDのデータを複数テーブルからDELETEする](https://hamalabo.net/mysql-multi-delete)  

``` sql
-- 通常の場合
DELETE FROM M_UserData WHERE UserId = 1;
DELETE FROM T_TimeCard WHERE UserId = 1;

-- 複数テーブルの場合
DELETE [User],[Time]
FROM M_UserData As [User]
LEFT JOIN T_TimeCard AS [Time]
ON User.UserId = Time.UserId
WHERE User.UserId = 1
```

---

## CREATE TABLE

``` sql
CREATE TABLE [dbo].[Customers]
(
    CustomerID  nchar(5)      NOT NULL,
    CompanyName nvarchar(50)  NOT NULL,
    ContactName nvarchar (50) NULL,
    Phone       nvarchar (24) NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerID])
)
```

``` sql
CREATE TABLE ConversionTable  
(
    [Id] int IDENTITY(1,1),  
    [From] int,  
    [To] int,  
    [Name] nvarchar(255),
    [Kana] nvarchar(255)  
);

INSERT INTO ConversionTable VALUES (0,4,'愛別','ｱｲﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (5,9,'赤井川','ｱｶｲｶﾞﾜ')
INSERT INTO ConversionTable VALUES (10,14,'赤平','ｱｶﾋﾞﾗ')
INSERT INTO ConversionTable VALUES (15,19,'阿寒','ｱｶﾝ')
INSERT INTO ConversionTable VALUES (20,24,'旭川','ｱｻﾋｶﾜ')
INSERT INTO ConversionTable VALUES (25,29,'朝日','ｱｻﾋ')
INSERT INTO ConversionTable VALUES (30,34,'芦別','ｱｼﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (35,39,'足寄','ｱｼｮﾛ')
INSERT INTO ConversionTable VALUES (40,44,'厚岸','ｱｯｹ')
INSERT INTO ConversionTable VALUES (45,49,'厚沢部','ｱｯｻﾌﾞ')
INSERT INTO ConversionTable VALUES (50,54,'厚田','ｱﾂﾀ')
INSERT INTO ConversionTable VALUES (55,59,'厚真','ｱﾂﾏ')
INSERT INTO ConversionTable VALUES (60,64,'網走','ｱﾊﾞｼﾘ')
INSERT INTO ConversionTable VALUES (65,69,'安平','ｱﾋﾞﾗ')
INSERT INTO ConversionTable VALUES (70,74,'虻田','ｱﾌﾞﾀ')
INSERT INTO ConversionTable VALUES (75,79,'生田原','ｲｸﾀﾊﾗ')
INSERT INTO ConversionTable VALUES (80,84,'池田','ｲｹﾀﾞ')
INSERT INTO ConversionTable VALUES (85,89,'石狩','ｲｼｶﾘ')
INSERT INTO ConversionTable VALUES (90,94,'今金','ｲﾏｶﾈ')
INSERT INTO ConversionTable VALUES (95,99,'岩内','ｲﾜﾅｲ')
INSERT INTO ConversionTable VALUES (100,104,'岩見沢','ｲﾜﾐｻﾞﾜ')
INSERT INTO ConversionTable VALUES (105,109,'歌志内','ｳﾀｼﾅｲ')
INSERT INTO ConversionTable VALUES (110,114,'歌登','ｳﾀﾉﾎﾞﾘ')
INSERT INTO ConversionTable VALUES (115,119,'浦臼','ｳﾗｳｽ')
INSERT INTO ConversionTable VALUES (120,124,'浦河','ｳﾗｶﾜ')
INSERT INTO ConversionTable VALUES (125,129,'浦幌','ｳﾗﾎﾛ')
INSERT INTO ConversionTable VALUES (130,134,'雨竜','ｳﾘｭｳ')
INSERT INTO ConversionTable VALUES (135,139,'江差','ｴｻ')
INSERT INTO ConversionTable VALUES (140,144,'枝幸','ｴｻ')
INSERT INTO ConversionTable VALUES (145,149,'恵山','ｴｻﾝ')
INSERT INTO ConversionTable VALUES (150,154,'恵庭','ｴﾆﾜ')
INSERT INTO ConversionTable VALUES (155,159,'江部乙','ｴﾍﾞｵﾂ')
INSERT INTO ConversionTable VALUES (160,164,'江別','ｴﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (165,169,'えりも','ｴﾘﾓ')
INSERT INTO ConversionTable VALUES (170,174,'遠軽','ｴﾝｶﾞﾙ')
INSERT INTO ConversionTable VALUES (175,179,'遠別','ｴﾝﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (180,184,'追分','ｵｲﾜｹ')
INSERT INTO ConversionTable VALUES (185,189,'雄武','ｵｳﾑ')
INSERT INTO ConversionTable VALUES (190,194,'大空','ｵｵｿﾞﾗ')
INSERT INTO ConversionTable VALUES (195,199,'大滝','ｵｵﾀｷ')
INSERT INTO ConversionTable VALUES (200,204,'大野','ｵｵﾉ')
INSERT INTO ConversionTable VALUES (205,209,'奥尻','ｵｸｼﾘ')
INSERT INTO ConversionTable VALUES (210,214,'置戸','ｵｹﾄ')
INSERT INTO ConversionTable VALUES (215,219,'興部','ｵｺｯﾍﾟ')
INSERT INTO ConversionTable VALUES (220,224,'長万部','ｵｼｬﾏﾝﾍﾞ')
INSERT INTO ConversionTable VALUES (225,229,'小樽','ｵﾀﾙ')
INSERT INTO ConversionTable VALUES (230,234,'音威子府','ｵﾄｲﾈｯﾌﾟ')
INSERT INTO ConversionTable VALUES (235,239,'音更','ｵﾄﾌｹ')
INSERT INTO ConversionTable VALUES (240,244,'乙部','ｵﾄﾍﾞ')
INSERT INTO ConversionTable VALUES (245,249,'帯広','ｵﾋﾞﾋﾛ')
INSERT INTO ConversionTable VALUES (250,254,'小平','ｵﾋﾞﾗ')
INSERT INTO ConversionTable VALUES (255,259,'音別','ｵﾝﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (260,264,'上磯','ｶﾐｲｿ')
INSERT INTO ConversionTable VALUES (265,269,'上川','ｶﾐｶﾜ')
INSERT INTO ConversionTable VALUES (270,274,'上士幌','ｶﾐｼﾎﾛ')
INSERT INTO ConversionTable VALUES (275,279,'上砂川','ｶﾐｽﾅｶﾞﾜ')
INSERT INTO ConversionTable VALUES (280,284,'上ノ国','ｶﾐﾉｸﾆ')
INSERT INTO ConversionTable VALUES (285,289,'上富良野','ｶﾐﾌﾗﾉ')
INSERT INTO ConversionTable VALUES (290,294,'上湧別','ｶﾐﾕｳﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (295,299,'亀田','ｶﾒﾀﾞ')
INSERT INTO ConversionTable VALUES (300,304,'神恵内','ｶﾓｴﾅｲ')
INSERT INTO ConversionTable VALUES (305,309,'木古内','ｷｺﾅｲ')
INSERT INTO ConversionTable VALUES (310,314,'北檜山','ｷﾀﾋﾔﾏ')
INSERT INTO ConversionTable VALUES (315,319,'北広島','ｷﾀﾋﾛｼﾏ')
INSERT INTO ConversionTable VALUES (320,324,'北見','ｷﾀﾐ')
INSERT INTO ConversionTable VALUES (325,329,'北','ｷﾀ')
INSERT INTO ConversionTable VALUES (330,334,'喜茂別','ｷﾓﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (335,339,'京極','ｷｮｳｺﾞｸ')
INSERT INTO ConversionTable VALUES (340,344,'共和','ｷｮｳﾜ')
INSERT INTO ConversionTable VALUES (345,349,'清里','ｷﾖｻﾄ')
INSERT INTO ConversionTable VALUES (350,354,'釧路','ｸｼﾛ')
INSERT INTO ConversionTable VALUES (355,359,'倶知安','ｸｯﾁｬﾝ')
INSERT INTO ConversionTable VALUES (360,364,'熊石','ｸﾏｲ')
INSERT INTO ConversionTable VALUES (365,369,'栗沢','ｸﾘｻﾜ')
INSERT INTO ConversionTable VALUES (370,374,'栗山','ｸﾘﾔﾏ')
INSERT INTO ConversionTable VALUES (375,379,'黒松内','ｸﾛﾏﾂﾅｲ')
INSERT INTO ConversionTable VALUES (380,384,'訓子府','ｸﾝﾈｯﾌﾟ')
INSERT INTO ConversionTable VALUES (385,389,'剣淵','ｹﾝﾌﾞﾁ')
INSERT INTO ConversionTable VALUES (390,394,'小清水','ｺｼﾐｽﾞ')
INSERT INTO ConversionTable VALUES (395,399,'札幌','ｻｯﾎﾟﾛ')
INSERT INTO ConversionTable VALUES (400,404,'様似','ｻﾏﾆ')
INSERT INTO ConversionTable VALUES (405,409,'更別','ｻﾗﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (410,414,'猿払','ｻﾙﾌﾂ')
INSERT INTO ConversionTable VALUES (415,419,'佐呂間','ｻﾛﾏ')
INSERT INTO ConversionTable VALUES (420,424,'砂原','ｻﾜﾗ')
INSERT INTO ConversionTable VALUES (425,429,'鹿追','ｼｶｵｲ')
INSERT INTO ConversionTable VALUES (430,434,'鹿部','ｼｶﾍﾞ')
INSERT INTO ConversionTable VALUES (435,439,'色丹','ｼｺﾀﾝ')
INSERT INTO ConversionTable VALUES (440,444,'静内','ｼｽﾞﾅｲ')
INSERT INTO ConversionTable VALUES (445,449,'標茶','ｼﾍﾞﾁｬ')
INSERT INTO ConversionTable VALUES (450,454,'士別','ｼﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (455,459,'標津','ｼﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (460,464,'蘂取','ｼﾍﾞﾄﾛ')
INSERT INTO ConversionTable VALUES (465,469,'士幌','ｼﾎﾛ')
INSERT INTO ConversionTable VALUES (470,474,'島牧','ｼﾏﾏｷ')
INSERT INTO ConversionTable VALUES (475,479,'清水','ｼﾐｽﾞ')
INSERT INTO ConversionTable VALUES (480,484,'占冠','ｼﾑｶｯﾌﾟ')
INSERT INTO ConversionTable VALUES (485,489,'下川','ｼﾓｶﾜ')
INSERT INTO ConversionTable VALUES (490,494,'積丹','ｼｬｺﾀﾝ')
INSERT INTO ConversionTable VALUES (495,499,'紗那','ｼｬﾅ')
INSERT INTO ConversionTable VALUES (500,504,'斜里','ｼｬﾘ')
INSERT INTO ConversionTable VALUES (505,509,'初山別','ｼｮｻﾝﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (510,514,'白老','ｼﾗｵｲ')
INSERT INTO ConversionTable VALUES (515,519,'白滝','ｼﾗﾀｷ')
INSERT INTO ConversionTable VALUES (520,524,'白糠','ｼﾗﾇｶ')
INSERT INTO ConversionTable VALUES (525,529,'知内','ｼﾘｳﾁ')
INSERT INTO ConversionTable VALUES (530,534,'尻岸内','ｼﾘｷｼﾅｲ')
INSERT INTO ConversionTable VALUES (535,539,'新篠津','ｼﾝｼﾉﾂ')
INSERT INTO ConversionTable VALUES (540,544,'新得','ｼﾝﾄｸ')
INSERT INTO ConversionTable VALUES (545,549,'新十津川','ｼﾝﾄﾂｶﾜ')
INSERT INTO ConversionTable VALUES (550,554,'新ひだか','ｼﾝﾋﾀﾞｶ')
INSERT INTO ConversionTable VALUES (555,559,'寿都','ｽｯﾂ')
INSERT INTO ConversionTable VALUES (560,564,'砂川','ｽﾅｶﾞﾜ')
INSERT INTO ConversionTable VALUES (565,569,'せたな','ｾﾀﾅ')
INSERT INTO ConversionTable VALUES (570,574,'壮瞥','ｿｳﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (575,579,'大樹','ﾀｲｷ')
INSERT INTO ConversionTable VALUES (580,584,'大成','ﾀｲｾｲ')
INSERT INTO ConversionTable VALUES (585,589,'鷹栖','ﾀｶｽ')
INSERT INTO ConversionTable VALUES (590,594,'滝川','ﾀｷｶﾜ')
INSERT INTO ConversionTable VALUES (595,599,'滝上','ﾀｷﾉｳｴ')
INSERT INTO ConversionTable VALUES (600,604,'端野','ﾀﾝﾉ')
INSERT INTO ConversionTable VALUES (605,609,'伊達','ﾀﾞﾃ')
INSERT INTO ConversionTable VALUES (610,614,'秩父別','ﾁｯﾌﾟﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (615,619,'千歳','ﾁﾄｾ')
INSERT INTO ConversionTable VALUES (620,624,'忠類','ﾁｭｳﾙｲ')
INSERT INTO ConversionTable VALUES (625,629,'月形','ﾂｷｶﾞﾀ')
INSERT INTO ConversionTable VALUES (630,634,'津別','ﾂﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (635,639,'鶴居','ﾂﾙｲ')
INSERT INTO ConversionTable VALUES (640,644,'天塩','ﾃｼｵ')
INSERT INTO ConversionTable VALUES (645,649,'弟子屈','ﾃｼｶｶﾞ')
INSERT INTO ConversionTable VALUES (650,654,'戸井','ﾄｲ')
INSERT INTO ConversionTable VALUES (655,659,'当別','ﾄｳﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (660,664,'当麻','ﾄｳﾏ')
INSERT INTO ConversionTable VALUES (665,669,'洞爺湖','ﾄｳﾔｺ')
INSERT INTO ConversionTable VALUES (670,674,'洞爺','ﾄｳﾔ')
INSERT INTO ConversionTable VALUES (675,679,'常呂','ﾄｺﾛ')
INSERT INTO ConversionTable VALUES (680,684,'椴法華','ﾄﾄﾞﾎｯｹ')
INSERT INTO ConversionTable VALUES (685,689,'苫小牧','ﾄﾏｺﾏｲ')
INSERT INTO ConversionTable VALUES (690,694,'苫前','ﾄﾏﾏｴ')
INSERT INTO ConversionTable VALUES (695,699,'泊','ﾄﾏﾘ')
INSERT INTO ConversionTable VALUES (700,704,'豊浦','ﾄﾖｳﾗ')
INSERT INTO ConversionTable VALUES (705,709,'豊頃','ﾄﾖｺﾛ')
INSERT INTO ConversionTable VALUES (710,714,'豊富','ﾄﾖﾄﾐ')
INSERT INTO ConversionTable VALUES (715,719,'奈井江','ﾅｲｴ')
INSERT INTO ConversionTable VALUES (720,724,'中川','ﾅｶｶﾞﾜ')
INSERT INTO ConversionTable VALUES (725,729,'中札内','ﾅｶｻﾂﾅｲ')
INSERT INTO ConversionTable VALUES (730,734,'中標津','ﾅｶｼﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (735,739,'中頓別','ﾅｶﾄﾝﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (740,744,'中富良野','ﾅｶﾌﾗﾉ')
INSERT INTO ConversionTable VALUES (745,749,'長沼','ﾅｶﾞﾇﾏ')
INSERT INTO ConversionTable VALUES (750,754,'七飯','ﾅﾅｴ')
INSERT INTO ConversionTable VALUES (755,759,'名寄','ﾅﾖﾛ')
INSERT INTO ConversionTable VALUES (760,764,'南幌','ﾅﾝﾎﾟﾛ')
INSERT INTO ConversionTable VALUES (765,769,'新冠','ﾆｲｶｯﾌﾟ')
INSERT INTO ConversionTable VALUES (770,774,'仁木','ﾆｷ')
INSERT INTO ConversionTable VALUES (775,779,'西興部','ﾆｼｵｺｯﾍﾟ')
INSERT INTO ConversionTable VALUES (780,784,'ニセコ','ﾆｾｺ')
INSERT INTO ConversionTable VALUES (785,789,'沼田','ﾇﾏﾀ')
INSERT INTO ConversionTable VALUES (790,794,'根室','ﾈﾑﾛ')
INSERT INTO ConversionTable VALUES (795,799,'登別','ﾉﾎﾞﾘﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (800,804,'函館','ﾊｺﾀﾞﾃ')
INSERT INTO ConversionTable VALUES (805,809,'羽幌','ﾊﾎﾞﾛ')
INSERT INTO ConversionTable VALUES (810,814,'浜頓別','ﾊﾏﾄﾝﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (815,819,'浜中','ﾊﾏﾅｶ')
INSERT INTO ConversionTable VALUES (820,824,'浜益','ﾊﾏﾏｽ')
INSERT INTO ConversionTable VALUES (825,829,'早来','ﾊﾔｷﾀ')
INSERT INTO ConversionTable VALUES (830,834,'東神楽','ﾋｶﾞｼｶｸﾞﾗ')
INSERT INTO ConversionTable VALUES (835,839,'東川','ﾋｶﾞｼｶﾜ')
INSERT INTO ConversionTable VALUES (840,844,'東鷹栖','ﾋｶﾞｼﾀｶｽ')
INSERT INTO ConversionTable VALUES (845,849,'東藻琴','ﾋｶﾞｼﾓｺﾄ')
INSERT INTO ConversionTable VALUES (850,854,'東利尻','ﾋｶﾞｼﾘｼﾘ')
INSERT INTO ConversionTable VALUES (855,859,'日高','ﾋﾀﾞｶ')
INSERT INTO ConversionTable VALUES (860,864,'広尾','ﾋﾛｵ')
INSERT INTO ConversionTable VALUES (865,869,'広島','ﾋﾛｼﾏ')
INSERT INTO ConversionTable VALUES (870,874,'美瑛','ﾋﾞｴｲ')
INSERT INTO ConversionTable VALUES (875,879,'美唄','ﾋﾞﾊﾞｲ')
INSERT INTO ConversionTable VALUES (880,884,'美深','ﾋﾞﾌｶ')
INSERT INTO ConversionTable VALUES (885,889,'美幌','ﾋﾞﾎﾛ')
INSERT INTO ConversionTable VALUES (890,894,'平取','ﾋﾞﾗﾄﾘ')
INSERT INTO ConversionTable VALUES (895,899,'比布','ﾋﾟｯﾌﾟ')
INSERT INTO ConversionTable VALUES (900,904,'風連','ﾌｳﾚﾝ')
INSERT INTO ConversionTable VALUES (905,909,'深川','ﾌｶｶﾞﾜ')
INSERT INTO ConversionTable VALUES (910,914,'福島','ﾌｸｼﾏ')
INSERT INTO ConversionTable VALUES (915,919,'富良野','ﾌﾗﾉ')
INSERT INTO ConversionTable VALUES (920,924,'古平','ﾌﾙﾋﾞﾗ')
INSERT INTO ConversionTable VALUES (925,929,'別海','ﾍﾞｯｶｲ')
INSERT INTO ConversionTable VALUES (930,934,'北斗','ﾎｸﾄ')
INSERT INTO ConversionTable VALUES (935,939,'北竜','ﾎｸﾘｭｳ')
INSERT INTO ConversionTable VALUES (940,944,'穂別','ﾎﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (945,949,'幌泉','ﾎﾛｲｽﾞﾐ')
INSERT INTO ConversionTable VALUES (950,954,'幌加内','ﾎﾛｶﾅｲ')
INSERT INTO ConversionTable VALUES (955,959,'幌延','ﾎﾛﾉﾍﾞ')
INSERT INTO ConversionTable VALUES (960,964,'本別','ﾎﾝﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (965,969,'幕別','ﾏｸﾍﾞﾂ')
INSERT INTO ConversionTable VALUES (970,974,'増毛','ﾏｼｹ')
INSERT INTO ConversionTable VALUES (975,979,'真狩','ﾏｯｶﾘ')
INSERT INTO ConversionTable VALUES (980,984,'松前','ﾏﾂﾏｴ')
INSERT INTO ConversionTable VALUES (985,989,'丸瀬布','ﾏﾙｾｯﾌﾟ')
INSERT INTO ConversionTable VALUES (990,994,'三笠','ﾐｶｻ')
INSERT INTO ConversionTable VALUES (995,999,'三石','ﾐﾂｲ')
```

---

## ALTER TABLE

### テーブルのカラム名を変更するSQL

```SQL
-- MariaDB
ALTER TABLE [TMa_Product] RENAME COLUMN [RevenuTypeCD] TO [RevenueTypeCD]

-- SQL Server
-- https://docs.microsoft.com/ja-jp/sql/relational-databases/system-stored-procedures/sp-rename-transact-sql?view=sql-server-ver15
-- 珍しく公式サイトが参考になった。
EXEC sp_rename 'スキーマ名.テーブル名.現在のカラム名', '新しいカラム名', 'COLUMN';

-- テーブルを指定したい場合はUSEでテーブルを切り替えて実行するしかないみたい。
USE Round3Dat_Test;
GO
EXEC sp_rename 'TMa_Supplier.ValidFalg','ValidFlag','COLUMN';
GO
```

### テーブルのデータ型を変更するSQL

```SQL
-- SQL Server
ALTER TABLE (操作対象テーブル) ALTER column (データ型を変更する列名) (変更するデータ型)

-- 例 : TMa_ProductテーブルのRevenueTypeCDカラムの型をintに変更するクエリ
ALTER TABLE [TMa_Product] ALTER column [RevenueTypeCD] int

--- MariaDB,カラム追加
-- TmOpenPlanPGMWEBテーブルのHolidayExtraPriceOneBagフィールドの後にTaxSelectionStatusTypeを追加。型はboolで初期値は0。コメント付き。
ALTER TABLE `TmOpenPlanPGMWEB` ADD COLUMN `TaxSelectionStatusType` TINYINT(1) NOT NULL DEFAULT 0 comment '税選択状態区分 税抜(外税):0 税込(内税):1' AFTER `HolidayExtraPriceOneBag`;

-- TmOpenPlanGDOテーブルからMailPushSendFlagフィールドを削除する
ALTER TABLE TmOpenPlanGDO DROP COLUMN MailPushSendFlag;
```

---

## UNOIN

``` SQL
SELECT 1 AS NUM
UNION
SELECT 2 AS NUM;
```

[union]は出力テーブルの構造が同じでないといけない。  

[union]は重複チェックする。  
[union all]は重複チェックしない。  

なので、速度的にはunion all のほうが早い。  

---

## COUNT

[【SQL】COUNTの使い方（レコード数取得）](https://medium-company.com/sql-count/#:~:text=COUNT%E9%96%A2%E6%95%B0%E3%81%AE%E5%BC%95%E6%95%B0%E3%81%AB%E5%88%97%E5%90%8D%E3%82%92%E6%8C%87%E5%AE%9A%E3%81%99%E3%82%8B,%E3%81%99%E3%82%8B%E3%81%93%E3%81%A8%E3%81%8C%E3%81%A7%E3%81%8D%E3%81%BE%E3%81%99%E3%80%82&text=%E3%80%8CID%3D%221005%22%E3%80%8D,%E7%B5%90%E6%9E%9C%E3%81%8C%E5%8F%96%E5%BE%97%E3%81%A7%E3%81%8D%E3%81%BE%E3%81%99%E3%80%829)  

### COUNT(*) : 件数を取得

COUNT関数の引数に*(アスタリスク)を指定することで、レコード数を取得することができます。

### COUNT(列名）:NULLを除いた件数を取得

COUNT関数の引数に列名を指定することで、指定した列がNULL以外のレコード数を取得することができます。

### COUNT(DISTINCT 列名）: 重複を除いた件数を取得

COUNT関数の引数にDISTINCT 列名を指定することで、重複を除いたレコード数を取得することができます。

### COUNT(*)の意味とNULLのCOUNT

[COUNT(*)　が何を意味しているのかわからない](https://ja.stackoverflow.com/questions/42915/count-%E3%81%8C%E4%BD%95%E3%82%92%E6%84%8F%E5%91%B3%E3%81%97%E3%81%A6%E3%81%84%E3%82%8B%E3%81%AE%E3%81%8B%E3%82%8F%E3%81%8B%E3%82%89%E3%81%AA%E3%81%84)  
→そもそも構文エラーになる。  
**COUNT(*)は行数を数えてくれる**  
COUNT()は、行数を数えて出力する集計関数です。→平成27年秋期 午後問3の解説より  

``` txt
OracleではCOUNT(*)とCOUNT(age)の結果は異なります。
ageにnullが入っているとCOUNT(age)では件数にカウントされません。
グループ化していても同様で、ageがnullのグループのみ0件となります。
COUNT(*)ではageにnullが入っていてもレコードの件数をカウントします。

COUNT(*)ではレコードの内容を取得するため、COUNT('X')やSUM(1)を使った方が高速化できると教わったことがあります。(10年ほど前に聞いたノウハウなので現在も適用されるのかは不明ですが…)
```

なるほど。COUNTはNULLはカウントしないのね。  
動作的にCOUNT(name)見たいにフィールド名を指定したほうが高速化できるっぽいけど、単純にレコード数を取得したいならCOUNT(*)でいいのか。  

### COUNT(*)とCOUNT(カラム名)の違い

基本情報27年春の問題にて遭遇。  
なんだかんだわかってなかったのでまとめ。  

[【SQL】COUNT(*)とCOUNT(カラム名)の違い](https://qiita.com/TomoProg/items/5ba5779b3015ac02f577)  

・COUNT(*)はNULL値かどうかに関係なく、取得された行の数を返す  
・COUNT(カラム名)はNULL値でない値の行数を返す  

``` txt
+----+--------+-------+
| id | name   | price |
+----+--------+-------+
|  1 | apple  |   100 |
|  2 | banana |   120 |
|  3 | grape  |   140 |
|  4 | melon  |  NULL |
|  5 | kiwi   |   120 |
+----+--------+-------+
```

select count(*) from shohin; →そもそも構文エラーになる。 5  
select count(price) from shohin; →そもそも構文エラーになる。 4  

これも基本情報27年春の問題にて遭遇。  

[SQL | COUNT(DISTINCT column_name) は「同じ値の種類数」をカウントする](https://qiita.com/YumaInaura/items/1a1123ed4f33d30d9548)  
初歩だって。トホホ・・・。  
[COUNT句内でDISTINCTを使う／重複を排除したカウント](https://nyoe3.hatenadiary.org/entry/20100313/1268468670)  

つまり、重複行を除いたカウントをしたい場合に有効な構文というわけだ。  
そうなると次はDISTINCTとはどこまで含めることができるのか気になってきたぞ。  

``` txt
+-------+--------+-------+
| name  | sex    | score |
+-------+--------+-------+
| Alice | female |    60 |
| Bob   | male   |    70 |
| Carol | female |    70 |
| David | male   |    80 |
| Eric  | male   |    80 |
+-------+--------+-------+
```

sex には male / famale の二種類がある。  
SELECT COUNT(DISTINCT(sex)) AS sex_kind FROM scores; →そもそも構文エラーになる。 2  

score には 60点 / 70点 / 80点の三種類がある。  
る。 3  

こんな感じでも書ける  
なる。 5,2  

---

## SUM

GROUP BY しなくても合計を求めることができる。  

```sql
select SUM(RoundCount)  
from TRe_ReservationFrame  
where BusinessDate = '20210215'  
```

---

## LIKE句

LIKE句は、指定したパターンと文字列比較を行うための演算子で、次の特殊記号を用いて文字列のパターンを指定します。  

- `_` … 任意の1文字  
- `%` … 0文字以上の任意の文字列  

### 基本情報技術者過去問題 平成31年春期 午後問3

エ : `= '201%'`  

ワイルドカードを使用した文字列は、LIKE句と同時に使用しなければ効果を生じません。  
年度が"201%"の行は存在しませんので結果は0行になります。  
エを選択したけど、との事。  

`_`が任意の1文字だとは知らなかった。  
つまり、普段よく使っている`LIKE '%○○%'`は、どこでもいいから○○があるかどうかを調べているってわけか。  

---

## ANY句

平成27年秋午後のデータベースより。  
ANY句なんて見慣れない句が出てきたのでまとめた。

最初はIN句と何が違うのかわからなかったが、副問い合わせの結果を条件として使うことができることがわかった。  
わかれば以外に便利そう。  
まぁ、使ったことないんだけどね。  

[ANY(SOME)句を用いた副問合せ](https://www.sql-reference.com/select/subquery_any.html)  
例としてANY句を使用して受注テーブルからレコードを抽出します．  

``` txt : 受注テーブル
注文番号,商品コード,受注個数
01-101,A001,100
01-102,A002,200
01-103,B001,300
01-104,B002,400
02-101,A001,150
02-102,A002,350
```

``` SQL
SELECT * FROM 受注
WHERE 受注個数 > ANY (
    SELECT 受注個数
    FROM 受注
    WHERE 商品コード = 'A002' (
)
```

``` txt 結果
注文番号,商品コード,受注個数
01-103,B001,300
01-104,B002,400
02-102,A002,350
```

---

## 相関副問合せ(相関サブクエリ)

<https://atmarkit.itmedia.co.jp/ait/articles/1703/01/news187.html>  
副問合せの文中で、副問合せの外側の属性（検索結果）を利用して検索している問合せを、相関副問合せと呼びます。  
一般的には、パフォーマンスは悪くなる見たい。  

``` SQL
SELECT 社員番号,社員名 FROM 社員 AS S1
WHERE 生年月日 > (SELECT MIN(生年月日) FROM 社員 AS S2 WHERE S1.性別 = S2.性別)
-- サブクエリ中でメインクエリのS1テーブルを参照している。

-- EXISTSを使えば自動的に相関副問い合わせになる。
SELECT * FROM USER_MASTER A
WHERE EXISTS (SELECT * FROM AUTHORIZATION B WHERE A.USER_ID = B.USER_ID )
```

処理の流れ  
（1）外側のSELECT文を1行分だけ実行  
（2）取り出した表を副問合せに代入して実行  
（3）外側のSELECT文における、1行目のWHERE句の判定を行う  

---

## SQLの結合条件のON句の順番について

<https://detail.chiebukuro.yahoo.co.jp/qa/question_detail/q11166323581>  

SQLのLEFT JOIN とかのONの順番は関係無いらしい。  
地味に知らなかったので、メモ。  

---

## CROSS JOIN+WHERE と INNER JOIN

<https://qiita.com/zaburo/items/548b3c40fee68cd1e3b7>  
<https://stackoverflow.com/questions/17759687/cross-join-vs-inner-join-in-sql>  

CROSS JOIN して WHERE で絞る方法(等価結合)とINNER JOINの結果は同じらしい。(厳密には内部処理的には違うらしいが)  
まぁ、CROSS JOINしてWHEREで絞るくらいなら素直にINNER JOIN使えって話。  
RN2.23では結構そういうことしてて、どういう挙動をするのかわからなかったのでやってみた。  

``` SQL
-- どちらも結果は同じになる

-- CROSS JOIN + WHERE
SELECT depts.dept_name,employees.name
FROM depts,employees
WHERE depts.dept_id = employees.dept_id;

-- INNER JOIN
SELECT depts.dept_name,employees.name
FROM depts INNER JOIN employees
WHERE depts.dept_id = employees.dept_id;
```

---

## NULLの扱い

### NULLのLIKE検索

LIKE検索には引っかからない。  
NULLを検索したかったからISNULLを使うこと。  

### NULLのORDERBY

[NULLと戯れる: ORDER BYとNULL](https://qiita.com/SVC34/items/c23341c79325a0a95979)  

どうにも、NULLを最小値とするか最大値とするかは、RDBMS毎に違ったり、設定で変更出来たりするみたい。  
Oracleは最大値扱いだが、SQLServerは最小値扱い見たい。  
まぁ、どちらにせよ、先頭か末尾であることに違いはないということですね。  

### NULLをキャスト

(NULL AS CHAR)→NULLのまま

### NULLとの演算

全てNULLになる  

`NULL -1 = NULL`  

### NULLのSUM

SUMはNULLが1件でも含まれると結果がNULLになる。

---

## insert intoのinto句のありなしの違い

[insert intoのinto句のありなしって違いは何ですか？](https://detail.chiebukuro.yahoo.co.jp/qa/question_detail/q1049319100)  
>MySQL、SQL Server、ACCESS では、 into は省略可能です。  
Oracle では、省略不可です。  

---

## N'文字列'の意味

1. Unicodeを使うという宣言・マーキング  
2. 日本語を使う場合に必須  

[SQLのNプレフィックスっていったい何？](http://once-and-only.com/programing/sql/sql%E3%81%AEn%E3%83%97%E3%83%AC%E3%83%95%E3%82%A3%E3%83%83%E3%82%AF%E3%82%B9%E3%81%A3%E3%81%A6%E3%81%84%E3%81%A3%E3%81%9F%E3%81%84%E4%BD%95%EF%BC%9F/)

>SQL Server で Unicode 文字列定数を扱う場合には、Unicode 文字列の前に大文字の N が必ず必要です。  
>これは、SQL Server Books Online の「Unicode データの使用」で説明されています。  
>”N” プレフィックスは、SQL-92 標準の National Language を意味し、必ず大文字にする必要があります。  
>Unicode 文字列定数の前に N を付加しない場合、その文字列は、SQL Server によって、使用される前に現在のデータベースの Unicode 以外のコードページに変換されます。  

※Unicodeは何故UじゃなくてNなのか？  
→  
National Languageという意味でUnicodeが採用されているため。  
National Languageは国語という意味ではなく「様々な国の文字」というニュアンス。  

[MS SQLServer のSQLで文字列の前にN:](https://oshiete.goo.ne.jp/qa/280266.html)

>N'***' とT-SQL内で書くと、''内の文字をUnicodeで表現されたものとして処理する、という意味になります。  
>Nは、nationalの略です。  
>ですから、日本語を使おうとするとNは必須になる、という事ですね。  
>こんな感じでつかいます。＃N'Unicode 文字列'  

---

## DISTINCTとワイルドカードの併用

DISTINCTとワイルドカード `*` を併用したら.NETFrameworkでは実行速度が遅くなるらしい  

---

## REPLACE

[SQLServerのREPLACE 文字列を置換する](https://sql-oracle.com/sqlserver/?p=195)  

``` sql
--文字列'SATOU'をREPLACEで連結する
--[結果] 'KATOU'
SELECT REPLACE('SATOU','S','K') ;
```

``` sql
--置換対象がない
--[結果] 'SATOU'
SELECT REPLACE('SATOU','Z','Y') ;
```

ここではREPLACEを使って「SATOU」の「Z」を「Y」に置換しようとしました。  
しかし、「SATOU」には「Z」が含まれないので「Y」には置換されませんでした。  
置換する対象がない場合はそのままの文字列が返ってきます。  

---

## NULLを排除した設計

[NULLを排除した設計①](http://onefact.jp/wp/2014/08/26/null%E3%82%92%E6%8E%92%E9%99%A4%E3%81%97%E3%81%9F%E8%A8%AD%E8%A8%88/)  

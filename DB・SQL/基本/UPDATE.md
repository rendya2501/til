
# UPDATE

<https://qiita.com/ryota_i/items/d17c7630bacb36d26864>  
>特定テーブルにおける、条件に当てはまるレコードの特定のカラムの値を任意の値に書き換える。  

``` sql : 基本
UPDATE テーブル名
SET 列名1 = 値1 [,列名2 = 値2]・・・
WHERE (条件);
```

---

## 副問い合わせを使ったUPDATE

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

---

## SQLServerにおけるUPDATE JOIN

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

---

## UPDATE JOIN の他のパターン

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

---

## UPDATEでCASE式

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

---

## UPDATEでWITH句を使う

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

---

## 行けそうだけどいけないUPDATE

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


-- Withで書いたら多分こうなる。
-- ここまで長ったらしいサブクエリを切り出すならWITHを使ったほうが見やすいかもしれない。
WITH SQ AS ( 
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
)
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
    JOIN [SQ]
    ON [TB_会員_Test].[顧客CD] = [SQ].[顧客CD]
```

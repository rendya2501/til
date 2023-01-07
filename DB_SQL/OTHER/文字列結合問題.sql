
-- 親予約テーブル
CREATE TABLE #ParentReservation(
    MainReservationNo NVARCHAR(255),
    Comment NVARCHAR(255),
)
INSERT INTO #ParentReservation 
VALUES
    ('AAA001','CommentC'),
    ('AAA002','CommentX')

-- 子予約テーブル
CREATE TABLE #ChildReservation(
    MainReservationNo NVARCHAR(255),
    SubReservationNo  NVARCHAR(255),
    BusinessDate DATETIME,
    Comment NVARCHAR(255),
)
INSERT INTO #ChildReservation
VALUES
    ('AAA001','0001','20221001','CommentA'),
    ('AAA001','0002','20221001','CommentC'),
    ('AAA001','0003','20221002','CommentB'),
    ('AAA001','0004','20221002','CommentBB'),
    ('AAA001','0005','20221003','CommentD'),
    ('AAA001','0006','20221003','CommentD'),
    ('AAA001','0007','20221003','CommentDCC'),
    ('AAA002','0001','20221001','CommentA'),
    ('AAA002','0002','20221001','CommentB'),
    ('AAA002','0003','20221002','CommentC'),
    ('AAA002','0004','20221003','CommentD')

-- 問題
-- ParentReservationテーブルとChildReservationテーブルがある。
-- ChildReservationテーブルを日付ごとに集約して、コメントを横に結合したい。
-- ただし、同じコメントは排除する。
-- また、結合の順番はSubReservationNo順とする。
-- そして同じMainReservationNoを持つParentReservationテーブルのコメントがChildReservationテーブルに存在しない場合、
-- ParentReservationテーブルのコメントを先頭に持ってくる。
-- その後に、ChildReservationテーブルの集約したコメントを結合する。
-- 先頭のコメントは対象のMainReservationNoの全てのカラムとする。
-- この条件を満たす結果は以下の通りとなる。

-- MainReservationNo | BusinessDate | Commnet
-- ------------------+--------------+----------------------------
-- AAA001            | 2022-10-01   | CommentA CommentC
-- AAA001            | 2022-10-02   | CommentB CommentBB
-- AAA001            | 2022-10-03   | CommentD CommentDCC
-- AAA002            | 2022-10-01   | CommentX CommentA CommentB
-- AAA002            | 2022-10-02   | CommentX CommentC
-- AAA002            | 2022-10-03   | CommentX CommentD

-- MainReservationNo:AAA002の親テーブルのコメントが子テーブルにないので、
-- MainReservationNo:AAA002の全てのレコードの先頭に親テーブルのコメントが結合されている。
-- AAA001 2022-10-03 のCommentDは重複しているので排除されている事がわかる。


-- 回答1. 一番最初に思いついてまとめた案
-- まず、子テーブルに存在しないコメントを持った親をCTEとして抜き出す。
-- その後、子テーブルを日付で集約しつつ、先ほどのCTEをLEFT JOINして各種結合を行う。
-- 存在判定には主にJOINを使用している。
-- 日付の集約、重複の排除にはDISTINCTとWINDOW関数のRANKを用いる事で実現した。
-- 一応、結果にはなるものの、長いのと分かりにくい。

WITH NotDuplicateParent
AS (
    -- AAA002  CommentX
    SELECT
        [Parent].[MainReservationNo],
        [Parent].[Comment]
    FROM
        [#ParentReservation] AS [Parent]
        JOIN (
            SELECT
                [Parent].[MainReservationNo]
            FROM 
                [#ChildReservation] AS [Child]
                LEFT JOIN [#ParentReservation] AS [Parent]
                ON [Child].[MainReservationNo] = [Parent].[MainReservationNo]
                AND [Child].[Comment] = [Parent].[Comment]
            WHERE
                [Parent].[MainReservationNo] IS NOT NULL
        ) AS [SQ]
        ON [Parent].[MainReservationNo] <> [SQ].[MainReservationNo]
),
Result
AS (
    SELECT
        GroupedChild.MainReservationNo,
        GroupedChild.BusinessDate,
        LTRIM(CONCAT(ISNULL(NotDuplicateParent.Comment,''),' ',GroupedChild.Comment)) AS Commnet
    FROM (
        -- AAA001  2022-10-01 00:00:00.000  CommentA CommentC
        -- AAA001  2022-10-02 00:00:00.000  CommentB CommentBB
        -- AAA001  2022-10-03 00:00:00.000  CommentD CommentDCC
        -- AAA002  2022-10-01 00:00:00.000  CommentA CommentB
        -- AAA002  2022-10-02 00:00:00.000  CommentC
        -- AAA002  2022-10-03 00:00:00.000  CommentD
        SELECT DISTINCT
            MainReservationNo,
            BusinessDate,
            Comment = LTRIM((
                SELECT
                    ' ' + Comment
                FROM
                    (
                    SELECT 
                        Child.*, 
                        RANK() OVER(PARTITION BY MainReservationNo,BusinessDate,Comment ORDER BY SubReservationNo) AS rk
                    FROM
                        #ChildReservation AS Child
                    ) AS t2
                WHERE
                    t2.MainReservationNo = t1.MainReservationNo
                    AND t2.BusinessDate = t1.BusinessDate
                    AND t2.rk = 1
                ORDER BY
                    t2.SubReservationNo FOR XML PATH('')
            ))
        FROM
            #ChildReservation AS t1
    ) AS GroupedChild
    LEFT JOIN NotDuplicateParent
    ON GroupedChild.MainReservationNo = NotDuplicateParent.MainReservationNo
)
SELECT * FROM Result




-- 回答2. 最終案 (相関服問い合わせ + GROUP BY)
--
-- まずCTEをやめた。色々短くする事が出来て、わざわざCTEを作る必要がなくなったため。
-- 子テーブルに子供が存在するかの判定はNOT EXISTSで行うようにした。
-- 理由はこっちのほうが実行コストが低かったのと、ステートメントが短くて分かりやすいため。
-- 親テーブルの結合は相関服問い合わせで行う事でJOINすら必要なくなった。
-- なので、子テーブルを日付で集約しつつ、相関服問い合わせで親テーブルのコメントを結合しつつ、
-- 子テーブルのコメントを横に結合することができるようになった。
--
-- 初期案と最終案のコスト比は 70%:30%
-- 同じ性能であれば50%:50%なので、20%の性能向上に成功

SELECT
    MainReservationNo,
    BusinessDate,
    Commnet = CONCAT(
        (
            SELECT
                ISNULL([Parent].[Comment],'')
            FROM
                [#ParentReservation] AS [Parent]
            WHERE
                NOT EXISTS(
                    SELECT
                        *
                    FROM 
                        [#ChildReservation] AS [Child]
                    WHERE
                        [Child].[MainReservationNo] = [Parent].[MainReservationNo]
                        AND [Child].[Comment] = [Parent].[Comment]
                )
                AND [Parent].[MainReservationNo] = t1.[MainReservationNo]
        ),
        ' ',
        LTRIM(
            (
                SELECT DISTINCT
                    ' ' + Comment
                FROM
                    #ChildReservation AS t2
                WHERE
                    t2.MainReservationNo = t1.MainReservationNo
                    AND t2.BusinessDate = t1.BusinessDate
                FOR XML PATH('')
            )
        )
    )
FROM
    #ChildReservation t1
GROUP BY
    t1.MainReservationNo,t1.BusinessDate





---------------------------------------------------------------------------------------
-- 以下試行錯誤の跡
---------------------------------------------------------------------------------------


-- FIRST VALUE案
-- RANK使って1行目を取得するなら最初のを取得すれば良いのでは？という案
WITH NotDuplicateParent
AS (
    SELECT
        [Parent].[MainReservationNo],
        [Parent].[Comment]
    FROM
        [#ParentReservation] AS [Parent]
        JOIN (
            SELECT
                [Parent].[MainReservationNo]
            FROM 
                [#ChildReservation] AS [Child]
                LEFT JOIN [#ParentReservation] AS [Parent]
                ON [Child].[MainReservationNo] = [Parent].[MainReservationNo]
                AND [Child].[Comment] = [Parent].[Comment]
            WHERE
                [Parent].[MainReservationNo] IS NOT NULL
        ) AS [SQ]
        ON [Parent].[MainReservationNo] <> [SQ].[MainReservationNo]
),
Result
AS (
    SELECT
        GroupedChild.MainReservationNo,
        GroupedChild.BusinessDate,
        LTRIM(CONCAT(ISNULL(NotDuplicateParent.Comment,''),' ',GroupedChild.Comment)) AS Commnet
    FROM (
        SELECT DISTINCT
            MainReservationNo,
            BusinessDate,
            Comment = LTRIM((
                SELECT DISTINCT
                    ' ' + Comment
                FROM
                    (
                    SELECT 
                        Child.*, 
                        FIRST_VALUE(Comment) OVER(PARTITION BY MainReservationNo,BusinessDate,Comment ORDER BY SubReservationNo) AS rk
                    FROM
                        #ChildReservation AS Child
                    ) AS t2
                WHERE
                    t2.MainReservationNo = t1.MainReservationNo
                    AND t2.BusinessDate = t1.BusinessDate
                    FOR XML PATH('')
            ))
        FROM
            #ChildReservation AS t1
    ) AS GroupedChild
    LEFT JOIN NotDuplicateParent
    ON GroupedChild.MainReservationNo = NotDuplicateParent.MainReservationNo
)
SELECT * FROM Result
GO



-- DISTINCT案1
-- RANKやFIRSTVALUEを使うのは重複を排除したいからであるなら、DISTINCTで良くない？という案
WITH NotDuplicateParent
AS (
    SELECT
        [Parent].[MainReservationNo],
        [Parent].[Comment]
    FROM
        [#ParentReservation] AS [Parent]
        JOIN (
            SELECT DISTINCT
                [Parent].[MainReservationNo]
            FROM 
                [#ChildReservation] AS [Child]
                LEFT JOIN [#ParentReservation] AS [Parent]
                ON [Child].[MainReservationNo] = [Parent].[MainReservationNo]
                AND [Child].[Comment] = [Parent].[Comment]
            WHERE
                [Parent].[MainReservationNo] IS NOT NULL
        ) AS [SQ]
        ON [Parent].[MainReservationNo] <> [SQ].[MainReservationNo]
),
Result
AS (
    SELECT
        GroupedChild.MainReservationNo,
        GroupedChild.BusinessDate,
        LTRIM(CONCAT(ISNULL(NotDuplicateParent.Comment,''),' ',GroupedChild.Comment)) AS Commnet
    FROM (
        SELECT DISTINCT
            MainReservationNo,
            BusinessDate,
            Comment = LTRIM((
                SELECT
                    ' ' + Comment
                FROM
                    (
                    SELECT DISTINCT
                        Child.MainReservationNo, 
                        Child.BusinessDate, 
                        Child.Comment
                    FROM
                        #ChildReservation AS Child
                    ) AS t2
                WHERE
                    t2.MainReservationNo = t1.MainReservationNo
                    AND t2.BusinessDate = t1.BusinessDate
                    FOR XML PATH('')
            ))
        FROM
            #ChildReservation AS t1
    ) AS GroupedChild
    LEFT JOIN NotDuplicateParent
    ON GroupedChild.MainReservationNo = NotDuplicateParent.MainReservationNo
)
SELECT * FROM Result
GO



-- 子テーブルのGROUP BY案
-- そもそも子テーブルをGROUP BYしてしまえば1つサブクエリを消すことができるのでは？という案
-- ここまでやるならCTEで括る必要もないのでは？という。
-- ついでに親テーブルの判定もJOINからEXSITSにしている。これは間に挟んだ検証でEXISTSを採用したため。
SELECT
    GroupedChild.MainReservationNo,
    GroupedChild.BusinessDate,
    LTRIM(CONCAT(ISNULL(NotDuplicateParent.Comment,''),' ',GroupedChild.Comment)) AS Commnet
FROM (
    SELECT
        MainReservationNo,
        BusinessDate,
        Comment = LTRIM(
            (
                SELECT DISTINCT
                    ' ' + Comment
                FROM
                    #ChildReservation AS t2
                WHERE
                    t2.MainReservationNo = t1.MainReservationNo
                    AND t2.BusinessDate = t1.BusinessDate
                FOR XML PATH('')
            )
        )
    FROM
        #ChildReservation AS t1
    GROUP BY
        t1.MainReservationNo,t1.BusinessDate
    ) AS GroupedChild
    LEFT JOIN (
        SELECT
            [MainReservationNo],
            [Comment]
        FROM
            [#ParentReservation] AS [Parent]
        WHERE
            NOT EXISTS(
                SELECT
                    *
                FROM 
                    [#ChildReservation] AS [Child]
                WHERE
                    [Child].[MainReservationNo] = [Parent].[MainReservationNo]
                    AND [Child].[Comment] = [Parent].[Comment]
            )
    ) NotDuplicateParent
ON GroupedChild.MainReservationNo = NotDuplicateParent.MainReservationNo



-- JOIN後にDISTINCT案
-- GROUP BYではサブクエリにしないといけないので、DISTINCTであればさらに簡略化できるのでは？という案
-- 全てJOINした後にDISTINCTする事で実際に実現することができた。
SELECT DISTINCT
    t1.MainReservationNo,
    t1.BusinessDate,
    Commnet = CONCAT(
        ISNULL(NotDuplicateParent.Comment,''),
        ' ',
        LTRIM(
            (
                SELECT DISTINCT
                    ' ' + Comment
                FROM
                    #ChildReservation AS t2
                WHERE
                    t2.MainReservationNo = t1.MainReservationNo
                    AND t2.BusinessDate = t1.BusinessDate
                FOR XML PATH('')
            )
        )
    )
FROM
    #ChildReservation AS t1
    LEFT JOIN (
        SELECT
            [MainReservationNo],
            [Comment]
        FROM
            [#ParentReservation] AS [Parent]
        WHERE
            NOT EXISTS(
                SELECT
                    *
                FROM 
                    [#ChildReservation] AS [Child]
                WHERE
                    [Child].[MainReservationNo] = [Parent].[MainReservationNo]
                    AND [Child].[Comment] = [Parent].[Comment]
            )
    ) NotDuplicateParent
    ON t1.MainReservationNo = NotDuplicateParent.MainReservationNo



-- 相関服問い合わせ案
-- そもそも親テーブルのコメントもJOINではなく、SELECTステートメント中で副問い合わせしてしまえば良いのでは？という案
SELECT DISTINCT
    MainReservationNo,
    BusinessDate,
    Commnet = CONCAT(
        (
            SELECT
                ISNULL([Parent].[Comment],'')
            FROM
                [#ParentReservation] AS [Parent]
            WHERE
                NOT EXISTS(
                    SELECT
                        *
                    FROM 
                        [#ChildReservation] AS [Child]
                    WHERE
                        [Child].[MainReservationNo] = [Parent].[MainReservationNo]
                        AND [Child].[Comment] = [Parent].[Comment]
                )
                AND [Parent].[MainReservationNo] = t1.[MainReservationNo]
        ),
        ' ',
        LTRIM(
            (
                SELECT DISTINCT
                    ' ' + Comment
                FROM
                    #ChildReservation AS t2
                WHERE
                    t2.MainReservationNo = t1.MainReservationNo
                    AND t2.BusinessDate = t1.BusinessDate
                FOR XML PATH('')
            )
        )
    )
FROM
    #ChildReservation t1



-- 相関服問い合わせ + GROUP BY案
-- 今回の例ではDISTINCTよりGROUP BYのほうがコストが安い事が分かったのでGROUP BYにしてみた。
-- FOR XML を使う関係上、DISTINCTよりGROUP BYのほうが分かりやすいというのもあった。
-- というわけで、これが最終案となった。
SELECT
    MainReservationNo,
    BusinessDate,
    Commnet = CONCAT(
        (
            SELECT
                ISNULL([Parent].[Comment],'')
            FROM
                [#ParentReservation] AS [Parent]
            WHERE
                NOT EXISTS(
                    SELECT
                        *
                    FROM 
                        [#ChildReservation] AS [Child]
                    WHERE
                        [Child].[MainReservationNo] = [Parent].[MainReservationNo]
                        AND [Child].[Comment] = [Parent].[Comment]
                )
                AND [Parent].[MainReservationNo] = t1.[MainReservationNo]
        ),
        ' ',
        LTRIM(
            (
                SELECT DISTINCT
                    ' ' + Comment
                FROM
                    #ChildReservation AS t2
                WHERE
                    t2.MainReservationNo = t1.MainReservationNo
                    AND t2.BusinessDate = t1.BusinessDate
                FOR XML PATH('')
            )
        )
    )
FROM
    #ChildReservation t1
GROUP BY
    t1.MainReservationNo,t1.BusinessDate






---------------------------------------------------------------------------------------------
-- 以下、子テーブルに存在しないコメントを持った親テーブルを抜き出すクエリ部分の検証
---------------------------------------------------------------------------------------------

-- 1. 一番最初に思いついた案
-- 親テーブルと子テーブルをJOINした後、丁寧にNULLを排除してから再度親とJOIN
SELECT
    [Parent].[MainReservationNo],
    [Parent].[Comment]
FROM
    [#ParentReservation] AS [Parent]
    JOIN (
        SELECT DISTINCT
            [Parent].[MainReservationNo]
        FROM 
            [#ChildReservation] AS [Child]
            LEFT JOIN [#ParentReservation] AS [Parent]
            ON [Child].[MainReservationNo] = [Parent].[MainReservationNo]
            AND [Child].[Comment] = [Parent].[Comment]
        WHERE
            [Parent].[MainReservationNo] IS NOT NULL
    ) AS [SQ]
    ON [Parent].[MainReservationNo] <> [SQ].[MainReservationNo]



-- 2. わざわざNULLを排除しなくても問題ないのでは？と思って実戦してみた案
-- これでも普通に行けたが、第1案とコスト的な違いはほとんどなかった。
-- 1案 50%
-- 2案 50%
SELECT
    [Parent].[MainReservationNo],
    [Parent].[Comment]
FROM
    [#ParentReservation] AS [Parent]
    JOIN (
        SELECT DISTINCT
            [Parent].[MainReservationNo]
        FROM 
            [#ChildReservation] AS [Child]
            LEFT JOIN [#ParentReservation] AS [Parent]
            ON [Child].[MainReservationNo] = [Parent].[MainReservationNo]
            AND [Child].[Comment] = [Parent].[Comment]
    ) AS [SQ]
    ON [Parent].[MainReservationNo] <> [SQ].[MainReservationNo]



-- 3. NOT EXISTS案
-- 純粋に子テーブルにない組み合わせを取得したいだけならEXSISTSでは？と思った案
-- 一番コストが安い事が分かった
-- 2案 76%
-- 3案 24%
SELECT
    [MainReservationNo],
    [Comment]
FROM
    [#ParentReservation] AS [Parent]
WHERE
    NOT EXISTS(
        SELECT
            *
        FROM 
            [#ChildReservation] AS [Child]
        WHERE
            [Child].[MainReservationNo] = [Parent].[MainReservationNo]
            AND [Child].[Comment] = [Parent].[Comment]
    )




---------------------------------------------------------------------------------------------
-- 以下、大昔の検証跡
---------------------------------------------------------------------------------------------

-- WITH Parent
-- AS (
--     SELECT 'AAA001' AS IntegratedReservationNo, NULL AS ReservationNo, NULL AS BusinessDate, 'CommentC' AS Comment
--     UNION
--     SELECT 'AAA002' AS IntegratedReservationNo, NULL AS ReservationNo, NULL AS BusinessDate, 'CommentX' AS Comment
-- ),
-- Child
-- AS (
--     SELECT 'AAA001' AS IntegratedReservationNo,'0001' AS ReservationNo, '20221001' AS BusinessDate, 'CommentA' AS Comment
--     UNION
--     SELECT 'AAA001','0002', '20221001', 'CommentC'
--     UNION
--     SELECT 'AAA001','0003', '20221002', 'CommentB'
--     UNION
--     SELECT 'AAA001','0004', '20221003', 'CommentD'
--     UNION
--     SELECT 'AAA002','0001', '20221001', 'CommentA'
--     UNION
--     SELECT 'AAA002','0002', '20221001', 'CommentB'
--     UNION
--     SELECT 'AAA002','0003', '20221002', 'CommentC'
--     UNION
--     SELECT 'AAA002','0004', '20221003', 'CommentD'
-- ),
-- val1
-- AS (
--     SELECT DISTINCT
--         IntegratedReservationNo,
--         BusinessDate,
--         Comment = LTRIM((
--             SELECT
--                 ' ' + Comment
--             FROM
--                 Child t2
--             WHERE
--                 t2.IntegratedReservationNo = t1.IntegratedReservationNo
--                 AND t2.BusinessDate = t1.BusinessDate
--             ORDER BY
--                 t2.ReservationNo FOR XML PATH('')
--         ))
--     FROM
--         Child t1
-- ),
-- val2 
-- AS (
--     SELECT
--         T1.IntegratedReservationNo,
--         T1.BusinessDate,
--         LTRIM(CONCAT(ISNULL(T2.Comment,''),' ',T1.Comment)) AS Commnet
--     FROM val1 AS T1
--     LEFT JOIN Parent AS [T2]
--     ON T1.IntegratedReservationNo = T2.IntegratedReservationNo
-- )
-- SELECT * FROM val1


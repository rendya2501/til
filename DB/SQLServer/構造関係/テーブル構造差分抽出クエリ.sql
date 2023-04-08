DECLARE @SourceDataBaseName NVARCHAR(MAX) = 'SourceDataBaseName'
DECLARE @TargetDataBaseName NVARCHAR(MAX) = 'TargetDataBaseName'

-- FromとToのテーブル存在確認
-- ○ : 存在する
-- × : 存在しない
exec('
SELECT
    COALESCE([SQ_Source_Table].[TABLE_NAME], [SQ_Target_Table].[TABLE_NAME]) AS ObjectName,
    CASE
        WHEN [SQ_Source_Table].[TABLE_NAME] IS NOT NULL THEN ''○''
        ELSE ''×''
    END AS [FromStatus],
    CASE
        WHEN [SQ_Target_Table].[TABLE_NAME] IS NOT NULL THEN ''○''
        ELSE ''×''
    END AS [ToStatus]
FROM
    (
        SELECT
            [TABLE_NAME]
        FROM
            [' + @SourceDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Source_Table]
        GROUP BY
            [TABLE_NAME]
    ) AS [SQ_Source_Table]
    FULL OUTER JOIN
        (
            SELECT
                [TABLE_NAME]
            FROM
                [' + @TargetDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Target_Table]
            GROUP BY
                [TABLE_NAME]
        ) AS [SQ_Target_Table]
    ON  [SQ_Source_Table].[TABLE_NAME] = [SQ_Target_Table].[TABLE_NAME]
WHERE
    [SQ_Source_Table].[TABLE_NAME] IS NULL
OR  [SQ_Target_Table].[TABLE_NAME] IS NULL
ORDER BY
    ObjectName
')



-- 変更のあるカラム一覧
-- FromとToの両方で差異があるカラム一覧を表示する
exec('
WITH SourceResult AS(
    SELECT
        [Source_Table].[TABLE_NAME],
        [Source_Table].[COLUMN_NAME],
        [Source_Table].[DATA_TYPE],
        [Source_Table].[CHARACTER_MAXIMUM_LENGTH],
        [Source_Table].[IS_NULLABLE],
        (CASE WHEN [Source_Key].[COLUMN_NAME] IS NOT NULL THEN ''Key'' ELSE NULL END) AS [Key]
    FROM
        [' + @SourceDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Source_Table]
        LEFT OUTER JOIN
            [INFORMATION_SCHEMA].[KEY_COLUMN_USAGE] AS [Source_Key]
        ON  [Source_Table].[TABLE_NAME] = [Source_Key].[TABLE_NAME]
        AND [Source_Table].[COLUMN_NAME] = [Source_Key].[COLUMN_NAME]
),
TargetResult AS(
    SELECT
        [Target_Table].[TABLE_NAME],
        [Target_Table].[COLUMN_NAME],
        [Target_Table].[DATA_TYPE],
        [Target_Table].[CHARACTER_MAXIMUM_LENGTH],
        [Target_Table].[IS_NULLABLE],
        (CASE WHEN [Target_Key].[COLUMN_NAME] IS NOT NULL THEN ''Key'' ELSE NULL END) AS [Key]
    FROM
        [' + @TargetDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Target_Table]
        LEFT OUTER JOIN
            [INFORMATION_SCHEMA].[KEY_COLUMN_USAGE] AS [Target_Key]
        ON  [Target_Table].[TABLE_NAME] = [Target_Key].[TABLE_NAME]
        AND [Target_Table].[COLUMN_NAME] = [Target_Key].[COLUMN_NAME]
),
Comparison AS(
    SELECT
        CASE
            WHEN NOT EXISTS(
                SELECT
                    1
                FROM
                    [' + @SourceDataBaseName + '].INFORMATION_SCHEMA.TABLES
                WHERE
                    TABLE_NAME = COALESCE(S.[TABLE_NAME], T.[TABLE_NAME])
            ) THEN ''FromTable is Missing''
            WHEN NOT EXISTS(
                SELECT
                    1
                FROM
                    [' + @TargetDataBaseName + '].INFORMATION_SCHEMA.TABLES
                WHERE
                    TABLE_NAME = COALESCE(S.[TABLE_NAME], T.[TABLE_NAME])
            ) THEN ''ToTable is Missing''
            WHEN S.[COLUMN_NAME] IS NULL THEN ''FromColumn is Missing''
            WHEN T.[COLUMN_NAME] IS NULL THEN ''ToColumn is Missing''
            WHEN S.[COLUMN_NAME] <> T.[COLUMN_NAME]
        OR  S.[DATA_TYPE] <> T.[DATA_TYPE]
        OR  S.[CHARACTER_MAXIMUM_LENGTH] <> T.[CHARACTER_MAXIMUM_LENGTH]
        OR  S.[IS_NULLABLE] <> T.[IS_NULLABLE]
        OR  S.[Key] <> T.[Key] THEN ''Column Diff''
            ELSE ''Match''
        END AS Comparison,
        COALESCE(S.[TABLE_NAME], T.[TABLE_NAME]) AS [TABLE_NAME],
        COALESCE(S.[COLUMN_NAME], T.[COLUMN_NAME]) AS [COLUMN_NAME],
        ''|'' AS ''|||'',
        S.[DATA_TYPE],
        S.[CHARACTER_MAXIMUM_LENGTH],
        S.[IS_NULLABLE],
        S.[Key],
        ''→'' AS [To],
        T.[COLUMN_NAME] AS [To_COLUMN_NAME],
        T.[DATA_TYPE] AS [TO_DATA_TYPE],
        T.[CHARACTER_MAXIMUM_LENGTH] AS [TO_CHARACTER_MAXIMUM_LENGTH],
        T.[IS_NULLABLE] AS [TO_IS_NULLABLE],
        T.[Key] AS [TO_KEY]
    FROM
        SourceResult AS S
        FULL OUTER JOIN
            TargetResult AS T
        ON  S.[TABLE_NAME] = T.[TABLE_NAME]
        AND S.[COLUMN_NAME] = T.[COLUMN_NAME]
)
SELECT
    *
FROM
    Comparison
WHERE
    Comparison.Comparison NOT IN(''Match'', ''FromTable is Missing'', ''ToTable is Missing'')
ORDER BY
    [TABLE_NAME]
')



-- インデックスの差異一覧
-- FromとToの両方で差異があるインデックス一覧を表示する
-- 自身のテーブルがそもそも存在しない → FromTable is Missing
-- 自身のテーブルは存在するが相手のindexが存在しない → ToIndex is Missing
-- 相手のテーブルがそもそも存在しない → ToTable is Missing
-- 相手のテーブルは存在するが自身のindexが存在しない → FromIndex is Missing
-- 自身と相手のテーブルとインデックスは存在するが、内容が違う → IndexDiff
exec('
WITH SourceResult AS(
    SELECT
        [O].[name] AS [ObjectName],
        [I].[name] AS [IndexName],
        [I].[type_desc] AS [IndexTypeDesc],
        [I].[is_primary_key] AS [IsPrimaryKey],
        [I].[is_unique] AS [IsUnique],
        [I].[is_disabled] AS [IsDisabled],
        [IndexKeys] = STUFF((
                SELECT
                    CONCAT(
                        '', '',
                        [AC].[name],
                        CASE
                            WHEN [is_descending_key] = 1 THEN '' - DESC''
                            ELSE ''''
                        END
                    )
                FROM
                    [' + @SourceDataBaseName + '].[sys].[index_columns] AS [IC]
                    JOIN
                        [' + @SourceDataBaseName + '].[sys].[all_columns] AS [AC]
                    ON  [IC].[object_id] = [AC].[object_id]
                    AND [IC].[column_id] = [AC].[column_id]
                WHERE
                    [IC].[object_id] = [I].[object_id]
                AND [IC].[index_id] = [I].[index_id]
                AND [is_included_column] = 0 FOR XML PATH(''''),
                    TYPE
            ).value(''.'', ''VARCHAR(MAX)''), 1, 2, '''')
    FROM
        [' + @SourceDataBaseName + '].[sys].[indexes] AS [I]
        INNER JOIN
            [' + @SourceDataBaseName + '].[sys].[objects] AS [O]
        ON  [I].[object_id] = [O].[object_id]
    WHERE
        [I].[index_id] > 0
    AND [O].[is_ms_shipped] = 0
),
TargetResult AS(
    SELECT
        [O].[name] AS [ObjectName],
        [I].[name] AS [IndexName],
        [I].[type_desc] AS [IndexTypeDesc],
        [I].[is_primary_key] AS [IsPrimaryKey],
        [I].[is_unique] AS [IsUnique],
        [I].[is_disabled] AS [IsDisabled],
        [IndexKeys] = STUFF((
                SELECT
                    CONCAT(
                        '', '',
                        [AC].[name],
                        CASE
                            WHEN [is_descending_key] = 1 THEN '' - DESC''
                            ELSE ''''
                        END
                    )
                FROM
                    [' + @TargetDataBaseName + '].[sys].[index_columns] AS [IC]
                    JOIN
                        [' + @TargetDataBaseName + '].[sys].[all_columns] AS [AC]
                    ON  [IC].[object_id] = [AC].[object_id]
                    AND [IC].[column_id] = [AC].[column_id]
                WHERE
                    [IC].[object_id] = [I].[object_id]
                AND [IC].[index_id] = [I].[index_id]
                AND [is_included_column] = 0 FOR XML PATH(''''),
                    TYPE
            ).value(''.'', ''VARCHAR(MAX)''), 1, 2, '''')
    FROM
        [' + @TargetDataBaseName + '].[sys].[indexes] AS [I]
        INNER JOIN
            [' + @TargetDataBaseName + '].[sys].[objects] AS [O]
        ON  [I].[object_id] = [O].[object_id]
    WHERE
        [I].[index_id] > 0
    AND [O].[is_ms_shipped] = 0
),
Comparison AS(
    SELECT
        CASE
            WHEN NOT EXISTS(SELECT 1 FROM [' + @SourceDataBaseName + '].INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = COALESCE(S.ObjectName, T.ObjectName)) THEN ''FromTable is Missing''
            WHEN NOT EXISTS(SELECT 1 FROM [' + @TargetDataBaseName + '].INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = COALESCE(S.ObjectName, T.ObjectName)) THEN ''ToTable is Missing''
            WHEN S.IndexName IS NULL THEN ''FromIndex is Missing''
            WHEN T.IndexName IS NULL THEN ''ToIndex is Missing''
            WHEN S.IndexTypeDesc <> T.IndexTypeDesc
            OR  S.IsPrimaryKey <> T.IsPrimaryKey
            OR  S.IsUnique <> T.IsUnique
            OR  S.IsDisabled <> T.IsDisabled
            OR  S.IndexKeys <> T.IndexKeys THEN ''IndexDiff''
            ELSE ''Match''
        END AS Comparison,
        COALESCE(S.ObjectName, T.ObjectName) AS ObjectName,
        COALESCE(S.IndexName, T.IndexName) AS IndexName,
        S.ObjectName as a,
        S.IndexName as ss,
        S.IndexTypeDesc,
        S.IsPrimaryKey,
        S.IsUnique,
        S.IsDisabled,
        S.IndexKeys,
        T.ObjectName as b,
        T.IndexName AS TargetIndexName,
        T.IndexTypeDesc AS TargetIndexTypeDesc,
        T.IsPrimaryKey AS TargetIsPrimaryKey,
        T.IsUnique AS TargetIsUnique,
        T.IsDisabled AS TargetIsDisabled,
        T.IndexKeys AS TargetIndexKeys
    FROM
        SourceResult AS S
        FULL OUTER JOIN
            TargetResult AS T
        ON  S.ObjectName = T.ObjectName
        AND S.IndexName = T.IndexName
)
SELECT
    Comparison,
    ObjectName,
    IndexName,
    ''|'' AS ''|||'',
    IndexTypeDesc,
    IsPrimaryKey,
    IsUnique,
    IsDisabled,
    IndexKeys,
    ''→'' AS [to],
    TargetIndexTypeDesc,
    TargetIsPrimaryKey,
    TargetIsUnique,
    TargetIsDisabled,
    TargetIndexKeys
FROM
    Comparison
WHERE
    Comparison NOT IN (''Match'',''FromTable is Missing'',''ToTable is Missing'')
ORDER BY
    ObjectName
')

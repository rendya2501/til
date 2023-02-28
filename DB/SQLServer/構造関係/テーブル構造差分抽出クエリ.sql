DECLARE @SourceDataBaseName NVARCHAR(MAX) = 'SourceDataBaseName'
DECLARE @TargetDataBaseName NVARCHAR(MAX) = 'TargetDataBaseName'


-- 削除されたテーブル一覧
-- Fromには存在しないが、Toには存在するテーブル一覧を表示する
exec('
SELECT
    [SQ_Target_Table].[TABLE_NAME]
FROM
    (
        SELECT
            [TABLE_NAME]
        FROM
            [' + @TargetDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Target_Table]
        GROUP BY [TABLE_NAME]
    ) AS [SQ_Target_Table]
    LEFT OUTER JOIN
    (
        SELECT
            [TABLE_NAME]
        FROM
            [' + @SourceDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Source_Table]
        GROUP BY [TABLE_NAME]
    ) AS [SQ_Source_Table]
    ON [SQ_Target_Table].[TABLE_NAME] = [SQ_Source_Table].[TABLE_NAME]
WHERE
    [SQ_Source_Table].[TABLE_NAME] IS NULL
ORDER BY [SQ_Target_Table].[TABLE_NAME]
');


-- 追加されたテーブル一覧
-- Fromには存在するが、Toには存在しないテーブル一覧を表示する
exec('
SELECT
    [SQ_Source_Table].[TABLE_NAME]
FROM
    (
        SELECT
            [TABLE_NAME]
        FROM
            [' + @SourceDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Source_Table]
        GROUP BY [TABLE_NAME]
    ) AS [SQ_Source_Table]
    LEFT OUTER JOIN
    (
        SELECT
            [TABLE_NAME]
        FROM
            [' + @TargetDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Target_Table]
        GROUP BY [TABLE_NAME]
    ) AS [SQ_Target_Table]
    ON [SQ_Source_Table].[TABLE_NAME] = [SQ_Target_Table].[TABLE_NAME]
WHERE
    [SQ_Target_Table].[TABLE_NAME] IS NULL
ORDER BY [SQ_Source_Table].[TABLE_NAME]
');


-- 変更のあるカラム一覧
-- FromとToの両方で差異があるカラム一覧を表示する
EXEC('
SELECT
    CASE WHEN [SQ_Source_Table].[TABLE_NAME] IS NOT NULL THEN [SQ_Source_Table].[TABLE_NAME] ELSE [SQ_Target_Table].[TABLE_NAME] END AS [TABLE_NAME],
    CASE WHEN [SQ_Source_Table].[COLUMN_NAME] IS NOT NULL THEN [SQ_Source_Table].[COLUMN_NAME] ELSE [SQ_Target_Table].[COLUMN_NAME] END AS [COLUMN_NAME],
    [SQ_Source_Table].[DATA_TYPE],
    [SQ_Source_Table].[CHARACTER_MAXIMUM_LENGTH] AS [LEN],
    [SQ_Source_Table].[IS_NULLABLE] AS [ISNULL],
    [SQ_Source_Table].[Key] AS [Key],
    ''→'' AS [To],
    [SQ_Target_Table].[DATA_TYPE],
    [SQ_Target_Table].[CHARACTER_MAXIMUM_LENGTH] AS [LEN],
    [SQ_Target_Table].[IS_NULLABLE] AS [ISNULL],
    [SQ_Target_Table].[Key] AS [Key]
FROM
    (
        SELECT
            [Source_Table].[TABLE_NAME],
            [Source_Table].[COLUMN_NAME],
            [Source_Table].[DATA_TYPE],
            [Source_Table].[CHARACTER_MAXIMUM_LENGTH],
            [Source_Table].[IS_NULLABLE],
            (CASE WHEN [Source_Key].[COLUMN_NAME] IS NOT NULL THEN ''Key'' ELSE NULL END) AS [Key]
        FROM
            [' + @SourceDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Source_Table]
            LEFT OUTER JOIN [INFORMATION_SCHEMA].[KEY_COLUMN_USAGE] AS [Source_Key]
            ON [Source_Table].[TABLE_NAME] = [Source_Key].[TABLE_NAME]
            AND [Source_Table].[COLUMN_NAME] = [Source_Key].[COLUMN_NAME]
    ) AS [SQ_Source_Table]
    FULL JOIN
    (
        SELECT
            [Target_Table].[TABLE_NAME],
            [Target_Table].[COLUMN_NAME],
            [Target_Table].[DATA_TYPE],
            [Target_Table].[CHARACTER_MAXIMUM_LENGTH],
            [Target_Table].[IS_NULLABLE],
            (CASE WHEN [Target_Key].[COLUMN_NAME] IS NOT NULL THEN ''Key'' ELSE NULL END) AS [Key]
        FROM
            [' + @TargetDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Target_Table]
            LEFT OUTER JOIN [INFORMATION_SCHEMA].[KEY_COLUMN_USAGE] AS [Target_Key]
            ON [Target_Table].[TABLE_NAME] = [Target_Key].[TABLE_NAME]
            AND [Target_Table].[COLUMN_NAME] = [Target_Key].[COLUMN_NAME]
    ) AS [SQ_Target_Table]
    ON [SQ_Source_Table].[TABLE_NAME] = [SQ_Target_Table].[TABLE_NAME]
    AND [SQ_Source_Table].[COLUMN_NAME] = [SQ_Target_Table].[COLUMN_NAME]
WHERE
    (
        ISNULL([SQ_Source_Table].[COLUMN_NAME], '''') <> ISNULL([SQ_Target_Table].[COLUMN_NAME], '''')
        OR ISNULL([SQ_Source_Table].[DATA_TYPE], '''') <> ISNULL([SQ_Target_Table].[DATA_TYPE], '''')
        OR ISNULL([SQ_Source_Table].[CHARACTER_MAXIMUM_LENGTH], -1) <> ISNULL([SQ_Target_Table].[CHARACTER_MAXIMUM_LENGTH], -1)
        OR ISNULL([SQ_Source_Table].[IS_NULLABLE], '''') <> ISNULL([SQ_Target_Table].[IS_NULLABLE], '''')
        OR ISNULL([SQ_Source_Table].[Key], '''') <> ISNULL([SQ_Target_Table].[Key], '''')
    )
    AND NOT EXISTS (
        SELECT
            *
        FROM
            (
                SELECT
                    [SQ_Target_Table].[TABLE_NAME]
                FROM
                    (
                        SELECT
                            [TABLE_NAME]
                        FROM
                            [' + @TargetDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Target_Table]
                        GROUP BY [TABLE_NAME]
                    ) AS [SQ_Target_Table]
                    LEFT OUTER JOIN
                    (
                        SELECT
                            [TABLE_NAME]
                        FROM
                            [' + @SourceDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Source_Table]
                        GROUP BY [TABLE_NAME]
                    ) AS [SQ_Source_Table]
                    ON [SQ_Target_Table].[TABLE_NAME] = [SQ_Source_Table].[TABLE_NAME]
                WHERE
                    [SQ_Source_Table].[TABLE_NAME] IS NULL
            ) AS [SQ_DeleteTable]
        WHERE
            [SQ_Target_Table].[TABLE_NAME] = [SQ_DeleteTable].[TABLE_NAME]
    )
    AND NOT EXISTS (
        SELECT
            *
        FROM
            (
                SELECT
                    [SQ_Source_Table].[TABLE_NAME]
                FROM
                    (
                        SELECT
                            [TABLE_NAME]
                        FROM
                            [' + @SourceDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Source_Table]
                        GROUP BY [TABLE_NAME]
                    ) AS [SQ_Source_Table]
                    LEFT OUTER JOIN
                    (
                        SELECT
                            [TABLE_NAME]
                        FROM
                            [' + @TargetDataBaseName + '].[INFORMATION_SCHEMA].[COLUMNS] AS [Target_Table]
                        GROUP BY [TABLE_NAME]
                    ) AS [SQ_Target_Table]
                    ON [SQ_Source_Table].[TABLE_NAME] = [SQ_Target_Table].[TABLE_NAME]
                WHERE
                    [SQ_Target_Table].[TABLE_NAME] IS NULL
            ) AS [SQ_AddTable]
        WHERE
            [SQ_Source_Table].[TABLE_NAME] = [SQ_AddTable].[TABLE_NAME]
    )
ORDER BY
    CASE WHEN [SQ_Source_Table].[TABLE_NAME] IS NOT NULL THEN [SQ_Source_Table].[TABLE_NAME] ELSE [SQ_Target_Table].[TABLE_NAME] END,
    CASE WHEN [SQ_Source_Table].[COLUMN_NAME] IS NOT NULL THEN [SQ_Source_Table].[COLUMN_NAME] ELSE [SQ_Target_Table].[COLUMN_NAME] END
');


-- インデックスの差異一覧
-- FromとToの両方で差異があるインデックス一覧を表示する
EXEC('
SELECT
    [SQ_Source_Table].[ObjectName],
    [SQ_Source_Table].[IndexName],
    [SQ_Source_Table].[IndexTypeDesc],
    [SQ_Source_Table].[IsPrimaryKey],
    [SQ_Source_Table].[IsUnique],
    [SQ_Source_Table].[IsDisabled],
    [SQ_Source_Table].[IndexKeys],
    ''→'' as [To],
    [SQ_Target_Table].[ObjectName],
    [SQ_Target_Table].[IndexName],
    [SQ_Target_Table].[IndexTypeDesc],
    [SQ_Target_Table].[IsPrimaryKey],
    [SQ_Target_Table].[IsUnique],
    [SQ_Target_Table].[IsDisabled],
    [SQ_Target_Table].[IndexKeys]
FROM 
    (
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
                    JOIN [' + @SourceDataBaseName + '].[sys].[all_columns] AS [AC]
                    ON  [IC].[object_id] = [AC].[object_id]
                    AND [IC].[column_id] = [AC].[column_id]
                WHERE
                    [IC].[object_id] = [I].[object_id]
                    AND [IC].[index_id] = [I].[index_id]
                    AND [is_included_column] = 0 
                    FOR XML PATH(''''), TYPE
            ).value(''.'', ''VARCHAR(MAX)''), 1, 2, '''')
        FROM
            [' + @SourceDataBaseName + '].[sys].[indexes] AS [I]
            INNER JOIN [' + @SourceDataBaseName + '].[sys].[objects] AS [O]
            ON [I].[object_id] = [O].[object_id]
        WHERE
            [I].[index_id] > 0
            AND [O].[is_ms_shipped] = 0
    ) AS [SQ_Source_Table]
    FULL JOIN
    (
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
                    JOIN [' + @TargetDataBaseName + '].[sys].[all_columns] AS [AC]
                    ON  [IC].[object_id] = [AC].[object_id]
                    AND [IC].[column_id] = [AC].[column_id]
                WHERE
                    [IC].[object_id] = [I].[object_id]
                    AND [IC].[index_id] = [I].[index_id]
                    AND [is_included_column] = 0 
                    FOR XML PATH(''''), TYPE
            ).value(''.'', ''VARCHAR(MAX)''), 1, 2, '''')
        FROM
            [' + @TargetDataBaseName + '].[sys].[indexes] AS [I]
            INNER JOIN [' + @TargetDataBaseName + '].[sys].[objects] AS [O]
            ON [I].[object_id] = [O].[object_id]
        WHERE
            [I].[index_id] > 0
            AND [O].[is_ms_shipped] = 0
    ) AS [SQ_Target_Table]
    ON [SQ_Source_Table].[ObjectName] = [SQ_Target_Table].[ObjectName]
    AND [SQ_Source_Table].[IndexName] = [SQ_Target_Table].[IndexName]
    AND [SQ_Source_Table].[IndexKeys] = [SQ_Target_Table].[IndexKeys]
WHERE 
    [SQ_Source_Table].[ObjectName] IS NULL
    OR [SQ_Target_Table].[ObjectName] IS NULL
')
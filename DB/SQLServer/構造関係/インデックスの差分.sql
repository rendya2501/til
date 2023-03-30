DECLARE @SourceDataBaseName NVARCHAR(MAX) = 'SourceDataBaseName'
DECLARE @TargetDataBaseName NVARCHAR(MAX) = 'TargetDataBaseName'

EXEC('
WITH SourceTables AS (
    SELECT [O].[name] AS [ObjectName]
    FROM [' + @SourceDataBaseName + '].[sys].[objects] AS [O]
    WHERE [O].[is_ms_shipped] = 0
),
TargetTables AS (
    SELECT [O].[name] AS [ObjectName]
    FROM [' + @TargetDataBaseName + '].[sys].[objects] AS [O]
    WHERE [O].[is_ms_shipped] = 0
),
TableDifferences AS (
    SELECT
        CASE 
            WHEN SourceTables.[ObjectName] IS NULL THEN ''FromNothing''
            WHEN TargetTables.[ObjectName] IS NULL THEN ''ToNothing''
            ELSE NULL
        END AS [DifferenceReason],
        COALESCE(SourceTables.[ObjectName], TargetTables.[ObjectName]) AS [ObjectName],
        NULL AS [IndexName],
        NULL AS [Source_IndexTypeDesc],
        NULL AS [Source_IsPrimaryKey],
        NULL AS [Source_IsUnique],
        NULL AS [Source_IsDisabled],
        NULL AS [Source_IndexKeys],
        NULL AS [To],
        NULL AS [Target_IndexTypeDesc],
        NULL AS [Target_IsPrimaryKey],
        NULL AS [Target_IsUnique],
        NULL AS [Target_IsDisabled],
        NULL AS [Target_IndexKeys]
    FROM SourceTables
    FULL JOIN TargetTables ON SourceTables.[ObjectName] = TargetTables.[ObjectName]
    WHERE SourceTables.[ObjectName] IS NULL OR TargetTables.[ObjectName] IS NULL
),
IndexDifferences AS (
	SELECT
		CASE 
			WHEN [SQ_Source_Table].[ObjectName] IS NULL THEN ''FromIndexNothing''
			WHEN [SQ_Target_Table].[ObjectName] IS NULL THEN ''ToIndexNothing''
			ELSE ''IndexDiff''
		END AS [DifferenceReason],
		COALESCE([SQ_Source_Table].[ObjectName], [SQ_Target_Table].[ObjectName]) AS [ObjectName],
		COALESCE([SQ_Source_Table].[IndexName], [SQ_Target_Table].[IndexName]) AS [IndexName],
		[SQ_Source_Table].[IndexTypeDesc] AS [Source_IndexTypeDesc],
		[SQ_Source_Table].[IsPrimaryKey] AS [Source_IsPrimaryKey],
		[SQ_Source_Table].[IsUnique] AS [Source_IsUnique],
		[SQ_Source_Table].[IsDisabled] AS [Source_IsDisabled],
		[SQ_Source_Table].[IndexKeys] AS [Source_IndexKeys],
		''→'' as [To],
		[SQ_Target_Table].[IndexTypeDesc] AS [Target_IndexTypeDesc],
		[SQ_Target_Table].[IsPrimaryKey] AS [Target_IsPrimaryKey],
		[SQ_Target_Table].[IsUnique] AS [Target_IsUnique],
		[SQ_Target_Table].[IsDisabled] AS [Target_IsDisabled],
		[SQ_Target_Table].[IndexKeys] AS [Target_IndexKeys]
	FROM 
		(
			-- ソースデータベースのインデックス情報を取得するサブクエリ
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
			-- ターゲットデータベースのインデックス情報を取得するサブクエリ
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
	WHERE 
		([SQ_Source_Table].[IndexKeys] <> [SQ_Target_Table].[IndexKeys]
		OR [SQ_Source_Table].[IndexName] <> [SQ_Target_Table].[IndexName]
		OR [SQ_Source_Table].[IndexTypeDesc] <> [SQ_Target_Table].[IndexTypeDesc]
		OR [SQ_Source_Table].[IsPrimaryKey] <> [SQ_Target_Table].[IsPrimaryKey]
		OR [SQ_Source_Table].[IsUnique] <> [SQ_Target_Table].[IsUnique]
		OR [SQ_Source_Table].[IsDisabled] <> [SQ_Target_Table].[IsDisabled])
		OR [SQ_Source_Table].[ObjectName] IS NULL
		OR [SQ_Target_Table].[ObjectName] IS NULL
		OR [SQ_Source_Table].[IndexName] IS NULL
		OR [SQ_Target_Table].[IndexName] IS NULL
)
SELECT *
FROM TableDifferences
UNION ALL
SELECT *
FROM IndexDifferences
ORDER BY [ObjectName], [IndexName]
')

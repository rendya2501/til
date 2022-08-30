DECLARE @SourceDataBaseName NVARCHAR(MAX) = 'SourceDataBaseName'
DECLARE @TargetDataBaseName NVARCHAR(MAX) = 'TargetDataBaseName'

-- 削除されたテーブル一覧
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
exec('
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
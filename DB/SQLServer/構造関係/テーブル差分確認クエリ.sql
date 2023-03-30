/****
2つのテーブルを比較して差分を確認するためのクエリです。
****/

DECLARE @SourceTable NVARCHAR(MAX) = 'SourceTable'
DECLARE @TargetTable NVARCHAR(MAX) = 'TargetTable'

SELECT
    [SQ_Source_Table].[TABLE_NAME],
    [SQ_Source_Table].[COLUMN_NAME],
    [SQ_Source_Table].[DATA_TYPE],
    [SQ_Source_Table].[CHARACTER_MAXIMUM_LENGTH] AS [LEN],
    [SQ_Source_Table].[IS_NULLABLE] AS [ISNULL],
    '→' AS [To],
    [SQ_Target_Table].[TABLE_NAME],
    [SQ_Target_Table].[COLUMN_NAME],
    [SQ_Target_Table].[DATA_TYPE],
    [SQ_Target_Table].[CHARACTER_MAXIMUM_LENGTH] AS [LEN],
    [SQ_Target_Table].[IS_NULLABLE] AS [ISNULL],
	CASE
        WHEN [SQ_Source_Table].[COLUMN_NAME] = [SQ_Target_Table].[COLUMN_NAME] 
			AND [SQ_Source_Table].[DATA_TYPE] = [SQ_Target_Table].[DATA_TYPE] 
            AND (
                ([SQ_Source_Table].[CHARACTER_MAXIMUM_LENGTH] = [SQ_Target_Table].[CHARACTER_MAXIMUM_LENGTH]) 
                OR ([SQ_Source_Table].[CHARACTER_MAXIMUM_LENGTH] IS NULL AND [SQ_Target_Table].[CHARACTER_MAXIMUM_LENGTH] IS NULL)
            )
            AND [SQ_Source_Table].[IS_NULLABLE] = [SQ_Target_Table].[IS_NULLABLE] THEN '○'
        ELSE '×'
    END AS [Match]
FROM (
    SELECT * 
    FROM [INFORMATION_SCHEMA].[COLUMNS]
    WHERE TABLE_NAME = @SourceTable 
) AS [SQ_Source_Table]
FULL JOIN
(
    SELECT * 
    FROM [INFORMATION_SCHEMA].[COLUMNS]
    WHERE TABLE_NAME = @TableseName
) AS [SQ_Target_Table]
ON [SQ_Source_Table].[COLUMN_NAME] = [SQ_Target_Table].[COLUMN_NAME]
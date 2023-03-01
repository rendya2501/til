

SELECT
    O.name AS ObjectName,
    I.name AS IndexName,
    I.type_desc,
    I.is_primary_key AS IsPrimaryKey,
    I.is_unique AS IsUnique,
    I.is_disabled AS IsDisabled,
    STUFF((
            SELECT
                CONCAT(
                    ', ',
                    sys.all_columns.name,
                    CASE
                        WHEN is_descending_key = 1 THEN ' - DESC'
                        ELSE ''
                    END
                )
            FROM
                sys.index_columns
                JOIN sys.all_columns
                ON  sys.index_columns.object_id = sys.all_columns.object_id
                AND sys.index_columns.column_id = sys.all_columns.column_id
            WHERE
                sys.index_columns.object_id = I.object_id
                AND sys.index_columns.index_id = I.index_id
                AND is_included_column = 0 FOR XML PATH(''),
                TYPE
        ).value('.', 'VARCHAR(MAX)'), 1, 2, '') AS IndexKeys,
    STUFF((
            SELECT
                CONCAT(
                    ', ',
                    sys.all_columns.name,
                    CASE
                        WHEN is_descending_key = 1 THEN ' - DESC'
                        ELSE ''
                    END
                )
            FROM
                sys.index_columns
                JOIN sys.all_columns
                ON  sys.index_columns.object_id = sys.all_columns.object_id
                AND sys.index_columns.column_id = sys.all_columns.column_id
            WHERE
                sys.index_columns.object_id = I.object_id
                AND sys.index_columns.index_id = I.index_id
                AND is_included_column = 1 FOR XML PATH(''),
                TYPE
        ).value('.', 'VARCHAR(MAX)'), 1, 2, '') AS IncludedColumns
FROM
    sys.indexes AS I
    INNER JOIN sys.objects AS O
    ON I.object_id = O.object_id
WHERE
    I.index_id > 0
    AND O.is_ms_shipped = 0
    


SELECT
    O.name AS ObjectName,
    I.name AS IndexName,
    I.type_desc,
    I.is_primary_key AS IsPrimaryKey,
    I.is_unique AS IsUnique,
    I.is_disabled AS IsDisabled,
    STUFF((
            SELECT
                CONCAT(
                    ', ',
                    (SELECT name FROM sys.all_columns WHERE object_id = I.object_id AND column_id = index_columns.column_id),
                    CASE
                        WHEN is_descending_key = 1 THEN ' - DESC'
                        ELSE ''
                    END
                )
            FROM
                sys.index_columns
            WHERE
                sys.index_columns.object_id = I.object_id
                AND sys.index_columns.index_id = I.index_id
                AND is_included_column = 0 
            FOR XML PATH(''),
            TYPE
        ).value('.', 'VARCHAR(MAX)'), 1, 2, '') AS IndexKeys
FROM
    sys.indexes AS I
    INNER JOIN sys.objects AS O
    ON I.object_id = O.object_id
WHERE
    I.index_id > 0
    AND O.is_ms_shipped = 0
    




SELECT 
    O.name AS ObjectName,
    I.name AS IndexName,
    I.type_desc AS IndexTypeDesc,
    I.is_primary_key AS IsPrimaryKey,
    I.is_unique AS IsUnique,
    I.is_disabled AS IsDisabled,
    STUFF((
        SELECT
            CONCAT(
                ', ',
                sys.all_columns.name,
                CASE
                    WHEN is_descending_key = 1 THEN ' - DESC'
                    ELSE ''
                END
            )
        FROM
            sys.index_columns
            JOIN sys.all_columns
            ON  sys.index_columns.object_id = sys.all_columns.object_id
            AND sys.index_columns.column_id = sys.all_columns.column_id
        WHERE
            sys.index_columns.object_id = I.object_id
            AND sys.index_columns.index_id = I.index_id
            AND is_included_column = 0 FOR XML PATH(''),
            TYPE
    ).value('.', 'VARCHAR(MAX)'), 1, 2, '') AS IndexKeys
FROM
    sys.indexes AS I
        INNER JOIN sys.objects AS O
            ON I.object_id = O.object_id
WHERE
    I.index_id > 0
    AND O.is_ms_shipped = 0
ORDER BY ObjectName

GO


WITH Syyyyyyys AS (
select
    O.name AS ObjectName,
    I.name AS IndexName,
    I.type_desc,
    I.is_primary_key AS IsPrimaryKey,
    I.is_unique AS IsUnique,
    I.is_disabled AS IsDisabled,
    sys.all_columns.name
FROM
    sys.index_columns
    JOIN sys.all_columns
    ON  sys.index_columns.object_id = sys.all_columns.object_id
    AND sys.index_columns.column_id = sys.all_columns.column_id
    JOIN sys.indexes AS I
    ON  sys.index_columns.object_id = I.object_id
    AND sys.index_columns.index_id = I.index_id
    JOIN sys.objects AS O
    ON I.object_id = O.object_id
WHERE
    I.index_id > 0
    AND O.is_ms_shipped = 0
)
SELECT DISTINCT
    T1.ObjectName,
    T1.IndexName,
    T1.type_desc,
    T1.IsPrimaryKey,
    T1.IsUnique,
    T1.IsDisabled,
    STUFF(
        (
        SELECT
            CONCAT(
                ', ',
                T2.name
            ) 
        FROM
            Syyyyyyys T2
        WHERE
            T1.ObjectName = T2.ObjectName
            AND T1.IndexName = T2.IndexName
        FOR XML PATH(''),
            TYPE
        ).value('.', 'VARCHAR(MAX)'), 1, 2, ''
    ) AS IndexKeys
FROM Syyyyyyys T1












select * from sys.index_columns
select * from sys.all_columns

select * from sys.objects
select * from sys.schemas



SELECT
    CONCAT(
        ', ',
        sys.all_columns.name,
        CASE
            WHEN is_descending_key = 1 THEN ' - DESC'
            ELSE ''
        END
    )
FROM
    sys.index_columns
    JOIN sys.all_columns
    ON  sys.index_columns.object_id = sys.all_columns.object_id
    AND sys.index_columns.column_id = sys.all_columns.column_id
WHERE
    sys.index_columns.object_id = I.object_id
    AND sys.index_columns.index_id = I.index_id
    AND is_included_column = 0



SELECT
	ISNULL([SQ_Source_Table].[ObjectName],ISNULL(CONVERT(nvarchar,OBJECT_ID ([SQ_Target_Table].[ObjectName], N'U')),[SQ_Target_Table].[ObjectName] + ' is Missing')),
    [SQ_Source_Table].[ObjectName],
    [SQ_Source_Table].[IndexName],
    [SQ_Source_Table].[IndexTypeDesc],
    [SQ_Source_Table].[IsPrimaryKey],
    [SQ_Source_Table].[IsUnique],
    [SQ_Source_Table].[IsDisabled],
    [SQ_Source_Table].[IndexKeys],
    '→' as [To],
	ISNULL([SQ_Target_Table].[ObjectName],ISNULL(CONVERT(nvarchar,OBJECT_ID ([SQ_Source_Table].[ObjectName], N'U')),[SQ_Source_Table].[ObjectName] + ' is Missing')),
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
                        ', ',
                        [AC].[name],
                        CASE
                            WHEN [is_descending_key] = 1 THEN ' - DESC'
                            ELSE ''
                        END
                    )
                FROM
                    [src].[sys].[index_columns] AS [IC]
                    JOIN [src].[sys].[all_columns] AS [AC]
                    ON  [IC].[object_id] = [AC].[object_id]
                    AND [IC].[column_id] = [AC].[column_id]
                WHERE
                    [IC].[object_id] = [I].[object_id]
                    AND [IC].[index_id] = [I].[index_id]
                    AND [is_included_column] = 0 
                    FOR XML PATH(''), TYPE
            ).value('.', 'VARCHAR(MAX)'), 1, 2, '')
        FROM
            [src].[sys].[indexes] AS [I]
            INNER JOIN [src].[sys].[objects] AS [O]
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
                        ', ',
                        [AC].[name],
                        CASE
                            WHEN [is_descending_key] = 1 THEN ' - DESC'
                            ELSE ''
                        END
                    )
                FROM
                    [dst].[sys].[index_columns] AS [IC]
                    JOIN [dst].[sys].[all_columns] AS [AC]
                    ON  [IC].[object_id] = [AC].[object_id]
                    AND [IC].[column_id] = [AC].[column_id]
                WHERE
                    [IC].[object_id] = [I].[object_id]
                    AND [IC].[index_id] = [I].[index_id]
                    AND [is_included_column] = 0 
                    FOR XML PATH(''), TYPE
            ).value('.', 'VARCHAR(MAX)'), 1, 2, '')
        FROM
            [dst].[sys].[indexes] AS [I]
            INNER JOIN [dst].[sys].[objects] AS [O]
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



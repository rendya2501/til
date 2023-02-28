

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




select DISTINCT
    O.name AS ObjectName,
    I.name AS IndexName,
    I.type_desc,
    I.is_primary_key AS IsPrimaryKey,
    I.is_unique AS IsUnique,
    I.is_disabled AS IsDisabled,
    STUFF(
        (
        SELECT
            CONCAT(
                ', ',
                sys.all_columns.name,
                CASE
                    WHEN is_descending_key = 1 THEN ' - DESC'
                    ELSE ''
                END
            ) FOR XML PATH(''),
            TYPE
        ).value('.', 'VARCHAR(MAX)'), 1, 2, ''
    ) AS IndexKeys
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





WITH Test AS (
    SELECT '001' AS Code, 'AAA' AS Name
    UNION ALL
    SELECT '001', 'BBB'
)
SELECT DISTINCT
Code,
STUFF(
        (
            SELECT
                CONCAT(
                    ', ',
                    name
                ) 
            FROM
                Test T2
            WHERE 
                T2.Code = Test.Code
            FOR XML PATH(''),TYPE
        ).value('.', 'VARCHAR(MAX)'), 1, 2, ''
    ) AS IndexKeys
FROM TEST




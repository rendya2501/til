/*
サーバー上のすべてのデータベースに対して特定のテーブルが存在するかどうかを調べるクエリ
*/
DECLARE @DatabaseName NVARCHAR(128);
DECLARE @TableName NVARCHAR(128) = 'SearchTableName'; -- 検索したいテーブル名を指定してください
DECLARE @SQL NVARCHAR(MAX);
DECLARE @TableExists BIT;
DECLARE @Result NVARCHAR(MAX) = '';

-- データベース名を取得
DECLARE db_cursor CURSOR FOR
SELECT name
FROM sys.databases
WHERE state = 0 -- Only select online databases
ORDER BY name

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @DatabaseName

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @SQL = N'SET @TableExists = (
                     SELECT CASE WHEN EXISTS (
                         SELECT 1
                         FROM ' + QUOTENAME(@DatabaseName) + '.INFORMATION_SCHEMA.TABLES
                         WHERE TABLE_NAME = ''' + @TableName + '''
                     ) THEN 1 ELSE 0 END
                 );'

    EXEC sp_executesql @SQL, N'@TableExists BIT OUTPUT', @TableExists OUTPUT

    IF @TableExists = 1
    BEGIN
        SET @Result += @DatabaseName + CHAR(13) + CHAR(10) 
    END

    FETCH NEXT FROM db_cursor INTO @DatabaseName
END

CLOSE db_cursor
DEALLOCATE db_cursor

-- 結果を出力
PRINT 'Table: [' + @TableName + '] EXISTS IN...'
PRINT LEFT(@Result, LEN(@Result) - 1)

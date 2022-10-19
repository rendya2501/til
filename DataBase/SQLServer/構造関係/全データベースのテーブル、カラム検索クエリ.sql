/*
このサイトを参考に少し改造した。
テーブルが存在するデータベースを検索したい時があったので、検索できるようにした。

[SQLServer 全てのデータベースのテーブルやカラムの情報を取得するSQLL](https://qiita.com/ken1980/items/ddff5c3c8cb37736eda7)  
*/

-- 動的クエリ
DECLARE @sql nvarchar(MAX) = '';
-- データベース名を格納する変数
DECLARE @databaseName nvarchar(MAX);
-- 検索したいテーブル名を設定する
DECLARE @target_table_name nvarchar(MAX) = '';

--全てのデータベースのテーブルの情報を取得する。
DECLARE databaseNameList CURSOR FOR 
    SELECT [name] FROM sys.databases --ここでWHERE条件を指定して対象データベースを絞り込める。
--カーソルオープン
OPEN databaseNameList;
--databaseNameList分繰り返す。
FETCH NEXT FROM databaseNameList INTO @databaseName;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF LEN(@sql) > 0
    BEGIN
        SET @sql += ' UNION '
    END

    --「UNION 操作の "SQL_Latin1_General_CP1_CI_AS" と "Japanese_CI_AS" 間での照合順序の競合を解決できません。」等と
    --エラーになったら、「照合順序の競合時に要COLLATE」とある列に「COLLATE Japanese_CI_AS」等として照合順序を指定する。
    --この例では「COLLATE Japanese_CI_AS 」としているが、使用状況に応じて要調整。
    SET @sql += 
        'SELECT
            ''' + @databaseName + ''' as "データベース名",
            sysobjects.name COLLATE Japanese_CI_AS テーブル名, --照合順序の競合時に要COLLATE
            (
                STUFF(
                    (SELECT '', '' + syscolumns.name
                    FROM ['+ @databaseName +'].dbo.syscolumns syscolumns
                    WHERE sysobjects.id = syscolumns.id
                    FOR XML PATH(''''),TYPE).value(''.'', ''NVARCHAR(MAX)''),
                    1,2,''''
                )
            ) AS "カラム名一覧"
        FROM
            ['+ @databaseName +'].dbo.sysobjects sysobjects
        WHERE
            sysobjects.type = ''U''
            AND sysobjects.category = 0
            -- AND [Name] IN (CASE WHEN '''+ @target_table_name + ''' <> '''' THEN '''+ @target_table_name + ''' ELSE [Name] END)
        '
    -- 検索したいテーブル名があれば条件を生成
    IF LEN(@target_table_name) > 0
    BEGIN
        SET @sql += 'AND [Name] = '''+ @target_table_name + ''''
    END

    FETCH NEXT FROM databaseNameList INTO @databaseName;
END;

--カーソルクローズ&リソース解放
CLOSE databaseNameList;
DEALLOCATE databaseNameList;

--デバッグ用
--print(@sql); 

--組み立てたSQLを実行する。
EXEC(@sql);

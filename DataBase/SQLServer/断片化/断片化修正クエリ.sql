-- 統計情報再構築
EXEC sp_updatestats

-- INDEX断片化解消
DECLARE @DBID SMALLINT = DB_ID()

-- カーソル定義 テーブルに対する断片化率の一覧を取得クエリ
DECLARE CUR_FragmentationList CURSOR FOR 
    SELECT
        CONVERT(NVARCHAR,SYSINDEXES.NAME) AS INDEX_NAME
        ,CONVERT(NVARCHAR,SYSOBJECTS.name) AS [TABLE_NAME]
    --    ,IPS.INDEX_TYPE_DESC AS INDEX_TYPE
    --    ,IPS.ALLOC_UNIT_TYPE_DESC AS UNIT_TYPE
        ,CONVERT(numeric,IPS.AVG_FRAGMENTATION_IN_PERCENT) AS [断片化率]
    --    ,SYSINDEXES.ROWS AS 件数
    --    ,IPS.*
    FROM
        (
            SELECT ID,INDID
            FROM SYSINDEXKEYS
            GROUP BY ID,INDID
        ) AS SYSINDEXKEYS
        INNER JOIN SYSOBJECTS
        ON  SYSINDEXKEYS.ID = SYSOBJECTS.ID
        INNER JOIN SYSINDEXES
        ON  SYSINDEXKEYS.ID = SYSINDEXES.ID
        AND SYSINDEXKEYS.INDID = SYSINDEXES.INDID
        INNER JOIN SYS.DM_DB_INDEX_PHYSICAL_STATS(@DBID, NULL, NULL, NULL, NULL) AS IPS
        ON  IPS.OBJECT_ID = SYSINDEXES.ID
        AND IPS.INDEX_ID = SYSINDEXES.INDID
    WHERE
        SYSOBJECTS.XTYPE = 'U'
        AND SYSINDEXES.NAME <> 'PK_DTPROPERTIES'
        AND IPS.AVG_FRAGMENTATION_IN_PERCENT > 10
    ORDER BY
        SYSINDEXES.NAME

-- 変数定義
DECLARE @INDEX_NAME AS NVARCHAR(100)
DECLARE @TABLE_NAME AS NVARCHAR(100)
DECLARE @PER AS numeric
DECLARE @Sql AS NVARCHAR(4000)

-- カーソルオープン
OPEN CUR_FragmentationList

-- カーソルフェッチ
FETCH NEXT FROM CUR_FragmentationList 
    INTO @INDEX_NAME, @TABLE_NAME, @PER

    -- ループ開始
    WHILE @@FETCH_STATUS = 0
        BEGIN
            PRINT @INDEX_NAME + '/' + @TABLE_NAME + '/' + CONVERT(NVARCHAR,@PER)

            SET @Sql = ''
            -- 断片化率が30%より小さい場合
            IF @PER < 30.0
                BEGIN
                    SET @Sql = N'ALTER INDEX ' + @INDEX_NAME + N' ON ' + @TABLE_NAME + N' REORGANIZE';
                END
            ELSE
                BEGIN
                    SET @Sql = N'ALTER INDEX ' + @INDEX_NAME + N' ON ' + @TABLE_NAME + N' REBUILD';
                END
            
            PRINT @Sql
            
            EXEC(@Sql)
            
            PRINT '実行しました。'
            
            -- 次をフェッチ
            FETCH NEXT FROM CUR_FragmentationList 
                INTO @INDEX_NAME, @TABLE_NAME, @PER
        END

-- カーソル閉じる&解放
CLOSE CUR_FragmentationList
DEALLOCATE CUR_FragmentationList

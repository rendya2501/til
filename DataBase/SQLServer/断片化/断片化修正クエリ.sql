-- 統計情報再構築
EXEC sp_updatestats

-- INDEX断片化解消
DECLARE @DBID SMALLINT = DB_ID()

-- カーソル定義 テーブルに対する断片化率の一覧を取得クエリ
DECLARE CUR_FragmentationList CURSOR FOR 
    SELECT
        CONVERT(NVARCHAR,SYSINDEXES.name) AS [Index_Name]
        ,CONVERT(NVARCHAR,SYSOBJECTS.name) AS [Table_Name]
    --    ,IPS.INDEX_TYPE_DESC AS INDEX_TYPE
    --    ,IPS.ALLOC_UNIT_TYPE_DESC AS UNIT_TYPE
        ,CONVERT(numeric,IPS.AVG_FRAGMENTATION_IN_PERCENT) AS [Fragmentation_Rate]
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
        AND SYSINDEXES.name <> 'PK_DTPROPERTIES'
        AND IPS.AVG_FRAGMENTATION_IN_PERCENT > 10
    ORDER BY
        SYSINDEXES.name

-- 変数定義
DECLARE @Index_Name AS NVARCHAR(100)
DECLARE @Table_Name AS NVARCHAR(100)
DECLARE @Fragmentation_Rate AS numeric
DECLARE @Sql AS NVARCHAR(4000)

-- カーソルオープン
OPEN CUR_FragmentationList

-- カーソルフェッチ
FETCH NEXT FROM CUR_FragmentationList 
INTO @Index_Name, @Table_Name, @Fragmentation_Rate

-- ループ開始
WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT @Index_Name + '/' + @Table_Name + '/' + CONVERT(NVARCHAR,@Fragmentation_Rate)

    SET @Sql = ''
    -- 断片化率が30%より小さい場合
    IF @Fragmentation_Rate < 30.0
        BEGIN
            SET @Sql = N'ALTER INDEX ' + @Index_Name + N' ON ' + @Table_Name + N' REORGANIZE';
        END
    ELSE
        BEGIN
            SET @Sql = N'ALTER INDEX ' + @Index_Name + N' ON ' + @Table_Name + N' REBUILD';
        END
    
    PRINT @Sql
    
    EXEC(@Sql)
    
    PRINT '実行しました。'
    
    -- 次をフェッチ
    FETCH NEXT FROM CUR_FragmentationList 
        INTO @Index_Name, @Table_Name, @Fragmentation_Rate
END

-- カーソル閉じる&解放
CLOSE CUR_FragmentationList
DEALLOCATE CUR_FragmentationList

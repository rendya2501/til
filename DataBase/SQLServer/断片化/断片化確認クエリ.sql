-- 断片化確認クエリ
-- https://techblog.zozo.com/entry/sqlserver-index-reorganize-vs-rebuild

declare @DB_ID int = DB_ID('DB名');
declare @OBJECT_ID int = OBJECT_ID('テーブル名');

select *
from sys.dm_db_index_physical_stats(@DB_ID, @Object_ID, null, null, 'DETAILED') as A
join sys.objects as B on A.object_id = B.object_id
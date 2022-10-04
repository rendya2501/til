-- 断片化確認クエリ
-- https://www.fenet.jp/dotnet/column/database/sql-server/4365/

declare @DB_ID int = DB_ID('DB_TEST');
declare @OBJECT_ID int = OBJECT_ID('USER');

select *
from sys.dm_db_index_physical_stats(@DB_ID, @Object_ID, null, null, 'DETAILED') as A
join sys.objects as B on A.object_id = B.object_id
create table [OperationLog] (
  [SeqNo] bigint identity not null
  , [OperationDateTime] datetime
  , [WindowTitle] nvarchar(50)
  , [Program] nvarchar(200)
  , [OperationType] tinyint
  , [StaffCD] nvarchar(16)
  , [StaffName] nvarchar(60)
  , [Terminal] nvarchar(500)
  , [AccessLogSeqNo] bigint
  , constraint [OperationLog_PKC] primary key ([SeqNo])
) ;

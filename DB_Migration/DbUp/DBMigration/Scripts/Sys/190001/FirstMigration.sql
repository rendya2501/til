
CREATE TABLE [dbo].[AccessLog](
	[SeqNo] [bigint] IDENTITY(1,1) NOT NULL,
	[AccessDateTime] [datetime] NULL,
	[ResponseDateTime] [datetime] NULL,
	[URI] [nvarchar](max) NULL,
	[Parameter] [nvarchar](max) NULL,
	[Response] [nvarchar](max) NULL,
	[StaffCD] [nvarchar](16) NULL,
	[StaffName] [nvarchar](60) NULL,
	[Terminal] [nvarchar](500) NULL,
 CONSTRAINT [AccessLog_PKC] PRIMARY KEY CLUSTERED 
(
	[SeqNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[ErrorLog](
	[SeqNo] [bigint] IDENTITY(1,1) NOT NULL,
	[OccurrenceDateTime] [datetime] NULL,
	[ExceptionMessage] [nvarchar](max) NULL,
	[StackTrace] [nvarchar](max) NULL,
	[StaffCD] [nvarchar](16) NULL,
	[Terminal] [nvarchar](200) NULL,
 CONSTRAINT [ErrorLog_PKC] PRIMARY KEY CLUSTERED 
(
	[SeqNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

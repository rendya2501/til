
CREATE TABLE [dbo].[SettlementDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SettlementID] [nchar](50) NOT NULL,
	[SettlementPlayerNo] [nchar](50) NULL,
	[BusinessDate] [datetime] NOT NULL,
	[AccountNo] [nchar](4) NULL,
	[AccountCls] [tinyint],
	[OrderNumber] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO



alter table kzprize
add freedeal bit

CREATE TABLE [dbo].[kzTransactionFreeDeals](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[inventoryitem_id] [int] NULL,
	[flashdeal_id] [int] NULL,
	[variance] [nvarchar](255) NULL,
	[qty] [int] NULL,
	[discount] [decimal](18, 2) NULL,
	[price] [decimal](18, 2) NULL,
	[transaction_date] [datetime] NULL,
	[process_status] [bit] NULL,
	[process_date] [datetime] NULL,
	[last_action] [varchar](1) NULL,
 CONSTRAINT [PK_kzTransactionFreeDeals] PRIMARY KEY CLUSTERED 
(
	[id] ASC
))

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[kzTransactionFreeDeals]  WITH CHECK ADD  CONSTRAINT [FK_kzTransactionFreeDeals_kzInventoryItems] FOREIGN KEY([inventoryitem_id])
REFERENCES [dbo].[kzInventoryItems] ([id])
GO

ALTER TABLE [dbo].[kzTransactionFreeDeals] CHECK CONSTRAINT [FK_kzTransactionFreeDeals_kzInventoryItems]
GO

ALTER TABLE [dbo].[kzTransactionFreeDeals]  WITH CHECK ADD  CONSTRAINT [FK_kzTransactionFreeDeals_kzUsers] FOREIGN KEY([user_id])
REFERENCES [dbo].[kzUsers] ([id])
GO

ALTER TABLE [dbo].[kzTransactionFreeDeals] CHECK CONSTRAINT [FK_kzTransactionFreeDeals_kzUsers]
GO






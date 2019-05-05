alter table kzTransactions
add tranID nvarchar(255),
orderid nvarchar(255),
status nvarchar(255),
domain nvarchar(255),
amount decimal(18,2),
currency nvarchar(255),
appcode nvarchar(255),
paydate datetime,
skey nvarchar(255),
channel nvarchar(255),
error_code nvarchar(255),
error_desc nvarchar(255)
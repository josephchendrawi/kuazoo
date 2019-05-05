drop table kzEmailSendLog
CREATE TABLE kzEmailSendLog(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
email nvarchar(255),
status int,
subject nvarchar(255),
creationdate datetime
)
CREATE TABLE kzCallbackMOLLog(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
creationdate datetime,
nbcb nvarchar(255),
tranID nvarchar(255),
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
)

alter table kztransactions
add process_status int


CREATE TABLE kzPreCode(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
code nvarchar(255),
transaction_id int
)

alter table kzprize
add fake_visualmeter decimal(18,0)

update kzPrize
set fake_visualmeter=0

CREATE TABLE kzPrize_A(
id  int,
name nvarchar(255),
sponsor_name nvarchar(255),
price decimal(18, 0),
description ntext,
terms ntext,
detail ntext,
expiry_date datetime,
publish_date datetime,
group_name nvarchar(255),
total_revenue decimal(18,0),
cal_total_revenue decimal(18,0),
game_type int,
fake_visualmeter decimal(18,0),
last_created datetime,
last_updated datetime,
last_action varchar(1),
trigger_date datetime
);

CREATE TRIGGER TR_kzPrize ON kzPrize
INSTEAD OF UPDATE
AS
	BEGIN
		INSERT into kzPrize_A(id,name,sponsor_name,price,description,terms,detail,expiry_date,publish_date,group_name,total_revenue, cal_total_revenue,game_type,fake_visualmeter,last_created,last_updated,last_action, trigger_date)
		SELECT id,name,sponsor_name,price,description,terms,detail,expiry_date,publish_date,group_name,total_revenue, cal_total_revenue,game_type,fake_visualmeter,last_created,last_updated,last_action, getutcdate() from inserted

	END
GO

CREATE TABLE kzInventoryItems_A(
id int,
name nvarchar(255),
short_desc nvarchar(255),
price decimal(18,2),
margin decimal(18,2),
general_description nvarchar(max),
description nvarchar(max),
merchant_id int,
keywords nvarchar(255),
inventoryitem_type_id int,
discount decimal(18,2),
expiry_date datetime,
maximumsales decimal(18,0),
remainsales decimal(18,0),
publish_date datetime,
minimumtarget decimal(18,0),
salesvisualmeter decimal(18,0),
prize_id int,
terms nvarchar(max),
created_by int,
last_created datetime,
last_updated datetime,
flag bit,
featured bit,
featured_seq int,
featured_text nvarchar(max),
email_flag bit,
city_id int,
last_action varchar(1),
trigger_date datetime
);



CREATE TRIGGER TR_kzInventoryItems ON kzInventoryItems
INSTEAD OF UPDATE
AS
	BEGIN
		INSERT into kzInventoryItems_A(id,name,short_desc,price,margin,general_description,description,merchant_id,keywords,
inventoryitem_type_id, discount, expiry_date, maximumsales,remainsales,publish_date,minimumtarget,salesvisualmeter,
prize_id, terms,created_by,last_created,last_updated,flag,featured,featured_seq,featured_text, email_flag,city_id,
last_action,trigger_date)
		SELECT id,name,short_desc,price,margin,general_description,description,merchant_id,keywords,
inventoryitem_type_id, discount, expiry_date, maximumsales,remainsales,publish_date,minimumtarget,salesvisualmeter,
prize_id, terms,created_by,last_created,last_updated,flag,featured,featured_seq,featured_text, email_flag,city_id,
last_action, getutcdate() from inserted

	END
GO


CREATE TABLE kzFlashDeal_A(
id  int,
inventoryitem_id int,
discount decimal(18,2),
start_time datetime,
end_time datetime,
flag bit,
last_action varchar(1),
email_flag bit,
trigger_date datetime
);

CREATE TRIGGER TR_kzFlashDeal ON kzFlashDeal
INSTEAD OF UPDATE
AS
	BEGIN
		INSERT into kzFlashDeal_A(id,inventoryitem_id,discount,start_time,end_time,flag,last_action,email_flag,trigger_date)
		SELECT id,inventoryitem_id,discount,start_time,end_time,flag,last_action,email_flag, getutcdate() from inserted

	END
GO

CREATE TABLE kzTags_A(
id  int,
name nvarchar(255),
showAsCategory bit,
parent_id int,
last_created datetime,
last_updated datetime,
last_action varchar(1),
trigger_date datetime
);
CREATE TRIGGER TR_kzTags ON kzTags
INSTEAD OF UPDATE
AS
	BEGIN
		INSERT into kzTags_A(id,name,showAsCategory,parent_id,last_created,last_updated,last_action,trigger_date)
		SELECT id,name,showAsCategory,parent_id,last_created,last_updated,last_action, getutcdate() from inserted

	END
GO


CREATE TABLE kzVariance_A(
id  int,
name nvarchar(255),
price decimal(18,2),
discount decimal(18,2),
inventoryitem_id int,
margin decimal(18,2),
available_limit int,
sku varchar(255),
trigger_date datetime
);
CREATE TRIGGER TR_kzVariance ON kzVariance
INSTEAD OF UPDATE
AS
	BEGIN
		INSERT into kzVariance_A(id,name,price,discount,inventoryitem_id,margin,available_limit,sku,trigger_date)
		SELECT id,name,price,discount,inventoryitem_id,margin,available_limit,sku, getutcdate() from inserted

	END
GO


CREATE TABLE kzKPointAction(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
code int,
description nvarchar(255),
amount decimal(18,0),
last_updated datetime,
last_action varchar(1)
)

CREATE TABLE kzKPointTrxH(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
user_id int,
balance decimal(18,0),
last_updated datetime,
last_action varchar(1)
);
ALTER TABLE kzKPointTrxH  WITH CHECK ADD  CONSTRAINT FK_kzKPointTrxH_kzUsers FOREIGN KEY(user_id)
REFERENCES kzUsers (id);

CREATE TABLE kzKPointTrxD(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
kpointh_id int,
amount decimal(18,0),
action_type int,
from_user int NULL,
to_user int NULL,
remarks ntext,
inventoryitem_id int NULL,
transaction_id int NULL,
last_created datetime,

);
ALTER TABLE kzKPointTrxD  WITH CHECK ADD  CONSTRAINT FK_kzKPointTrxD_kzKPointTrxH FOREIGN KEY(kpointh_id)
REFERENCES kzKPointTrxH (id);
ALTER TABLE kzKPointTrxD  WITH CHECK ADD  CONSTRAINT FK_kzKPointTrxD_kzInventoryItems FOREIGN KEY(inventoryitem_id)
REFERENCES kzInventoryItems (id);
ALTER TABLE kzKPointTrxD  WITH CHECK ADD  CONSTRAINT FK_kzKPointTrxD_kzTransactions FOREIGN KEY(transaction_id)
REFERENCES kzTransactions (id);


insert into kzKPointAction (code,description,amount,last_action)
values(1,'Invite a friend',30,'1');
insert into kzKPointAction (code,description,amount,last_action)
values(2,'Post Deal before purchase',1,'1');
insert into kzKPointAction (code,description,amount,last_action)
values(3,'Post Deal after purchase',3,'1');
insert into kzKPointAction (code,description,amount,last_action)
values(4,'Purchase Item. For each RM spent',1,'1');
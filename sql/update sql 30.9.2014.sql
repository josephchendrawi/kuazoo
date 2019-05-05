update kzInventoryItemTypes
set name='Products' where id=1

update kzInventoryItemTypes
set name='Services' where id=2

delete kzInventoryItemTypes
where id=3


delete kzInventoryItems
where id=4

alter table kztransactiondetails
alter column price decimal(18,2)

alter table kzvariance
add margin decimal(18,0);

alter table kzvariance 
add available_limit int;

alter table kzvariance
add seq int;

alter table kzvariance
drop column seq;

alter table kzvariance 
alter column available_limit int;

alter table kzvariance
add sku varchar(255);

update kzVariance 
set available_limit=10, margin=b.margin
from kzVariance a, kzInventoryItems b
where a.inventoryitem_id = b.id
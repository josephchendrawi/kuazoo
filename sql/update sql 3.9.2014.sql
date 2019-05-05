alter table kzVariance
add price decimal(18,0)

update kzVariance
set kzVariance.price = b.price
from kzVariance a
inner join kzInventoryItems b
on a.inventoryitem_id = b.id
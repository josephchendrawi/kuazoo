alter table kzinventoryitems
add margin decimal(18,0)

update kzInventoryItems
set margin = price

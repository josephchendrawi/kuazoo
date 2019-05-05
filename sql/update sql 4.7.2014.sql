alter table kzInventoryItems
drop column revenue

alter table kzPrize
add  total_revenue decimal (18,0)

update kzPrize
set total_revenue=1000
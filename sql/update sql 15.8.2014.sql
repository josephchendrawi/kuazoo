alter table kzinventoryitems
add city_id int

ALTER TABLE kzInventoryItems  WITH CHECK ADD  CONSTRAINT FK_kzInventoryItems_kzCities FOREIGN KEY(city_id)
REFERENCES kzCities (id);


update kzInventoryItems
set city_id=1
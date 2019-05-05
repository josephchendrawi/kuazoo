CREATE TABLE kzInventoryItemRPs(
	inventory_id int,
	popular bit,
	reviewed bit,
	CONSTRAINT PK_kzInventoryItemRPs PRIMARY KEY (inventory_id)
);


ALTER TABLE kzInventoryItemRPs  WITH CHECK ADD  CONSTRAINT [FK_kzInventoryItemRPs_kzInventoryItems] FOREIGN KEY([inventory_id])
REFERENCES kzInventoryItems ([id])
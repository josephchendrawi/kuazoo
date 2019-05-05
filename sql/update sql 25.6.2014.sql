CREATE TABLE kzVariance(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name nvarchar(255),
inventoryitem_id int
);
ALTER TABLE kzVariance  WITH CHECK ADD  CONSTRAINT FK_kzVariance_kzInventoryItems FOREIGN KEY(inventoryitem_id)
REFERENCES kzInventoryItems (id);


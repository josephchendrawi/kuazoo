alter table kzinventoryitems
alter column margin decimal(18,2)

alter table kzvariance
alter column margin decimal(18,2)

alter table kzinventoryitems
alter column price decimal(18,2)
alter table kzinventoryitems
alter column discount decimal(18,2)

alter table kzvariance
alter column price decimal(18,2)
alter table kzvariance
alter column discount decimal(18,2)


alter table kzTransactionDetails
alter column price decimal(18,2)
alter table kzTransactionDetails
alter column discount decimal(18,2)

alter table kzFlashDeal
alter column discount decimal(18,2)


alter table kzPrize
alter column price decimal(18,2)
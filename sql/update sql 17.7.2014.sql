alter table kzusers
add token nvarchar(255)
alter table kzusers
add token_salt nvarchar(255)
alter table kzusers
add token_expire datetime

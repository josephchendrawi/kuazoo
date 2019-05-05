CREATE TABLE kzBanner(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name nvarchar(255),
link nvarchar(255)
);

CREATE TABLE kzBannerImages(
banner_id int,
image_id int,
image_url nvarchar(255),
CONSTRAINT PK_kzBannerImages PRIMARY KEY (banner_id,image_id)
);

alter table kzBanner
add last_action varchar(1)

alter table kzBanner
add seq int
CREATE TABLE kzBVD(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
title nvarchar(255),
link nvarchar(255),
seq int,
typee int,
last_action varchar(1)
);

CREATE TABLE kzBVDImages(
bvd_id int,
image_id int,
image_url nvarchar(255),
CONSTRAINT PK_kzBVDImages PRIMARY KEY (bvd_id,image_id)
);
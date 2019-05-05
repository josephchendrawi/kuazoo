CREATE TABLE kzLogger(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
last_created datetime,
type varchar(10),
url nvarchar(255),
ip_address varchar(255),
logger ntext,
api_key varchar(255),
req_body ntext,
res_body ntext,
err_message ntext
);
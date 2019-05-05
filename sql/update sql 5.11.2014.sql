

CREATE TABLE kzWebExceptionLogger(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
last_created datetime,
url nvarchar(255),
ip_address varchar(255),
logger ntext,
err_exception ntext,
err_message ntext
);

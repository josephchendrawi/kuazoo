CREATE TABLE kzAdmins(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
first_name nvarchar(255),
last_name nvarchar(255),
email varchar(255),
last_created datetime,
last_updated datetime,
password nvarchar(255),
password_salt nvarchar(255),
last_action varchar(1)
);
CREATE TABLE kzCountries(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name  varchar(255),
last_action varchar(1)
);

CREATE TABLE kzStates(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name varchar(255),
country_id int,
last_action varchar(1)
);
ALTER TABLE kzStates  WITH CHECK ADD  CONSTRAINT FK_kzCities_kzStates FOREIGN KEY(country_id)
REFERENCES kzCountries (id);

CREATE TABLE kzCurrencies(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name nvarchar(255),
code nvarchar(10),
last_action varchar(1)
);

CREATE TABLE kzCities(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name varchar(255),
country_id int,
currency_id int,
last_action varchar(1)
);
ALTER TABLE kzCities  WITH CHECK ADD  CONSTRAINT FK_kzCities_kzCountries FOREIGN KEY(country_id)
REFERENCES kzCountries (id);
ALTER TABLE kzCities  WITH CHECK ADD  CONSTRAINT FK_kzCities_kzCurrencies FOREIGN KEY(currency_id)
REFERENCES kzCurrencies (id);

CREATE TABLE kzStatus(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name varchar(255),
last_action varchar(1)
);

CREATE TABLE kzMerchants(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name nvarchar(255),
address_line1 nvarchar(255),
address_line2 nvarchar(255),
city_id int,
postcode varchar(10),
contact_number varchar(255),
email varchar(255),
website varchar(255),
facebook nvarchar(255),
latitude	float,
longitude	float,
description ntext,

status_id int,
last_created datetime,
last_updated datetime,
last_action varchar(1)
);
ALTER TABLE kzMerchants  WITH CHECK ADD  CONSTRAINT FK_kzMerchants_kzCities FOREIGN KEY(city_id)
REFERENCES kzCities (id);
ALTER TABLE kzMerchants  WITH CHECK ADD  CONSTRAINT FK_kzMerchants_kzStatus FOREIGN KEY(status_id)
REFERENCES kzStatus (id);

CREATE TABLE kzMerchantImages(
merchant_id int,
image_id int,
image_url nvarchar(255),
CONSTRAINT PK_kzMerchantImages PRIMARY KEY (merchant_id,image_id)
);


CREATE TABLE kzPrize(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name nvarchar(255),
sponsor_name nvarchar(255),
price decimal(18, 0),
description ntext,
terms ntext,
detail ntext,
expiry_date datetime,
publish_date datetime,
group_name nvarchar(255),
total_revenue decimal(18,0),
cal_total_revenue decimal(18,0),
game_type int,
fake_visualmeter decimal(18,0),
last_created datetime,
last_updated datetime,
last_action varchar(1)
);


CREATE TABLE kzInventoryItemTypes(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name nvarchar(255),
last_action varchar(1)
);

CREATE TABLE kzInventoryItems(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name nvarchar(255),
short_desc nvarchar(255),
price decimal(18,2),
margin decimal(18,2),
general_description ntext,
description ntext,
merchant_id int,
keywords nvarchar(255),
inventoryitem_type_id int,
discount decimal(18,2),
expiry_date datetime,
maximumsales decimal(18,0),
remainsales decimal(18,0),
publish_date datetime,
minimumtarget decimal(18,0),
salesvisualmeter decimal(18,0),
prize_id int,
terms ntext,
created_by int,
last_created datetime,
last_updated datetime,
flag bit,
featured bit,
featured_seq int,
featured_text ntext,
email_flag bit,
city_id int,
draft bit,
last_action varchar(1)
);
ALTER TABLE kzInventoryItems  WITH CHECK ADD  CONSTRAINT FK_kzInventoryItems_kzAdmins FOREIGN KEY(created_by)
REFERENCES kzAdmins (id);
ALTER TABLE kzInventoryItems  WITH CHECK ADD  CONSTRAINT FK_kzInventoryItems_kzMerchants FOREIGN KEY(merchant_id)
REFERENCES kzMerchants (id);
ALTER TABLE kzInventoryItems  WITH CHECK ADD  CONSTRAINT FK_kzInventoryItems_kzInventoryItemTypes FOREIGN KEY(inventoryitem_type_id)
REFERENCES kzInventoryItemTypes (id);
ALTER TABLE kzInventoryItems  WITH CHECK ADD  CONSTRAINT FK_kzInventoryItems_kzPrize FOREIGN KEY(prize_id)
REFERENCES kzPrize (id);
ALTER TABLE kzInventoryItems  WITH CHECK ADD  CONSTRAINT FK_kzInventoryItems_kzCities FOREIGN KEY(city_id)
REFERENCES kzCities (id);




CREATE TABLE kzFlashDeal(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
inventoryitem_id int,
discount decimal(18,2),
start_time datetime,
end_time datetime,
flag bit,
last_action varchar(1),
email_flag bit
);
ALTER TABLE kzFlashDeal  WITH CHECK ADD  CONSTRAINT FK_kzFlashDeal_kzInventoryItems FOREIGN KEY(inventoryitem_id)
REFERENCES kzInventoryItems (id);

CREATE TABLE kzImages(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
url nvarchar(255),
last_action varchar(1)
);


CREATE TABLE kzInventoryItemImages(
inventoryitem_id int,
image_id int,
main bit,
CONSTRAINT PK_kzInventoryItemImages PRIMARY KEY (inventoryitem_id,image_id)
);

CREATE TABLE kzTags(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name nvarchar(255),
showAsCategory bit,
parent_id int,
last_created datetime,
last_updated datetime,
last_action varchar(1)
);
ALTER TABLE kzTags  WITH CHECK ADD  CONSTRAINT FK_kzTags_kzTags FOREIGN KEY(parent_id)
REFERENCES kzTags (id);

CREATE TABLE kzInventoryItemTags(
inventoryitem_id int,
tag_id int,
CONSTRAINT PK_kzInventoryItemTags PRIMARY KEY (inventoryitem_id,tag_id)
);


CREATE TABLE kzPrizeImages(
prize_id int,
image_id int,
main bit,
CONSTRAINT PK_kzPrizeImages PRIMARY KEY (prize_id,image_id)
);


CREATE TABLE kzUserStatus(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name varchar(255),
last_action varchar(1)
);

CREATE TABLE kzUsers(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
first_name nvarchar(255),
last_name nvarchar(255),
email varchar(255),
gender int,
dateofbirth datetime,
image_id int,
last_created datetime,
last_updated datetime,
last_login datetime,
last_lockout datetime,
password nvarchar(255),
password_salt nvarchar(255),
userstatus_id int,
notif bit,
locked_status bit,
resetpass nvarchar(255),
resetpass_expire datetime,
token nvarchar(255),
token_salt nvarchar(255),
token_expire datetime
);
ALTER TABLE kzUsers  WITH CHECK ADD  CONSTRAINT FK_kzUsers_kzImages FOREIGN KEY(image_id)
REFERENCES kzImages (id);
ALTER TABLE kzUsers  WITH CHECK ADD  CONSTRAINT FK_kzUsers_kzUserStatus FOREIGN KEY(userstatus_id)
REFERENCES kzUserStatus (id);



CREATE TABLE kzWishList(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
user_id int,
inventoryitem_id int,
last_created datetime,
last_updated datetime,
last_action varchar(1)
);
ALTER TABLE kzWishList  WITH CHECK ADD  CONSTRAINT FK_kzWishListt_kzUsers FOREIGN KEY(user_id)
REFERENCES kzUsers (id);
ALTER TABLE kzWishList  WITH CHECK ADD  CONSTRAINT FK_kzWishList_kzInventoryItems FOREIGN KEY(inventoryitem_id)
REFERENCES kzInventoryItems (id);


CREATE TABLE kzPromotion(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
code varchar(255),
type int,
value decimal(18,0),
flag bit,
valid_date datetime,
last_created datetime,
last_updated datetime,
last_action varchar(1)
);


CREATE TABLE kzShippingUser(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
user_id int,
first_name nvarchar(255),
last_name nvarchar(255),
phone varchar(255),
gender int,
address_line1 nvarchar(255),
address_line2 nvarchar(255),
city nvarchar(255),
state nvarchar(255),
country nvarchar(255),
zipcode varchar(10),
gift bit,
note ntext,
last_action varchar(1)
);
ALTER TABLE kzShippingUser  WITH CHECK ADD  CONSTRAINT FK_kzShippingUser_kzUsers FOREIGN KEY(user_id)
REFERENCES kzUsers (id);


CREATE TABLE kzBillingUser(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
user_id int,
payment_method int,
payment_cc varchar(255),
payment_ccv varchar(3),
payment_expm int,
payment_expy int,
first_name nvarchar(255),
last_name nvarchar(255),
phone varchar(255),
gender int,
address_line1 nvarchar(255),
address_line2 nvarchar(255),
city nvarchar(255),
state nvarchar(255),
country nvarchar(255),
zipcode varchar(10),
last_action varchar(1)
);
ALTER TABLE kzBillingUser  WITH CHECK ADD  CONSTRAINT FK_kzBillingUser_kzUsers FOREIGN KEY(user_id)
REFERENCES kzUsers (id);

CREATE TABLE kzReview(
id  bigint IDENTITY(1,1) PRIMARY KEY NOT NULL,
inventoryitem_id int,
user_id int,
rating int,
message ntext,
review_date datetime,
last_action varchar(1)
);
ALTER TABLE kzReview  WITH CHECK ADD  CONSTRAINT FK_kzReview_kzInventoryItems FOREIGN KEY(inventoryitem_id)
REFERENCES kzInventoryItems (id);
ALTER TABLE kzReview  WITH CHECK ADD  CONSTRAINT FK_kzReview_kzUsers FOREIGN KEY(user_id)
REFERENCES kzUsers (id);


CREATE TABLE kzFacebook(
id  bigint IDENTITY(1,1) PRIMARY KEY NOT NULL,
facebookid nvarchar(255),
user_id int,
accesstoken nvarchar(255),
link_date datetime
);
ALTER TABLE kzFacebook  WITH CHECK ADD  CONSTRAINT FK_kzFacebook_kzUsers FOREIGN KEY(user_id)
REFERENCES kzUsers (id);

CREATE TABLE kzVariance(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name nvarchar(255),
price decimal(18,2),
discount decimal(18,2),
inventoryitem_id int,
margin decimal(18,2),
available_limit int,
sku varchar(255),
);
ALTER TABLE kzVariance  WITH CHECK ADD  CONSTRAINT FK_kzVariance_kzInventoryItems FOREIGN KEY(inventoryitem_id)
REFERENCES kzInventoryItems (id);

CREATE TABLE kzTransactions(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
user_id int,
promotion_id int,
kpoint decimal(18,0),
transaction_date datetime,
transaction_status int,
process_status int,
process_date datetime,
participate_game bit,
last_action varchar(1),
tranID nvarchar(255),
orderid nvarchar(255),
status nvarchar(255),
domain nvarchar(255),
amount decimal(18,2),
currency nvarchar(255),
appcode nvarchar(255),
paydate datetime,
skey nvarchar(255),
channel nvarchar(255),
error_code nvarchar(255),
error_desc nvarchar(255)
);
ALTER TABLE kzTransactions  WITH CHECK ADD  CONSTRAINT FK_kzTransactions_kzUsers FOREIGN KEY(user_id)
REFERENCES kzUsers (id);
ALTER TABLE kzTransactions  WITH CHECK ADD  CONSTRAINT FK_kzTransactions_kzPromotion FOREIGN KEY(promotion_id)
REFERENCES kzPromotion (id);

CREATE TABLE kzTransactionDetails(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
transaction_id int,
inventoryitem_id int,
flashdeal_id int NULL,
variance nvarchar(255),
qty int,
discount decimal(18,2),
price decimal(18,2)
);
ALTER TABLE kzTransactionDetails  WITH CHECK ADD  CONSTRAINT FK_kzTransactionDetails_kzTransactions FOREIGN KEY(transaction_id)
REFERENCES kzTransactions (id);
ALTER TABLE kzTransactionDetails  WITH CHECK ADD  CONSTRAINT FK_kzTransactionDetails_kzInventoryItems FOREIGN KEY(inventoryitem_id)
REFERENCES kzInventoryItems (id);



CREATE TABLE kzShipping(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
transaction_id int,
first_name nvarchar(255),
last_name nvarchar(255),
phone varchar(255),
gender int,
address_line1 nvarchar(255),
address_line2 nvarchar(255),
city nvarchar(255),
state nvarchar(255),
country nvarchar(255),
zipcode varchar(10),
gift bit,
note ntext,
last_action varchar(1)
);
ALTER TABLE kzShipping  WITH CHECK ADD  CONSTRAINT FK_kzShipping_kzTransactions FOREIGN KEY(transaction_id)
REFERENCES kzTransactions (id);


CREATE TABLE kzBilling(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
transaction_id int,
payment_method int,
payment_cc varchar(255),
payment_ccv varchar(3),
payment_expm int,
payment_expy int,
first_name nvarchar(255),
last_name nvarchar(255),
phone varchar(255),
gender int,
address_line1 nvarchar(255),
address_line2 nvarchar(255),
city nvarchar(255),
state nvarchar(255),
country nvarchar(255),
zipcode varchar(10),
last_action varchar(1)
);
ALTER TABLE kzBilling  WITH CHECK ADD  CONSTRAINT FK_kzBilling_kzTransactions FOREIGN KEY(transaction_id)
REFERENCES kzTransactions (id);



CREATE TABLE kzWinner(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
prize_id int,
user_id int,
transaction_id int,
inventoryitem_id int,
create_date datetime
);
ALTER TABLE kzWinner  WITH CHECK ADD  CONSTRAINT FK_kzWinner_kzPrize FOREIGN KEY(prize_id)
REFERENCES kzPrize (id);
ALTER TABLE kzWinner  WITH CHECK ADD  CONSTRAINT FK_kzWinner_kzUsers FOREIGN KEY(user_id)
REFERENCES kzUsers (id);
ALTER TABLE kzWinner  WITH CHECK ADD  CONSTRAINT FK_kzWinner_kzTransactions FOREIGN KEY(transaction_id)
REFERENCES kzTransactions (id);
ALTER TABLE kzWinner  WITH CHECK ADD  CONSTRAINT FK_kzWinner_kzInventoryItems FOREIGN KEY(inventoryitem_id)
REFERENCES kzInventoryItems (id);


CREATE TABLE kzKPointAction(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
code int,
description nvarchar(255),
amount decimal(18,0),
last_updated datetime,
last_action varchar(1)
)

CREATE TABLE kzKPointTrxH(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
user_id int,
balance decimal(18,0),
last_updated datetime,
last_action varchar(1)
);
ALTER TABLE kzKPointTrxH  WITH CHECK ADD  CONSTRAINT FK_kzKPointTrxH_kzUsers FOREIGN KEY(user_id)
REFERENCES kzUsers (id);

CREATE TABLE kzKPointTrxD(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
kpointh_id int,
amount decimal(18,0),
action_type int,
from_user int NULL,
to_user int NULL,
remarks ntext,
inventoryitem_id int NULL,
transaction_id int NULL,
last_created datetime,

);
ALTER TABLE kzKPointTrxD  WITH CHECK ADD  CONSTRAINT FK_kzKPointTrxD_kzKPointTrxH FOREIGN KEY(kpointh_id)
REFERENCES kzKPointTrxH (id);
ALTER TABLE kzKPointTrxD  WITH CHECK ADD  CONSTRAINT FK_kzKPointTrxD_kzInventoryItems FOREIGN KEY(inventoryitem_id)
REFERENCES kzInventoryItems (id);
ALTER TABLE kzKPointTrxD  WITH CHECK ADD  CONSTRAINT FK_kzKPointTrxD_kzTransactions FOREIGN KEY(transaction_id)
REFERENCES kzTransactions (id);


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

CREATE TABLE kzSubscribeEmail(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
email nvarchar(255),
created_date datetime
);


CREATE TABLE kzStatic(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name nvarchar(255),
description ntext
);


CREATE TABLE kzGames(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
prize_id int,
name nvarchar(255),
description ntext,
instruction ntext,
image_id int,
expiry_date datetime,
hidden_latitude	float,
hidden_longitude float,
last_created datetime,
last_updated datetime,
last_action varchar(1)
);
ALTER TABLE kzGames  WITH CHECK ADD  CONSTRAINT FK_kzGames_kzPrize FOREIGN KEY(prize_id)
REFERENCES kzPrize (id);


CREATE TABLE kzGameTransactions(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
user_id int,
transaction_id int,
game_id int,
hidden_latitude	float,
hidden_longitude float,
timeused int,
transaction_date datetime,
last_action varchar(1)
);
ALTER TABLE kzGameTransactions  WITH CHECK ADD  CONSTRAINT FK_kzGameTransactions_kzUsers FOREIGN KEY(user_id)
REFERENCES kzUsers (id);
ALTER TABLE kzGameTransactions  WITH CHECK ADD  CONSTRAINT FK_kzGameTransactions_kzTransactions FOREIGN KEY(transaction_id)
REFERENCES kzTransactions (id);
ALTER TABLE kzGameTransactions  WITH CHECK ADD  CONSTRAINT FK_kzGameTransactions_kzGames FOREIGN KEY(game_id)
REFERENCES kzGames (id);

CREATE TABLE kzEmail(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
email nvarchar(255)
)

CREATE TABLE kzEmailSendLog(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
email nvarchar(255),
status int,
subject nvarchar(255),
creationdate datetime
)

CREATE TABLE kzCallbackMOLLog(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
creationdate datetime,
nbcb nvarchar(255),
tranID nvarchar(255),
orderid nvarchar(255),
status nvarchar(255),
domain nvarchar(255),
amount decimal(18,2),
currency nvarchar(255),
appcode nvarchar(255),
paydate datetime,
skey nvarchar(255),
channel nvarchar(255),
error_code nvarchar(255),
error_desc nvarchar(255)
)

CREATE TABLE kzPreCode(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
code nvarchar(255),
transaction_id int
)

CREATE TABLE kzWebExceptionLogger(
id  int IDENTITY(1,1) PRIMARY KEY NOT NULL,
last_created datetime,
url nvarchar(255),
ip_address varchar(255),
logger ntext,
err_exception ntext,
err_message ntext
);





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
transaction_date datetime,
last_action varchar(1)
);
ALTER TABLE kzGameTransactions  WITH CHECK ADD  CONSTRAINT FK_kzGameTransactions_kzUsers FOREIGN KEY(user_id)
REFERENCES kzUsers (id);
ALTER TABLE kzGameTransactions  WITH CHECK ADD  CONSTRAINT FK_kzGameTransactions_kzTransactions FOREIGN KEY(transaction_id)
REFERENCES kzTransactions (id);
ALTER TABLE kzGameTransactions  WITH CHECK ADD  CONSTRAINT FK_kzGameTransactions_kzGames FOREIGN KEY(game_id)
REFERENCES kzGames (id);

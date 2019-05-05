CREATE TABLE kzStates(
id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
name varchar(255),
country_id int,
last_action varchar(1)
);
ALTER TABLE kzStates  WITH CHECK ADD  CONSTRAINT FK_kzCities_kzStates FOREIGN KEY(country_id)
REFERENCES kzCountries (id);

insert into kzStates(name,country_id,last_action)values('Johor',1,'1');
insert into kzStates(name,country_id,last_action)values('Kedah',1,'1');
insert into kzStates(name,country_id,last_action)values('Kelantan',1,'1');
insert into kzStates(name,country_id,last_action)values('Malacca',1,'1');
insert into kzStates(name,country_id,last_action)values('Negeri Sembilan',1,'1');
insert into kzStates(name,country_id,last_action)values('Pahang',1,'1');
insert into kzStates(name,country_id,last_action)values('Perak',1,'1');
insert into kzStates(name,country_id,last_action)values('Perilis',1,'1');
insert into kzStates(name,country_id,last_action)values('Penang',1,'1');
insert into kzStates(name,country_id,last_action)values('Sabah',1,'1');
insert into kzStates(name,country_id,last_action)values('Sarawak',1,'1');
insert into kzStates(name,country_id,last_action)values('Selangor',1,'1');
insert into kzStates(name,country_id,last_action)values('Terengganu',1,'1');
insert into kzStates(name,country_id,last_action)values('Federal Territory of Kuala Lumpur',1,'1');
insert into kzStates(name,country_id,last_action)values('Federal Territory of Labuan',1,'1');
insert into kzStates(name,country_id,last_action)values('ederal Territory of Putrajaya',1,'1');

alter table kzprize
add publish_date datetime

update kzPrize
set publish_date = '2014-08-01'

alter table kztransactions
add process_date datetime
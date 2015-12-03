delete from CategoryEntry
delete from Category
delete from Product

set identity_insert category on
insert into Category (CategoryID, Name)
select 1,	'RaMan' UNION
select 2,	'BatMon' UNION
select 3,	'Ra Trek' UNION
select 4,	'Star Mons' UNION
select 1, '' where 1=0
set identity_insert category off

DBCC CHECKIDENT(Category,RESEED,1);
DBCC CHECKIDENT(Category);

set identity_insert product on
insert into Product (ProductID, Name, Description, Price)
select 1,	'RaMan T-Shirt',	'Using the power of the databases, the RaMan t-shirt will protect the world from malformed database designs and ill-conceived data warehousing structures.<br/><br/>One Size Fits all!', 20 UNION
select 2,	'RaMan Cap',		'Using the power of the databases, the RaMan cap will protect the world from malformed database designs and ill-conceived data warehousing structures.<br/><br/>One Size Fits all!', 20 UNION
select 3,	'RaMan Underwear',	'Using the power of the databases, the RaMan underwear will cover the ass of those database designers who don''t want to become bad designers.<br/><br/>One Size Fits all!', 10 UNION
select 104,	'BatMon Ears',		'The BatMon ears give you bat like perception of data issues on the interWEBZ.', 10 UNION
select 105,	'BatMon Mobile',	'Cruise around in uhh... style? In the BatMon Mobile!', 4000 UNION
select 206,	'Ra Trek Oracle',	'Feel like cruising around the universe in style? Well thats too bad, all you get is a 4"x4" Star Trek Enterprise rip-off!', 50 UNION
select 207,	'Ra Trek Jump Suit','Feel like a part of the team! Why red? Because in software development, everyone to some extent is replaceable!', 50 UNION
select 308,	'Star Mons Speeder','Got Confused with Star Mons and Le Mons? Well this should fix that. Enjoy your own Star War... Ahem Star Mons speed! <br/><br/>Real Speed not included.', 5000 UNION
select 309,	'The Farce',		'The next best thing to The Force! The Farce!', 5000 UNION
select 1, '', '', 0 where 1=0
set identity_insert product off

DBCC CHECKIDENT(Product,RESEED,1);
DBCC CHECKIDENT(Product);

insert into CategoryEntry(CategoryID, ProductID)
select 1, ProductID from Product where ProductID Between 1 and 100 UNION
select 2, ProductID from Product where ProductID Between 101 and 200 UNION
select 3, ProductID from Product where ProductID Between 201 and 300 UNION
select 4, ProductID from Product where ProductID Between 301 and 400 UNION
select 1,1 where 1=0
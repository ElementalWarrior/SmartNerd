delete from CategoryEntry
insert into CategoryEntry(CategoryEntryID, CategoryID, ProductID)
select 1, 1, 1 UNION
select 2, 1, 2 UNION
select 3, 1, 3 UNION
select 1,1,1 where 1=0

delete from Product
insert into Product (ProductID, Name, Description, Price)
select 1,	'RaMan T-Shirt',	'Using the power of the databases, the RaMan t-shirt will protect the world from malformed database designs and ill-conceived data warehousing structures.<br/><br/>One Size Fits all!', 20 UNION
select 2,	'RaMan Cap',		'Using the power of the databases, the RaMan cap will protect the world from malformed database designs and ill-conceived data warehousing structures.<br/><br/>One Size Fits all!', 20 UNION
select 3,	'RaMan Underwear',	'Using the power of the databases, the RaMan underwear will cover the ass of those database designers who don''t want to become bad designers.<br/><br/>One Size Fits all!', 10 UNION
select 0, '', '', 0 where 1=0

delete from Category
insert into Category (CategoryID, Name)
select 1,	'RaMan' UNION
select 2,	'BatMon' UNION
select 3,	'Ra Trek' UNION
select 4,	'Star Mons' UNION
select 1, '' where 1=0

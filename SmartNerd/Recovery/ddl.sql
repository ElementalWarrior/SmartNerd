drop table CategoryEntry;
drop table Category;
drop table OrderProduct;
drop table Inventory;
drop table Product;
drop table PhysicalLocation;
drop table Orders;
drop table AccountAddress;
drop table Address;
drop table Payment;
create table Address
(
	AddressID int primary key identity(1,1),
	FullName varchar(100) not null,
	Line1 varchar(100) not null,
	Line2 varchar(100),
	City varchar(50) not null,
	StateOrProvince varchar(2) not null,
	ZipCode varchar(20) not null,
	County varchar(2) not null
);
create table AccountAddress
(
	AccountAddressID int primary key identity(1,1),
	AddressID int not null,
	UserID nvarchar(128) not null,
	AddressType varchar(20) not null,
	foreign key (AddressID) references Address(AddressID),
	foreign key (UserID) references AspNetUsers(Id),
	constraint chk_AddressType check (addresstype in ('Mailing', 'Billing'))
);
create table Orders
(
	OrderID int primary key identity(1,1),
	AccountID uniqueidentifier,
	CartID uniqueidentifier default newid() not null,
	OrderTotal decimal not null,
	DateCreated datetime not null default getdate(),
	DatePlaced datetime,
	AddressID int,
	foreign key (AddressID) references Address(AddressID),
);
create table PhysicalLocation
(
	PhysicalLocationID int primary key identity(1,1),
	Name varchar(50) not null,
	LocationType varchar(20) not null,
	constraint chk_LocationType check (locationtype in ('Store', 'Warehouse'))
);
create table Product
(
	ProductID int primary key identity(1,1),
	Name varchar(50) not null,
	Description text,
	Price decimal not null,
	DateCreated datetime not null default getdate(),
	Inventory int not null default 0
);
create table Inventory
(
	InventoryID int primary key identity(1,1),
	PhysicalLocationID int not null,
	ProductID int not null,
	Count int not null,
	foreign key (PhysicalLocationID) references PhysicalLocation(PhysicalLocationID),
	foreign key (ProductID) references Product(ProductID)
);
create table OrderProduct
(
	OrderProductID int primary key identity(1,1),
	ProductID int not null,
	OrderID int not null,
	Quantity int not null,
	foreign key (ProductID) references Product(ProductID),
	foreign key (OrderID) references Orders(OrderID)
);
create table Category
(
	CategoryID int primary key identity(1,1),
	Name varchar(50) not null
);
create table CategoryEntry
(
	CategoryEntryID int primary key identity(1,1),
	CategoryID int not null,
	ProductID int not null,
	foreign key (CategoryID) references Category(CategoryID),
	foreign key (ProductID) references Product(ProductID)
);
create table Payment
(
	PaymentID int primary key identity(1,1),
	OrderID int not null,
	CardType varchar(20) not null,
	FourDigits varchar(4) not null,
	Amount Decimal not null,
	DateCreated datetime not null default getdate(),
	PayPalID varchar(50) not null,
	foreign key (OrderID) references Orders(OrderID)
);

--CREATE TABLE [dbo].[AspNetUsers](
--	[Id] [nvarchar](128) NOT NULL,
--	[UserName] [nvarchar](max) NULL,
--	[PasswordHash] [nvarchar](max) NULL,
--	[SecurityStamp] [nvarchar](max) NULL,
--	[FirstName] [nvarchar](max) NULL,
--	[LastName] [nvarchar](max) NULL,
--	[Email] [nvarchar](max) NULL,
--	[Phone] [nvarchar](max) NULL,
--	[Discriminator] [nvarchar](128) NOT NULL,
--)

--CREATE TABLE [dbo].[AspNetRoles](
--	[Id] [nvarchar](128) NOT NULL,
--	[Name] [nvarchar](max) NOT NULL,
--)
--CREATE TABLE [dbo].[AspNetUserRoles](
--	[UserId] [nvarchar](128) NOT NULL,
--	[RoleId] [nvarchar](128) NOT NULL,
--	FOREIGN KEY([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]),
--	FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
--)

--CREATE TABLE [dbo].[AspNetUserLogins](
--	[UserId] [nvarchar](128) NOT NULL,
--	[LoginProvider] [nvarchar](128) NOT NULL,
--	[ProviderKey] [nvarchar](128) NOT NULL,
--	FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
--)
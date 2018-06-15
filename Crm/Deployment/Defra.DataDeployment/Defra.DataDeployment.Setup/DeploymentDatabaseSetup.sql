create table srcbusinessunit
(
businessunitid uniqueidentifier primary key not null,
name nvarchar(200) not null,
parentbusinessunitid uniqueidentifier not null
)

create table destbusinessunit
(
businessunitid uniqueidentifier primary key not null,
name nvarchar(200) not null,
parentbusinessunitid uniqueidentifier
)

create table srclpteams
(
teamid uniqueidentifier primary key not null,
name nvarchar(200) not null,
rolebusinessunitname nvarchar(200) not null
)

create table destrole
(
roleid uniqueidentifier primary key not null,
rolebusinessunitname nvarchar(200) not null,
name nvarchar(200) not null
)

create table srcrolemembership
(
teamroleid uniqueidentifier primary key not null,
teamid uniqueidentifier not null,
roleid uniqueidentifier not null,
rolename nvarchar(200) not null,
rolebusinessunitname nvarchar(200) not null
)

create table destrolemembership
(
teamroleid uniqueidentifier primary key not null,
teamid uniqueidentifier not null,
roleid uniqueidentifier not null,
rolename nvarchar(200) not null,
rolebusinessunitname nvarchar(200) not null
)
create table srcqueues
(
	queueid uniqueidentifier primary key not null,
	businessunitid uniqueidentifier  null,
	[description] ntext  null,
	emailaddress nvarchar(100)  null,
	name nvarchar(200)  null,
	onwerid uniqueidentifier null,
	owneridtype nvarchar(64)  null,
	queueviewtype int null,
	statecode int null,
	statuscode int null,
	outgoingemailstatus int null,
	incomingemailstatus int null,
	mailboxstatus int null
)

create table destqueues
(
	queueid uniqueidentifier primary key not null,
	businessunitid uniqueidentifier  null,
	[description] ntext  null,
	emailaddress nvarchar(100)  null,
	name nvarchar(200)  null,
	onwerid uniqueidentifier null,
	owneridtype nvarchar(64)  null,
	queueviewtype int null,
	statecode int null,
	statuscode int null,
	outgoingemailstatus int null,
	incomingemailstatus int null,
	mailboxstatus int null
)
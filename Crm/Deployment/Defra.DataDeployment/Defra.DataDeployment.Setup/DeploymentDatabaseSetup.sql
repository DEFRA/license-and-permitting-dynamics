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
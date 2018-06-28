
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
parentbusinessunitid uniqueidentifier,
[parentbusinessunitname] [nvarchar](200) NULL
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

CREATE TABLE [dbo].[queueemails](
	[queue_name] [nvarchar](300) NOT NULL,
	[email] [nvarchar](100) NULL,
	[environment] [nvarchar](300) NULL
) ON [PRIMARY]
GO






delete from queueemails where environment is not null

-- CoreLp
insert into queueemails (queue_name, email, environment)
SELECT queue_name, replace(email,'@environment-agency.gov.uk','@defradev.onmicrosoft.com'), 'ea-lp-dev-corelp'
  from [dbo].queueemails
  where environment is null

-- Waste
insert into queueemails (queue_name, email, environment)
SELECT queue_name, replace(email,'@environment-agency.gov.uk','@defradev.onmicrosoft.com'), 'ea-lp-dev-waste'
  from [dbo].queueemails
  where environment is null
  
-- Master
insert into queueemails (queue_name, email, environment)
SELECT queue_name, replace(email,'@environment-agency.gov.uk','@defradev.onmicrosoft.com'), 'ea-lp-dev-master'
  from [dbo].queueemails
  where environment is null
  
-- QA
insert into queueemails (queue_name, email, environment)
SELECT queue_name, replace(email,'@environment-agency.gov.uk','@defradev.onmicrosoft.com'), 'ea-lp-qa-master'
  from [dbo].queueemails
  where environment is null
  
-- UAT
insert into queueemails (queue_name, email, environment)
SELECT queue_name, replace(email,'@environment-agency.gov.uk','@defra.onmicrosoft.com'), 'ea-lp-uat'
  from [dbo].queueemails
  where environment is null

-- PROD
insert into queueemails (queue_name, email, environment)
SELECT queue_name, email, 'ea-lp-prod'
  from [dbo].queueemails
  where environment is null


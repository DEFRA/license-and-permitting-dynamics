﻿USE [master]
GO

/****** Object:  Database [DEFRASSIS]    Script Date: 26/01/2019 12:42:09 ******/
CREATE DATABASE [DEFRASSIS]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'LPConfigDataMigration', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQL2017\MSSQL\DATA\LPConfigDataMigration.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'LPConfigDataMigration_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQL2017\MSSQL\DATA\LPConfigDataMigration_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DEFRASSIS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [DEFRASSIS] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [DEFRASSIS] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [DEFRASSIS] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [DEFRASSIS] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [DEFRASSIS] SET ARITHABORT OFF 
GO

ALTER DATABASE [DEFRASSIS] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [DEFRASSIS] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [DEFRASSIS] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [DEFRASSIS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [DEFRASSIS] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [DEFRASSIS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [DEFRASSIS] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [DEFRASSIS] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [DEFRASSIS] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [DEFRASSIS] SET  DISABLE_BROKER 
GO

ALTER DATABASE [DEFRASSIS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [DEFRASSIS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [DEFRASSIS] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [DEFRASSIS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [DEFRASSIS] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [DEFRASSIS] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [DEFRASSIS] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [DEFRASSIS] SET RECOVERY FULL 
GO

ALTER DATABASE [DEFRASSIS] SET  MULTI_USER 
GO

ALTER DATABASE [DEFRASSIS] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [DEFRASSIS] SET DB_CHAINING OFF 
GO

ALTER DATABASE [DEFRASSIS] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [DEFRASSIS] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [DEFRASSIS] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [DEFRASSIS] SET  READ_WRITE 
GO


USE [DEFRASSIS]
GO

/****** Object:  Table [dbo].[destbusinessunit]    Script Date: 26/01/2019 12:43:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[destbusinessunit](
	[businessunitid] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](200) NOT NULL,
	[parentbusinessunitid] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[businessunitid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[destrolemembership]    Script Date: 26/01/2019 12:43:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[destrolemembership](
	[teamroleid] [uniqueidentifier] NOT NULL,
	[teamid] [uniqueidentifier] NOT NULL,
	[roleid] [uniqueidentifier] NOT NULL,
	[rolename] [nvarchar](200) NOT NULL,
	[rolebusinessunitname] [nvarchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[teamroleid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[destrole]    Script Date: 26/01/2019 12:43:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[destrole](
	[roleid] [uniqueidentifier] NOT NULL,
	[rolebusinessunitname] [nvarchar](200) NOT NULL,
	[name] [nvarchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[roleid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[srcrolemembership]    Script Date: 26/01/2019 12:43:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[srcrolemembership](
	[teamroleid] [uniqueidentifier] NOT NULL,
	[teamid] [uniqueidentifier] NOT NULL,
	[roleid] [uniqueidentifier] NOT NULL,
	[rolename] [nvarchar](200) NOT NULL,
	[rolebusinessunitname] [nvarchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[teamroleid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[srcbusinessunit]    Script Date: 26/01/2019 12:43:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[srcbusinessunit](
	[businessunitid] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](200) NOT NULL,
	[parentbusinessunitid] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[businessunitid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[srclpteams]    Script Date: 26/01/2019 12:43:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[srclpteams](
	[teamid] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](200) NOT NULL,
	[rolebusinessunitname] [nvarchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[teamid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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


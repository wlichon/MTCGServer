﻿/*
Deployment script for MTCG_DB

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "MTCG_DB"
:setvar DefaultFilePrefix "MTCG_DB"
:setvar DefaultDataPath "C:\Users\wikto\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\"
:setvar DefaultLogPath "C:\Users\wikto\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Rename refactoring operation with key 27fdd335-dd1d-4c11-bffc-93c3b7027aab, dceb28ec-990a-4aa2-b0aa-24f3fd3e2cd9 is skipped, element [dbo].[MTCGDeck].[Id] (SqlSimpleColumn) will not be renamed to DeckID';


GO
PRINT N'Altering Table [dbo].[MTCGUser]...';


GO
ALTER TABLE [dbo].[MTCGUser]
    ADD [FirstCard]  NVARCHAR (50) NULL,
        [SecondCard] NVARCHAR (50) NULL,
        [ThirdCard]  NVARCHAR (50) NULL,
        [FourthCard] NVARCHAR (50) NULL;


GO
PRINT N'Refreshing Procedure [dbo].[acquire_package]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[acquire_package]';


GO
-- Refactoring step to update target server with deployed transaction logs
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '27fdd335-dd1d-4c11-bffc-93c3b7027aab')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('27fdd335-dd1d-4c11-bffc-93c3b7027aab')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = 'dceb28ec-990a-4aa2-b0aa-24f3fd3e2cd9')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('dceb28ec-990a-4aa2-b0aa-24f3fd3e2cd9')

GO

GO
PRINT N'Update complete.';


GO

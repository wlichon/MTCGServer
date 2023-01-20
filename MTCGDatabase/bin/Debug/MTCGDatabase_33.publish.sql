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
PRINT N'Creating Procedure [dbo].[add_cards_to_deck]...';


GO
CREATE PROCEDURE [dbo].[add_cards_to_deck]
	@FirstCard nvarchar(50),
	@SecondCard nvarchar(50),
	@ThirdCard nvarchar(50),
	@FourthCard nvarchar(50)
AS

BEGIN


	IF NOT EXISTS(SELECT * FROM dbo.MTCGCard WHERE CardID = @FirstCard)
	BEGIN
		RAISERROR('Card 1 is not in users posession',16,1);
		RETURN
	END

	IF NOT EXISTS(SELECT * FROM dbo.MTCGCard WHERE CardID = @SecondCard)
	BEGIN
		RAISERROR('Card 2 is not in users posession',16,1);
		RETURN
	END

	IF NOT EXISTS(SELECT * FROM dbo.MTCGCard WHERE CardID = @ThirdCard)
	BEGIN
		RAISERROR('Card 3 is not in users posession',16,1);
		RETURN
	END

	IF NOT EXISTS(SELECT * FROM dbo.MTCGCard WHERE CardID = @FourthCard)
	BEGIN
		RAISERROR('Card 4 is not in users posession',16,1);
		RETURN
	END

	UPDATE dbo.MTCGUser SET FirstCard = @FirstCard, SecondCard = @SecondCard, ThirdCard = @ThirdCard, FourthCard = @FourthCard WHERE Username = 'kienboec'


END
GO
PRINT N'Update complete.';


GO
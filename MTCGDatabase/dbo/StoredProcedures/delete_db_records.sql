CREATE PROCEDURE [dbo].[delete_db_records]
	
AS
	DELETE FROM dbo.MTCGCard;
	DELETE FROM dbo.MTCGUser;
RETURN 0

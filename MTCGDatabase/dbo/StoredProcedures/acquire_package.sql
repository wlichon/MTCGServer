CREATE PROCEDURE [dbo].[acquire_package]
	@Username nvarchar(50)
AS

BEGIN TRANSACTION


	IF((SELECT Coins
		 FROM dbo.MTCGUser
		 WHERE Username = @Username) < 5)
	BEGIN
		RAISERROR('Coin balance is too low',16,1);
		RETURN
	END

	IF((SELECT COUNT(*) from (SELECT top 5 * from dbo.MTCGCard WHERE Username is NULL) x) < 5)
	BEGIN
		RAISERROR('Not enough packs',16,1);
		RETURN
	END

	ELSE

	BEGIN

	UPDATE dbo.MTCGUser
	SET Coins = Coins - 5
	WHERE Username = @Username;

	WITH q AS
	(
	SELECT TOP 5 * FROM dbo.MTCGCard WHERE Username is NULL ORDER BY CardNumber ASC
	)
	UPDATE q
	SET Username = @Username
	
	END


COMMIT TRANSACTION


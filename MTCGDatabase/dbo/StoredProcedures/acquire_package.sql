CREATE PROCEDURE [dbo].[acquire_package]
	@Username nvarchar(50)
AS

BEGIN TRANSACTION


	IF((SELECT Coins
		 FROM dbo.MTCGUser
		 WHERE Username = @Username) < 5)
	BEGIN
		RAISERROR('Coin balance is too low',16,1);
	END

	ELSE

	BEGIN

	UPDATE dbo.MTCGUser
	SET Coins = Coins - 5
	WHERE Username = @Username;

	UPDATE top (5) dbo.MTCGCard
	SET Username = @Username
	WHERE Username is NULL;
	END


COMMIT TRANSACTION


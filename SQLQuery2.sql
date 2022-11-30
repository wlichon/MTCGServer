BEGIN TRANSACTION


	IF((SELECT Coins
		 FROM dbo.MTCGUser
		 WHERE Username = 'admin') < 5)
	BEGIN
		RAISERROR('Coin balance is too low',16,1);
	END

	ELSE
	BEGIN

	UPDATE dbo.MTCGUser
	SET Coins = Coins - 5
	WHERE Username = 'admin';

	UPDATE top (5) dbo.MTCGCard
	SET Username = 'admin'
	WHERE Username is NULL;
	END


COMMIT TRANSACTION


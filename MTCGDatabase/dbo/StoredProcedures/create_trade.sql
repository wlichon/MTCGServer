CREATE PROCEDURE [dbo].[create_trade]
	@Username nvarchar(50),
	@Id nvarchar(50),
	@CardToTrade nvarchar(50),
	@isSpell bit,
	@MinDmg int
AS

BEGIN TRANSACTION


	IF((SELECT COUNT(*)
		 FROM dbo.MTCGCard
		 WHERE Username = @Username AND CardID = @CardToTrade) = 0)
	BEGIN
		RAISERROR('Card is not in users possesion',16,1);
		RETURN
	END

	IF((SELECT COUNT(*) from (SELECT  * from dbo.MTCGDeals WHERE Id = @Id) x) != 0)
	BEGIN
		RAISERROR('Trade deal with provided ID already exists',16,1);
		RETURN
	END

	IF((SELECT COUNT(*) from (SELECT FirstCard, SecondCard, ThirdCard, FourthCard
		FROM dbo.MTCGUser
		WHERE (FirstCard = @CardToTrade OR SecondCard = @CardToTrade OR ThirdCard = @CardToTrade OR FourthCard = @CardToTrade) AND Username = 'altenhof') x) != 0)
	BEGIN
		RAISERROR('Card is registered in users deck therefore it cannot be traded',16,1);
		RETURN
	END

	ELSE

	BEGIN

	INSERT INTO dbo.MTCGDeals (Id,CardToTrade,isSpell,MinDmg) values (@Id,@CardToTrade,@isSpell,@MinDmg)
	
	UPDATE dbo.MTCGCard SET isTraded = 1 WHERE CardID = @CardToTrade;
	
	END


COMMIT TRANSACTION
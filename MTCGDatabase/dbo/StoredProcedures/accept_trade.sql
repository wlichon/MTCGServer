CREATE PROCEDURE [dbo].[accept_trade]
	@Username nvarchar(50),
	@TradeId nvarchar(50),
	@CardToTrade nvarchar(50)
AS

BEGIN TRANSACTION



	IF((SELECT COUNT(*)
		 FROM dbo.MTCGCard
		 WHERE Username = @Username AND CardID = @CardToTrade) = 0)
	BEGIN
		RAISERROR('Card is not in users possesion',16,1);
		RETURN
	END

	IF((SELECT COUNT(*)
		 FROM dbo.MTCGDeals
		 WHERE Id = @TradeId) = 0)
	BEGIN
		RAISERROR('Trade deal with provided Id does not exist',16,1);
		RETURN
	END
	DECLARE @DamageBuyer int;
	SET @DamageBuyer = (SELECT Damage FROM dbo.MTCGCard WHERE CardID = @CardToTrade);


	IF((SELECT MinDmg FROM dbo.MTCGDeals WHERE Id = @TradeId) > @DamageBuyer)
	BEGIN
		RAISERROR('Damage of offered card is too low',16,1);
		RETURN
	END

	IF((SELECT COUNT(*) from (SELECT FirstCard, SecondCard, ThirdCard, FourthCard
		FROM dbo.MTCGUser
		WHERE (FirstCard = @CardToTrade OR SecondCard = @CardToTrade OR ThirdCard = @CardToTrade OR FourthCard = @CardToTrade) AND Username = @Username) x) != 0)
	BEGIN
		RAISERROR('Card is registered in users deck therefore it cannot be traded',16,1);
		RETURN
	END

	DECLARE @SellerCard nvarchar(50);
	SET @SellerCard = (SELECT CardToTrade FROM dbo.MTCGDeals WHERE Id = @TradeId);

	IF( (SELECT isSpell FROM dbo.MTCGDeals WHERE Id = @TradeId) != (SELECT isSpell FROM dbo.MTCGCard WHERE CardID = @CardToTrade))
	BEGIN
		RAISERROR('Card does not match the requested type',16,1);
		RETURN
	END

	DECLARE @SellerOwner nvarchar(50);
	SET @SellerOwner = (SELECT Username FROM dbo.MTCGCard WHERE CardID = @SellerCard);

	IF( @SellerOwner = @Username)
	BEGIN
		RAISERROR('Cannot trade with yourself',16,1);
		RETURN
	END

	

	ELSE

	BEGIN

	
	

	
	UPDATE dbo.MTCGCard SET Username = @Username WHERE CardID = @SellerCard;

	UPDATE dbo.MTCGCard SET Username = @SellerOwner WHERE CardID = @CardToTrade;

	UPDATE dbo.MTCGCard SET isTraded = 0 WHERE CardID = @SellerCard;

	DELETE FROM dbo.MTCGDeals WHERE Id = @TradeId;
	
	END


COMMIT TRANSACTION
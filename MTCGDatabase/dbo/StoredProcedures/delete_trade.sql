CREATE PROCEDURE [dbo].[delete_trade]
	@Username nvarchar(50),
	@TradeId nvarchar(50)
	
AS

BEGIN TRANSACTION
	
	DECLARE @TradeCreator nvarchar(50);
	

	SET @TradeCreator = (SELECT Username FROM dbo.MTCGCard WHERE CardID = (SELECT CardToTrade FROM dbo.MTCGDeals WHERE Id = @TradeId));

	IF(@TradeCreator != @Username)
	BEGIN
		RAISERROR('You are not the creator of this trade, therefore you cannot delete it',16,1);
		RETURN
	END


	

	ELSE

	BEGIN

	DELETE FROM dbo.MTCGDeals WHERE Id = @TradeId;
	
	END


COMMIT TRANSACTION
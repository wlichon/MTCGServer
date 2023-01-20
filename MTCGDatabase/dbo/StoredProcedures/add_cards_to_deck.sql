CREATE PROCEDURE [dbo].[add_cards_to_deck]
	@FirstCard nvarchar(50),
	@SecondCard nvarchar(50),
	@ThirdCard nvarchar(50),
	@FourthCard nvarchar(50),
	@Username nvarchar(50)
AS

BEGIN


	IF NOT EXISTS(SELECT * FROM dbo.MTCGCard WHERE CardID = @FirstCard AND Username = @Username AND isTraded = 0)
	BEGIN
		RAISERROR('Card 1 is not in users posession or is being traded',16,1);
		RETURN
	END

	IF NOT EXISTS(SELECT * FROM dbo.MTCGCard WHERE CardID = @SecondCard AND Username = @Username AND isTraded = 0)
	BEGIN
		RAISERROR('Card 2 is not in users posession or is being traded',16,1);
		RETURN
	END

	IF NOT EXISTS(SELECT * FROM dbo.MTCGCard WHERE CardID = @ThirdCard AND Username = @Username AND isTraded = 0)
	BEGIN
		RAISERROR('Card 3 is not in users posession or is being traded',16,1);
		RETURN
	END

	IF NOT EXISTS(SELECT * FROM dbo.MTCGCard WHERE CardID = @FourthCard AND Username = @Username AND isTraded = 0)
	BEGIN
		RAISERROR('Card 4 is not in users posession or is being traded',16,1);
		RETURN
	END

	UPDATE dbo.MTCGUser SET FirstCard = @FirstCard, SecondCard = @SecondCard, ThirdCard = @ThirdCard, FourthCard = @FourthCard WHERE Username = @Username


END

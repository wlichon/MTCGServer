CREATE PROCEDURE [dbo].[reset_card_owners]
AS

BEGIN
	UPDATE dbo.MTCGCard
	SET Username = NULL;

END

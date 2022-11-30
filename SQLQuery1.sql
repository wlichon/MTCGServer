USE [MTCG_DB]
GO

DECLARE	@return_value Int

EXEC	@return_value = [dbo].[reset_card_owners]

SELECT	@return_value as 'Return Value'

GO

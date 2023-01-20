CREATE TABLE [dbo].[MTCGDeals]
(
	[Id] NVARCHAR(50) NOT NULL PRIMARY KEY, 
    [CardToTrade] NVARCHAR(50) NOT NULL, 
    [isSpell] BIT NOT NULL, 
    [MinDmg] INT NOT NULL, 
    CONSTRAINT [FK_Deal_ToCardID] FOREIGN KEY ([CardToTrade]) REFERENCES [MTCGCard]([CardID])
)

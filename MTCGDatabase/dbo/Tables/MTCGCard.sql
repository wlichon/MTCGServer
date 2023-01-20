CREATE TABLE [dbo].[MTCGCard]
(
	[CardID] NVARCHAR(50) NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Damage] INT NOT NULL, 
    [Username] NVARCHAR(50), 
    [Element] NVARCHAR(50) NOT NULL, 
    [isSpell] BIT NOT NULL, 
    [CardNumber] INT NOT NULL IDENTITY, 
    [isTraded] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_Card_ToUser] FOREIGN KEY ([Username]) REFERENCES [MTCGUser]([Username]) ON DELETE CASCADE
)

GO

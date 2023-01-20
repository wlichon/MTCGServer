CREATE TABLE [dbo].[MTCGUser]
(
    [Username] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [Password] NVARCHAR(50) NOT NULL, 
    [Coins] INT NOT NULL DEFAULT 20, 
    [FirstCard] NVARCHAR(50) NULL, 
    [SecondCard] NVARCHAR(50) NULL, 
    [ThirdCard] NVARCHAR(50) NULL, 
    [FourthCard] NVARCHAR(50) NULL, 
    [Rating] INT NOT NULL DEFAULT 1500, 
    [Bio] NVARCHAR(50) NULL, 
    [Image] NVARCHAR(50) NULL, 
    CONSTRAINT [FK_FirstCard_ToCards] FOREIGN KEY ([FirstCard]) REFERENCES [MTCGCard]([CardID]), 
    CONSTRAINT [FK_SecondCard_ToCards] FOREIGN KEY ([SecondCard]) REFERENCES [MTCGCard]([CardID]), 
    CONSTRAINT [FK_ThirdCard_ToCards] FOREIGN KEY ([ThirdCard]) REFERENCES [MTCGCard]([CardID]), 
    CONSTRAINT [FK_FourthCard_ToCards] FOREIGN KEY ([FourthCard]) REFERENCES [MTCGCard]([CardID])
)

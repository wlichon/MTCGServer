using Autofac.Extras.Moq;
using NUnit.Framework;
using MTCGServer.Backend.Database;
using Newtonsoft.Json;
using MTCGServer.Backend.Models;
using MTCGServer.Network;
using static MTCGServer.Backend.Models.Card;
using System.Collections.Generic;
using Moq;
using System;
using System.Linq;

namespace MTCGServer.UnitTests
{

    // NAMING CONVENTION: Function_Scenario_ExpectedBehaviour
    // Arrange
    // Act
    // Assert

    [TestFixture]
    public class BattleTests
    {
        [Test]

        public void DecideWinner_UndefinedCards_ReturnsNegativeOne()
        {
            // Arrange
            var lob = new Lobby();
            
            var card1 = new Card();
            var card2 = new Card();

            // Act
            var ex = Assert.Throws<Exception>(() =>
            {
                var result = lob.DecideWinner(card1, card2);

            });


            // Assert
            Assert.AreEqual(ex.Message, "Invalid Card");

        }


        [Test]
        public void DecideWinner_OrkVsOrk_ReturnsOne()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "321", Name = "Ork", Damage = 30, Element = "Normal"};
            var card2 = new Card { Id = "123", Name = "Ork", Damage = 20, Element = "Normal" };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 1);

        }

        [Test]
        public void DecideWinner_OrkVsOrk_ReturnsTwo()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "123", Name = "Ork", Damage = 20, Element = "Normal" };
            var card2 = new Card { Id = "321", Name = "Ork", Damage = 30, Element = "Normal" };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 2);

        }

        [Test]
        public void DecideWinner_OrkVsOrk_ReturnsZero()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "123", Name = "Ork", Damage = 20, Element = "Normal" };
            var card2 = new Card { Id = "321", Name = "Ork", Damage = 20, Element = "Normal" };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 0);

        }

        [Test]
        public void DecideWinner_RegularSpellVsFireElf_ReturnsTwo()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "123", Name = "RegularSpell", Damage = 100, Element = "Normal", isSpell = true };
            var card2 = new Card { Id = "321", Name = "FireElf", Damage = 80, Element = "Fire" };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 2);

        }

        [Test]
        public void DecideWinner_FireSpellVsOrk_ReturnsOne()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "123", Name = "FireSpell", Damage = 40, Element = "Fire", isSpell = true };
            var card2 = new Card { Id = "321", Name = "Ork", Damage = 90, Element = "Normal" };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 1);

        }

        [Test]
        public void DecideWinner_FireSpellVsOrk_ReturnsZero()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "123", Name = "FireSpell", Damage = 20, Element = "Fire", isSpell = true };
            var card2 = new Card { Id = "321", Name = "Ork", Damage = 80, Element = "Normal" };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 0);

        }

        [Test]
        public void DecideWinner_WaterSpellVsFireDragon_ReturnsZero()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "123", Name = "WaterSpell", Damage = 20, Element = "Water", isSpell = true };
            var card2 = new Card { Id = "321", Name = "Dragon", Damage = 80, Element = "Fire" };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 0);

        }

        [Test]
        public void DecideWinner_WaterSpellVsFireDragon_ReturnsOne()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "123", Name = "WaterSpell", Damage = 21, Element = "Water", isSpell = true };
            var card2 = new Card { Id = "321", Name = "Dragon", Damage = 80, Element = "Fire" };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 1);

        }



    }

    [TestFixture]

    public class BattleTestsSpecialties
    {
        [Test]

        public void DecideWinner_GoblinVsDragon_ReturnsTwo()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "123", Name = "WaterGoblin", Damage = 60, Element = "Water"};
            var card2 = new Card { Id = "321", Name = "Dragon", Damage = 35, Element = "Fire" };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 2);
        }

        [Test]

        public void DecideWinner_DragonVsGoblin_ReturnsOne()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "321", Name = "Dragon", Damage = 35, Element = "Fire" };
            var card2 = new Card { Id = "123", Name = "WaterGoblin", Damage = 60, Element = "Water" };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 1);
        }

        [Test]

        public void DecideWinner_DragonVsFireElf_ReturnsTwo()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "321", Name = "Dragon", Damage = 120, Element = "Fire" };
            var card2 = new Card { Id = "123", Name = "FireElf", Damage = 60, Element = "Fire" };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 2);
        }

        [Test]
        public void DecideWinner_KrakenVsSpell_ReturnsOne()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "321", Name = "Kraken", Damage = 70, Element = "Water" };
            var card2 = new Card { Id = "123", Name = "RegularSpell", Damage = 120, Element = "Normal", isSpell = true };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 1);
        }

        [Test]
        public void DecideWinner_KnightVsWaterSpell_ReturnsTwo()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "321", Name = "Knight", Damage = 90, Element = "Normal" };
            var card2 = new Card { Id = "123", Name = "WaterSpell", Damage = 40, Element = "Water" , isSpell = true};

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 2);
        }

        [Test]
        public void DecideWinner_WizzardVsOrk_ReturnsOne()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "321", Name = "Wizzard", Damage = 50, Element = "Normal" };
            var card2 = new Card { Id = "123", Name = "Ork", Damage = 80, Element = "Normal" };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 1);
        }

        [Test]
        public void DecideWinner_FireElfVsDragon_ReturnsOne()
        {
            // Arrange
            var lob = new Lobby();

            var card1 = new Card { Id = "321", Name = "FireElf", Damage = 30, Element = "Fire" };
            var card2 = new Card { Id = "123", Name = "WaterDragon", Damage = 150, Element = "Water" };

            // Act
            var result = lob.DecideWinner(card1, card2);

            // Assert
            Assert.AreEqual(result, 1);
        }

    }

    [TestFixture]

    public class BattleCardTransferTest
    {

        //private Mock<List<Card>> mockDeck1;
        //private Mock<List<Card>> mockDeck2;

        private List<Card> Deck1;
        private List<Card> Deck2;

        [SetUp]
        public void TestInitialize()
        {
            /*
            var mockDeck1 = new Mock<List<Card>>();
            var mockDeck2 = new Mock<List<Card>>();

            mockDeck1.Setup(Deck1 => Deck1[0]).Returns(new Card { Id = "321", Name = "FireElf", Damage = 30, Element = "Fire" });
            mockDeck1.Setup(Deck1 => Deck1[1]).Returns(new Card { Id = "123", Name = "WaterDragon", Damage = 150, Element = "Water" });
            mockDeck1.Setup(Deck1 => Deck1[2]).Returns(new Card { Id = "124", Name = "FireDragon", Damage = 120, Element = "Fire" });
            mockDeck1.Setup(Deck1 => Deck1[3]).Returns(new Card { Id = "125", Name = "RegularSpell", Damage = 110, Element = "Normal" });

            mockDeck2.Setup(Deck1 => Deck1[0]).Returns(new Card { Id = "234", Name = "Ork", Damage = 60, Element = "Normal" });
            mockDeck2.Setup(Deck1 => Deck1[1]).Returns(new Card { Id = "421", Name = "Wizzard", Damage = 80, Element = "Normal" });
            mockDeck2.Setup(Deck1 => Deck1[2]).Returns(new Card { Id = "422", Name = "Knight", Damage = 70, Element = "Normal" });
            mockDeck2.Setup(Deck1 => Deck1[3]).Returns(new Card { Id = "423", Name = "Kraken", Damage = 170, Element = "Water" });

            mockDeck1.Setup(Deck1 => Deck1.Count).Returns(4);
            mockDeck2.Setup(Deck2 => Deck2.Count).Returns(4);
            */
            Deck1 = new List<Card>()
            {
                new Card { Id = "321", Name = "FireElf", Damage = 30, Element = "Fire" },
                new Card { Id = "123", Name = "WaterDragon", Damage = 170, Element = "Water" },
                new Card { Id = "124", Name = "FireDragon", Damage = 120, Element = "Fire" },
                new Card { Id = "422", Name = "Knight", Damage = 90, Element = "Normal" }
                
            };

            Deck2 = new List<Card>()
            {
                new Card { Id = "234", Name = "Ork", Damage = 60, Element = "Normal" },
                new Card { Id = "421", Name = "Wizzard", Damage = 80, Element = "Normal" },
                new Card { Id = "125", Name = "WaterSpell", Damage = 60, Element = "Water" },
                new Card { Id = "423", Name = "Kraken", Damage = 170, Element = "Water" }
            };

    } 


        [Test]

        public void IterateBattle_FireDragonVsOrk_Deck1ContainsOrk()
        {
            // Arrange
            var lob = new Lobby();


            var mockRnd = new Mock<Random>();

            mockRnd.SetupSequence(rnd => rnd.Next(4))
                .Returns(2) //first card index, FireDragon
                .Returns(0); //second card index, Ork
            


            // Act
            lob.IterateBattle(Deck1,Deck2,mockRnd.Object);

            // Assert
            Assert.That(Deck1.Any(p => p.Id == "234")); //checks if Ork is now in Deck1 since it lost the battle
        }

        [Test]

        public void IterateBattle_WaterDragonVsKraken_NoChangeSinceDraw()
        {
            var lob = new Lobby();


            var mockRnd = new Mock<Random>();

            mockRnd.SetupSequence(rnd => rnd.Next(4))
                .Returns(1) //first card index, WaterDragon
                .Returns(3); //second card index, Kraken



            // Act
            lob.IterateBattle(Deck1, Deck2, mockRnd.Object);

            // Assert
            Assert.AreEqual(Deck1.Count,Deck2.Count);
        }

        [Test]

        public void IterateBattle_WaterSpellVsKnight_Deck2ContainsKnight()
        {
            var lob = new Lobby();


            var mockRnd = new Mock<Random>();

            mockRnd.SetupSequence(rnd => rnd.Next(4))
                .Returns(3) 
                .Returns(2);



            // Act
            lob.IterateBattle(Deck1, Deck2, mockRnd.Object);

            // Assert
            Assert.That(Deck2.Any(p => p.Id == "422")); 
        }


    }

    [TestFixture]

    public class BattleHistoryLogTest
    { 

     
        [Test]

        public void DecideWinner_OrkvsOrk_ReturnsCorrectLog()
        {
            var lob = new Lobby();
            lob.player1 = "Jane";
            lob.player2 = "John";
            var card1 = new Card { Id = "234", Name = "Ork", Damage = 60, Element = "Normal" };
            var card2 = new Card { Id = "123", Name = "Ork", Damage = 70, Element = "Normal" };
            String expected = "Jane: Ork (60 Damage) vs John: Ork (70 Damage) => Ork wins";

            lob.DecideWinner(card1, card2);

            String actual = lob.history.logs[0];


            Assert.Multiple(() =>
            {
                Assert.That(lob.history.logs.Count == 1);
                Assert.AreEqual(expected, actual);
            });

        }

        [Test]

        public void DecideWinner_FireDragonVsWaterSpell_ReturnsCorrectLog()
        {
            var lob = new Lobby();
            lob.player1 = "Jane";
            lob.player2 = "John";
            var card1 = new Card { Id = "234", Name = "FireDragon", Damage = 110, Element = "Fire" };
            var card2 = new Card { Id = "123", Name = "WaterSpell", Damage = 40, Element = "Water" , isSpell = true};
            String expected = "Jane: FireDragon (110 Damage) vs John: WaterSpell (40 Damage) => 110 VS 40 -> 55 VS 80 => WaterSpell wins";

            lob.DecideWinner(card1, card2);

            String actual = lob.history.logs[0];


            Assert.Multiple(() =>
            {
                Assert.That(lob.history.logs.Count == 1);
                Assert.AreEqual(expected, actual);
            });
        }

        public void DecideWinner_FireSpellVsWaterSpell_ReturnsCorrectLog()
        {
            var lob = new Lobby();
            lob.player1 = "Jane";
            lob.player2 = "John";
            var card1 = new Card { Id = "234", Name = "FireSpell", Damage = 160, Element = "Fire", isSpell = true };
            var card2 = new Card { Id = "123", Name = "WaterSpell", Damage = 40, Element = "Water", isSpell = true };
            String expected = "Jane: FireSpell (160 Damage) vs John: WaterSpell (40 Damage) => 160 VS 40 -> 80 VS 80 => Draw (no action)";

            lob.DecideWinner(card1, card2);

            String actual = lob.history.logs[0];


            Assert.Multiple(() =>
            {
                Assert.That(lob.history.logs.Count == 1);
                Assert.AreEqual(expected, actual);
            });
        }

        [Test]
        public void DecideWinner_KrakenVsSpell_ReturnsCorrectLog()
        {
            var lob = new Lobby();
            lob.player1 = "Jane";
            lob.player2 = "John";
            var card1 = new Card { Id = "234", Name = "Kraken", Damage = 160, Element = "Water"};
            var card2 = new Card { Id = "123", Name = "RegularSpell", Damage = 500, Element = "Water", isSpell = true };
            String expected = "Jane: Kraken (160 Damage) vs John: RegularSpell (500 Damage) => The Kraken is immune against spells => Kraken wins";

            lob.DecideWinner(card1, card2);

            String actual = lob.history.logs[0];


            Assert.Multiple(() =>
            {
                Assert.That(lob.history.logs.Count == 1);
                Assert.AreEqual(expected, actual);
            });
        }

        [Test]
        public void DecideWinner_MultipleLogs_ReturnsCorrectLogs()
        {
            var lob = new Lobby();
            lob.player1 = "Jane";
            lob.player2 = "John";
            var card1 = new Card { Id = "734", Name = "FireSpell", Damage = 160, Element = "Fire", isSpell = true };
            var card2 = new Card { Id = "123", Name = "WaterSpell", Damage = 40, Element = "Water", isSpell = true };
            var card3 = new Card { Id = "235", Name = "Knight", Damage = 70, Element = "Normal" };
            var card4 = new Card { Id = "126", Name = "WaterSpell", Damage = 40, Element = "Water", isSpell = true };
            var card5 = new Card { Id = "476", Name = "Dragon", Damage = 150, Element = "Normal"};
            var card6 = new Card { Id = "963", Name = "FireElf", Damage = 30, Element = "Fire" };
            
            String expected1 = "Jane: FireSpell (160 Damage) vs John: WaterSpell (40 Damage) => 160 VS 40 -> 80 VS 80 => Draw (no action)";
            String expected2 = "Jane: Knight (70 Damage) vs John: WaterSpell (40 Damage) => The Knights armor is too heavy, causing him to drown => WaterSpell wins";
            String expected3 = "Jane: Dragon (150 Damage) vs John: FireElf (30 Damage) => Elves know Dragons since they were little and can evade their attacks => Elf wins";

            lob.DecideWinner(card1, card2);

            String actual1 = lob.history.logs[0];

            lob.DecideWinner(card3, card4);

            String actual2 = lob.history.logs[1];

            lob.DecideWinner(card5, card6);

            String actual3 = lob.history.logs[2];


            Assert.Multiple(() =>
            {
                Assert.That(lob.history.logs.Count == 3);
                Assert.AreEqual(expected1, actual1);
                Assert.AreEqual(expected2, actual2);
                Assert.AreEqual(expected3, actual3);
            });
        }
    }
}
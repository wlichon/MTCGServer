using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sc = System.StringComparison;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MTCGServer.Backend.Models
{
   

    public struct Value
    {
        public String name { get; set; }
        public int outcome { get; set; }

        public Card.logComment comment { get; set; }

        public Value(String name, int outcome, Card.logComment logComment)
        {
            this.name = name;
            this.outcome = outcome;
            this.comment = logComment;
        }

    }

    public class ShowDeck
    {
        public string Token { get; set; }

        public bool differentRepresentation { get; set; } = false;
    }

    public class EditUserData
    {
        public string Token { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
    }

    public class TradeDeal
    {
        public string Token { get; set; }
        public string Id { get; set; }
        public string CardToTrade { get; set; }
        public int isSpell { get; set; }
        public int MinDmg { get; set; }
    }

    public class AcceptTrade
    {
        public string Token { get; set; }

        public string TradeId { get; set; }

        public string CardToTrade { get; set; }
    }

    public class DeleteTrade
    {
        public string Token { get; set; }

        public string TradeId { get; set; }

    }


    public class ScoreBoard
    {
        public string Username { get; set; }
        public int Rating { get; set; }
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Card
    {

        public enum logComment
        {
            GoblinVsDragon,
            DragonVsElf,
            WizzardVsOrk,
            KnightVsWaterSpell,
            KrakenVsSpell
        }

        public static List<String> logComments = new List<String>()
        {
            "Goblins are too afraid of Dragons to attack => Dragon wins",
            "Elves know Dragons since they were little and can evade their attacks => Elf wins",
            "Wizzard controls the Orks mind => Wizzard wins",
            "The Knights armor is too heavy, causing him to drown => WaterSpell wins",
            "The Kraken is immune against spells => Kraken wins"
        };

        public static Dictionary<String, List<Value>> specialties = new Dictionary<String, List<Value>>()
        {
            {"goblin", new List<Value>{new Value("dragon",2, logComment.GoblinVsDragon) } },
            {"dragon", new List<Value>{new Value("goblin",1,logComment.GoblinVsDragon), new Value("fireelf", 2,logComment.DragonVsElf) } },
            {"wizzard", new List<Value>{new Value("ork",1,logComment.WizzardVsOrk) } },
            {"orks", new List<Value>{new Value("wizzard",2,logComment.WizzardVsOrk) } },
            {"knight", new List<Value>{new Value("waterspell",2,logComment.KnightVsWaterSpell) } },
            {"waterspell", new List<Value>{new Value("knight",1,logComment.KnightVsWaterSpell) } },
            {"kraken", new List<Value>{new Value("spell",1,logComment.KrakenVsSpell) } },
            {"spell", new List<Value>{new Value("kraken",2,logComment.KrakenVsSpell) } },
            {"fireelf", new List<Value>{new Value("dragon",1,logComment.DragonVsElf) } }

        };
        public string Id { get; set; }
        public string Name { get; set; }
        public double Damage { get; set; }
        public string Element { get; set; }

        public bool isSpell { get; set; }

        public bool isValid()
        {
            if (Id == null || Name == null || Damage == 0 || Element == null)
            {
                return false;
            }
            return true;
        }

        

        public int SpecialtyDecider(Card opponent, ref String logComment)
        {
            
            String patterns = @"[Gg]oblin|[Dd]ragon|[Kk]night|[Ww]ater[Ss]pell|[Ss]pell|[Ww]izzard|[Oo]rk|[Kk]raken|[Ff]ire[Ee]lf";


            Match match1 = Regex.Match(Name, patterns);
            Match match2 = Regex.Match(opponent.Name, patterns);

            String name1 = match1.Value.ToLower();
            String name2 = match2.Value.ToLower();


            


            if(specialties.ContainsKey(name1) && specialties[name1].Any(p => p.name == name2))
            {
                int index = specialties[name1].FindIndex(p => p.name == name2);
                logComment = logComments[(int)specialties[name1][index].comment];
                return specialties[name1][index].outcome;
            }

            return 0;
            

            
        }

        public double ElementDamageScaler(Card opponent)
        {
            if (Element == "Normal")
            {
                if (opponent.Element == "Normal")
                {
                    return 1;
                }
                if (opponent.Element == "Fire")
                {
                    return 0.5;
                }
                if (opponent.Element == "Water")
                {
                    return 2;

                }
            }

            if (Element == "Fire")
            {
                if (opponent.Element == "Normal")
                {
                    return 2;
                }
                if (opponent.Element == "Fire")
                {
                    return 1;
                }
                if (opponent.Element == "Water")
                {
                    return 0.5;
                }
            }

            if (Element == "Water")
            {
                if (opponent.Element == "Normal")
                {
                    return 0.5;
                }
                if (opponent.Element == "Fire")
                {
                    return 2;
                }
                if (opponent.Element == "Water")
                    return 1;
            }

            throw new Exception("Invalid type");
        }

    }

    public class AuthToken
    {
        public string Token { get; set; }
    }

    public class DeckConfig
    {
        public List<String> Cards { get; set; }

        public string Token { get; set; }
    }



    public class BattleHistory
    {
        public List<String> logs = new List<String>();
    }

    public class Lobby
    {
        public BattleHistory history = new BattleHistory();
        public Dictionary<String, List<Card>> PlayersLookingForGame { get; set; }
        public bool BattleCompleted { get; set; }
        public int LobbyID { get; set; }
        public String player1 { get; set; }
        public String player2 { get; set; }

        public int player1Rating { get; set; }

        public int player2Rating { get; set; }
        
        
        public int DecideWinner(Card card1, Card card2) // -2 unknown error, -1 if invalid card members, 0 if draw, 1 if card1 is stronger, 2 if card2 is stronger
        {
            int outcome;
            String logComment = "";

            List<String> outcomes = new List<String>()
            {
                "Draw (no action)",
                $"{card1.Name} wins",
                $"{card2.Name} wins"
            };

            double card1Damage = card1.Damage;
            double card2Damage = card2.Damage;

            

            if (!card1.isValid() || !card2.isValid())
            {
                throw new Exception("Invalid Card");
            }



            if ((outcome = card1.SpecialtyDecider(card2, ref logComment)) != 0)
            {
                
            }


            else if (card1.isSpell || card2.isSpell) // XOR, only true if one or two of the cards are a spell
            {
                

                double scaledCard1Damage = card1.Damage * card1.ElementDamageScaler(card2);
                double scaledCard2Damage = card2.Damage * card2.ElementDamageScaler(card1);

                if (scaledCard1Damage > scaledCard2Damage)
                {
                    outcome = 1;
                }
                else if (scaledCard1Damage < scaledCard2Damage)
                {
                    outcome = 2;
                }
                else
                {
                    outcome = 0;
                }


                logComment = $"{card1Damage} VS {card2Damage} -> {scaledCard1Damage} VS {scaledCard2Damage} => {outcomes[outcome]}";

            }

            else // fight between monster and monster
            {

                if (card1Damage > card2Damage)
                {

                    outcome = 1;
                }
                else if (card1Damage < card2Damage)
                {
                    outcome = 2;
                }
                else
                {

                    outcome = 0;
                }
                logComment = $"{outcomes[outcome]}";
            }

            String logPattern = $"{player1}: {card1.Name} ({card1Damage} Damage) vs {player2}: {card2.Name} ({card2Damage} Damage) => {logComment}";
            this.history.logs.Add(logPattern);
            return outcome;

        }
        public int StartBattle(ref int player1NewRating, ref int player2NewRating) {
            List<Card> Deck1 = PlayersLookingForGame[player1];
            List<Card> Deck2 = PlayersLookingForGame[player2];

            Random rnd = new Random();

            int round = 1;

            while(Deck1.Count != 0 && Deck2.Count != 0 && round < 100)
            {
                IterateBattle(Deck1, Deck2, rnd);
                
            }

            double player1Score;
            double player2Score;
            

            if(Deck1.Count == 0)
            {
                player1Score = 0;
                player2Score = 1;
                this.history.logs.Add($"{player2} Wins!!");
            }
            else if(Deck2.Count == 0)
            {
                player1Score = 1;
                player2Score = 0;
                this.history.logs.Add($"{player1} Wins!!");
            }
            else
            {
                player1Score = 0.5;
                player2Score = 0.5;
                this.history.logs.Add($"It's a draw due to 100 rounds being played!");
            }

            EloCalculation(this.player1Rating, this.player2Rating, player1Score, player2Score,ref player1NewRating,ref player2NewRating);
            this.history.logs.Add($"{player1} Rating: {player1Rating} -> {player1NewRating} | {player2} Rating: {player2Rating} -> {player2NewRating}");

            return 0;
        }

        public static void EloCalculation(int player1Rating, int player2Rating, double player1Score, double player2Score, ref int player1NewRating, ref int player2NewRating)
        {
            const int k = 32;

            double player1Expected = 1 / (1 + Math.Pow(10, (player2Rating - player1Rating) / 400.0));
            double player2Expected = 1 / (1 + Math.Pow(10, (player1Rating - player2Rating) / 400.0));

            player1NewRating = player1Rating + (int)(k * (player1Score - player1Expected));
            player2NewRating = player2Rating + (int)(k * (player2Score - player2Expected));
        }


        public void IterateBattle(List<Card> Deck1, List<Card> Deck2, Random rnd)
        {
            int randomCardIndexP1 = rnd.Next(Deck1.Count);
            int randomCardIndexP2 = rnd.Next(Deck2.Count);

            Card Card1 = Deck1[randomCardIndexP1];
            Card Card2 = Deck2[randomCardIndexP2];


            int result = DecideWinner(Card1, Card2);

            switch (result)
            {
                case 0: //nothing changes with a draw
                    break;
                case 1: //card 1 wins
                    Deck2.RemoveAt(randomCardIndexP2);
                    Deck1.Add(Card2);
                    break;
                case 2: //card2 wins
                    Deck1.RemoveAt(randomCardIndexP1);
                    Deck2.Add(Card1);
                    break;
                default:
                    throw new Exception("Unexpected return value of DecideWinner function");

            }
        }

        public String ReturnHistoryAsString()
        {
            String history = "";
            foreach(var log in this.history.logs)
            {
                history += log + "\n";
            }

            return history;
        }

        public void DoBattle(ref int player1NewRating, ref int player2NewRating)
        {
            if (BattleCompleted)
            {
                PlayersLookingForGame.Remove(player1);
                PlayersLookingForGame.Remove(player2);
                return;

            }

            StartBattle(ref player1NewRating, ref player2NewRating);


            BattleCompleted = true;
        }
        public Lobby()
        {
            this.player1 = "";
            this.player2 = "";
            this.player1Rating = 0;
            this.player2Rating = 0;
            this.LobbyID = -1;
            this.BattleCompleted = false;
            this.PlayersLookingForGame = null;
        }
        
        public void FillLobby(String newPlayer)
        {
            if(player1.Length == 0) {
                player1 = newPlayer;
                
            }
            else if(player2.Length == 0)
            {
                player2 = newPlayer;
            }
            else
            {
                throw new InvalidOperationException("Cant add new player to lobby since it's full");

            }

        }
        public int PlayersInLobby()
        {
            if(player1.Length == 0)
            {
                return 0;
            }
            if(player2.Length == 0)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        public String GetOpponentFromLobby(String currentPlayer)
        {
            if(player1 != currentPlayer)
            {
                return player1;
            }
            else
            {
                return player2;
            }
        }



    }
}

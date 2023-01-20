using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using MTCGServer.Backend.Models;
using System.Data;
using MTCGServer.Network;
using System.Data.SqlClient;

namespace MTCGServer.Backend.Database
{
    public class Database
    {

        private static Mutex mut = new Mutex();

        private List<string> sessionTokens;
        private string connectionString;
        private Dictionary<String, List<Card>> PlayersLookingForGame;
        private List<Lobby> lobbies;
        private int lobbyID;
        public enum SQLFunction
        {
            LoginUser,
            RegisterUser,
            CreatePackage,
            AcquirePackage,
            ShowCardCollection,
            ShowDeck,
            ConfigureDeck,
            Battle,
            ShowScoreboard,
            ShowStats,
            ShowTrades,
            CreateTrade,
            AcceptTrade,
            DeleteTrade,
            EditUserData,
            ShowUserData
        }


        public Database(string connectionString)
        {
            this.connectionString = connectionString;
            this.sessionTokens = new List<string>();
            this.PlayersLookingForGame = new Dictionary<string, List<Card>>();
            this.lobbies = new List<Lobby>();
            this.lobbyID = 0;
        }

        private void LoginUser(object obj, SqlCommand cmd, HttpRes res)
        {
            User UserObj = (User)obj;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = UserObj.Username;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = UserObj.Password;

            DataSet ds = new DataSet();
            cmd.ExecuteNonQuery();
            var dAdap = new SqlDataAdapter();
            dAdap.SelectCommand = cmd;
            dAdap.Fill(ds, "result");
            try
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //string? usernameInDB = ds.Tables[0].Rows[0]["Username"].ToString();
                    //string? passwordInDB = ds.Tables[0].Rows[0]["Password"].ToString();


                    res.Code = Code.OK;
                    res.Status = "User logged in\n";
                    this.sessionTokens.Add(UserObj.Username);

                }
                else
                {
                    res.Code = Code.BAD_REQUEST;
                    res.Status = "Login failed\n";
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {
                    default:
                        res.Status = "Something went wrong\n";
                        break;
                }

            }
        }

        private void RegisterUser(object obj, SqlCommand cmd, HttpRes res)
        {
            User UserObj = (User)obj;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = UserObj.Username;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = UserObj.Password;
            try
            {

                cmd.ExecuteNonQuery();


                res.Code = Code.OK;
                res.Status = "User registered\n";


            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {

                    case 2627:
                        res.Status = "Username already exists\n";
                        res.Code = Code.BAD_REQUEST;
                        break;
                    default:
                        res.Status = "Something went wrong\n";
                        res.Code = Code.BAD_REQUEST;
                        break;
                }
            }
        }

        private void CreatePackage(object ObjCards, SqlCommand cmd, HttpRes res)
        {
            int cardNumber = 1;
            bool err = false;
            cmd.Parameters.Add("@CardID", SqlDbType.NVarChar);
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar);
            cmd.Parameters.Add("@Damage", SqlDbType.NVarChar);
            cmd.Parameters.Add("@Element", SqlDbType.NVarChar);
            cmd.Parameters.Add("@isSpell", SqlDbType.Bit);
            foreach (Card card in (List<Card>)ObjCards)
            {
                cmd.Parameters["@CardID"].Value = card.Id;
                cmd.Parameters["@Name"].Value = card.Name;
                cmd.Parameters["@Damage"].Value = card.Damage;
                cmd.Parameters["@Element"].Value = card.Element;
                cmd.Parameters["@isSpell"].Value = card.isSpell;
                //string sql = "insert into dbo.MTCGCard (CardID, Name, Damage, Username) values(@CardID, @Name, @Damage, @Username)";
                try
                { 
                    cmd.ExecuteNonQuery();

                    cardNumber++;
                }
                catch(SqlException ex)
                {
                    Console.WriteLine($"Card Number {0} didnt get added\n" + ex.Message, cardNumber);
                    err = true;
                }

            }

            if (err)
            {
                res.Code = Code.BAD_REQUEST;
                res.Status = "Not all of the cards were added due to an error\n";
            }
            else
            {
                res.Code = Code.OK;
                res.Status = "Package(s) created by admin\n";
            }
        }

        private void AcquirePackage(object obj, SqlCommand sp, HttpRes res)
        {
            AuthToken TokenObj = (AuthToken)obj;
            sp.CommandType = CommandType.StoredProcedure;

            sp.Parameters.Add(new SqlParameter("@Username", TokenObj.Token));
            try
            {
                if (!sessionTokens.Contains(TokenObj.Token))
                {
                    throw new Exception("User session not active");
                }

                sp.ExecuteNonQuery();


                res.Code = Code.OK;
                res.Status = "Package acquired\n";


            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {
                    case 547:
                        res.Status = "User doesnt exist\n";
                        res.Code = Code.BAD_REQUEST;
                        break;
                    case 50000:
                        res.Status = ex.Errors[0].Message;
                        res.Code = Code.FORBIDDEN;
                        break;
                    default:
                        res.Status = "Something went wrong\n";
                        res.Code = Code.BAD_REQUEST;
                        break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.Status = ex.Message;
                res.Code = Code.BAD_REQUEST;
            }
        }

        private void ShowCardCollection(object obj, SqlCommand cmd, HttpRes res)
        {
            AuthToken AuthObj = (AuthToken)obj;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = AuthObj.Token;

            DataSet ds = new DataSet();
            cmd.ExecuteNonQuery();
            var dAdap = new SqlDataAdapter();
            dAdap.SelectCommand = cmd;
            dAdap.Fill(ds, "result");

            string cards = "Collection: \n";

            //DataTable Cards = new DataTable("Cards");
       
            try
            {
                if (!sessionTokens.Contains(AuthObj.Token))
                {
                    throw new Exception("User session not active");
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Console.WriteLine(row["Name"] + ", " + row["Damage"]);
                        cards += $"Id = {row["CardID"].ToString()}, Name = {row["Name"].ToString()}, Damage = {Convert.ToDouble(row["Damage"].ToString())}, Element = {row["Element"].ToString()}, isSpell = {(bool)row["isSpell"]} \n";
                        
                            /*
                            public class Card
                                {
                                    public string Id { get; set; }
                                    public string Name { get; set; }
                                    public double Damage { get; set; }
                                }
                            */
                    }
                    //string? usernameInDB = ds.Tables[0].Rows[0]["Username"].ToString();
                    //string? passwordInDB = ds.Tables[0].Rows[0]["Password"].ToString();

                   
                    res.Code = Code.OK;
                    res.Status = cards;

                }
                else
                {
                    res.Code = Code.BAD_REQUEST;
                    res.Status = "Card Collection Selection failed\n";
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {
                    default:
                        res.Status = "Something went wrong\n";
                        break;
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.Status = ex.Message;
                res.Code = Code.BAD_REQUEST;
            }
        }

        private void ConfigureDeck(object obj, SqlCommand sp, HttpRes res)
        {
            DeckConfig DeckObj = (DeckConfig)obj;
            sp.CommandType = CommandType.StoredProcedure;

            try
            {
            sp.Parameters.Add(new SqlParameter("@Username", DeckObj.Token));

            if(DeckObj.Cards.Count != 4)
            {
                    throw new Exception("Deck needs to have 4 cards");
            }

            sp.Parameters.Add(new SqlParameter("@FirstCard", DeckObj.Cards[0]));
            sp.Parameters.Add(new SqlParameter("@SecondCard", DeckObj.Cards[1]));
            sp.Parameters.Add(new SqlParameter("@ThirdCard", DeckObj.Cards[2]));
            sp.Parameters.Add(new SqlParameter("@FourthCard", DeckObj.Cards[3]));
            
            if (!sessionTokens.Contains(DeckObj.Token))
            {
                throw new Exception("User session not active");
            }

            sp.ExecuteNonQuery();


            res.Code = Code.OK;
            res.Status = "Deck Configured\n";


            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {
                    case 547:
                        res.Status = "User doesnt exist\n";
                        res.Code = Code.BAD_REQUEST;
                        break;
                    default:
                        res.Status = "Something went wrong\n";
                        res.Code = Code.BAD_REQUEST;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.Status = ex.Message;
                res.Code = Code.BAD_REQUEST;
            }
        }

        private void ShowDeck(object obj, SqlCommand cmd, HttpRes res)
        {
            ShowDeck ShowDeckObj = (ShowDeck)obj;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = ShowDeckObj.Token;

            DataSet ds = new DataSet();
            cmd.ExecuteNonQuery();
            var dAdap = new SqlDataAdapter();
            dAdap.SelectCommand = cmd;
            dAdap.Fill(ds, "result");

            String Deck = "Deck: \n";


            try
            {
                if (!sessionTokens.Contains(ShowDeckObj.Token))
                {
                    throw new Exception("User session not active");
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        if (ShowDeckObj.differentRepresentation)
                        {
                            Deck += row["CardID"].ToString() + " || " + row["Name"].ToString() + " || " + row["Damage"].ToString() + " || " + row["Element"].ToString() + " || " +  row["isSpell"].ToString() + "\n";
                        }
                        else
                        { 
                            Deck += row["Name"].ToString() + ", " + row["Damage"].ToString() + ", " + row["Element"].ToString() + "\n";
                        }


                    }

                    res.Code = Code.OK;
                    res.Status = Deck;

                }
                else
                {
                    res.Code = Code.BAD_REQUEST;
                    res.Status = "There are no cards in your deck\n";
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {
                    default:
                        res.Status = ex.Errors[0].Message;
                        break;
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.Status = ex.Message;
                res.Code = Code.BAD_REQUEST;
            }
        }
        private void Battle(object obj, SqlCommand cmd, HttpRes res)
        {
            var TokenObj = (AuthToken)obj;

            String currentPlayer = TokenObj.Token;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = currentPlayer;

            DataSet ds = new DataSet();
            cmd.ExecuteNonQuery();
            var dAdap = new SqlDataAdapter();
            dAdap.SelectCommand = cmd;
            dAdap.Fill(ds, "result");

            var deck = new List<Card>();

            try
            {
                if (!sessionTokens.Contains(currentPlayer))
                {
                    res.Code = Code.BAD_REQUEST;
                    res.Status = $"{currentPlayer} user session not active";

                    throw new Exception($"{currentPlayer} user session not active");
                }
                 
                if (ds.Tables[0].Rows.Count != 4)
                {
                    res.Code = Code.BAD_REQUEST;
                    res.Status = $"{currentPlayer} does not have 4 Cards defined in his deck";

                    throw new Exception($"{currentPlayer} does not have 4 Cards defined in his deck");
                }

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var Card = new Card { Id = row["CardID"].ToString(), Name = row["Name"].ToString(), Damage = Convert.ToDouble(row["Damage"].ToString()), Element = row["Element"].ToString(), isSpell = (bool)row["isSpell"] };
                    deck.Add(Card);
                        
                }

                String sql2 = "SELECT Rating FROM dbo.MTCGUser WHERE Username = @Username2";

                cmd.CommandText = sql2;
                cmd.Parameters.Add("@Username2",SqlDbType.NVarChar).Value = currentPlayer;
                object result = cmd.ExecuteScalar();
                //using (SqlCommand cmd2 = new SqlCommand(sql2, this.connectionString)) ;

                
                if(result == null)
                {
                    throw new Exception("Rating shouldnt be null");
                }

                int rating = Convert.ToInt32(result);
                


                String opponent;
                List<Card> opponentsDeck;
                
                Lobby lob = null;
                bool FoundLobby = false;

                    

                mut.WaitOne();

                PlayersLookingForGame.Add(currentPlayer, deck); // add player name as key and deck as value in dictionary

                foreach (var lobby in lobbies)
                {
                    if(lobby.PlayersInLobby() == 1)
                    {
                        lob = lobby;
                        lob.FillLobby(currentPlayer);

                        lob.player2Rating = rating;
                        lob = lobby;
                        FoundLobby = true;
                        break;
                    }
                }
                if (!FoundLobby)
                {
                    lob = new Lobby { player1 = currentPlayer, LobbyID = lobbyID++, player1Rating = rating };
                    lobbies.Add(lob);

                }
                    

                while(lob.PlayersInLobby() != 2)
                {
                    mut.ReleaseMutex();

                    Console.WriteLine("{0} looking for opponent...", currentPlayer);

                    Thread.Sleep(1000);

                    mut.WaitOne();

                }

                int player1NewRating = 0;
                int player2NewRating = 0;

                lob.PlayersLookingForGame = PlayersLookingForGame;
                lob.DoBattle(ref player1NewRating, ref player2NewRating);

      

                UpdateEloRatingClass.UpdateEloRating(lob.player1, lob.player2, player1NewRating, player2NewRating,cmd);

                mut.ReleaseMutex();

                Console.WriteLine("LobbyID: {0}, Player1: {1}, Player2 {2}", lob.LobbyID, lob.player1, lob.player2);





                res.Code = Code.OK;
                res.Status = lob.ReturnHistoryAsString();

                
                

            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {
                    default:
                        res.Status = "Something went wrong\n";
                        break;
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.Status = ex.Message;
                res.Code = Code.BAD_REQUEST;
            }


        }

        static class UpdateEloRatingClass
        {
            static public bool EloIsUpdated { get; set; }
            public static void UpdateEloRating(String player1, String player2, int player1NewRating, int player2NewRating, SqlCommand cmd)
            {
                if (EloIsUpdated)
                {
                    return;
                }

                
                String sql1 = "UPDATE dbo.MTCGUser SET Rating = @player1NewRating WHERE Username = @player1";
                cmd.CommandText = sql1;
                cmd.Parameters.Add("@player1", SqlDbType.NVarChar).Value = player1;
                cmd.Parameters.Add("@player1NewRating", SqlDbType.Int).Value = player1NewRating;

                cmd.ExecuteNonQuery();

                String sql2 = "UPDATE dbo.MTCGUser SET Rating = @player2NewRating WHERE Username = @player2";
                cmd.CommandText = sql2;
                cmd.Parameters.Add("@player2", SqlDbType.NVarChar).Value = player2;
                cmd.Parameters.Add("@player2NewRating", SqlDbType.Int).Value = player2NewRating;

                cmd.ExecuteNonQuery();



                EloIsUpdated = true;

            }


        }

        private void ShowScoreboard(object obj, SqlCommand cmd, HttpRes res)
        {
            AuthToken AuthObj = (AuthToken)obj;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = AuthObj.Token;

            DataSet ds = new DataSet();
            cmd.ExecuteNonQuery();
            var dAdap = new SqlDataAdapter();
            dAdap.SelectCommand = cmd;
            dAdap.Fill(ds, "result");

            String Scoreboard = "Username || Rating \n";


            try
            {
                if (!sessionTokens.Contains(AuthObj.Token))
                {
                    throw new Exception("User session not active");
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Console.WriteLine(row["Username"] + ", " + row["Rating"]);
                        Scoreboard += row["Username"].ToString() + " || " + row["Rating"].ToString() + "\n";
                        
                       
                    }

                    res.Code = Code.OK;
                    res.Status = Scoreboard;

                }
                else
                {
                    res.Code = Code.BAD_REQUEST;
                    res.Status = "There are no records in the Scoreboard\n";
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {
                    default:
                        res.Status = "Something went wrong\n";
                        break;
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.Status = ex.Message;
                res.Code = Code.BAD_REQUEST;
            }

        }

        private void ShowStats(object obj, SqlCommand cmd, HttpRes res)
        {
            AuthToken AuthObj = (AuthToken)obj;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = AuthObj.Token;

            DataSet ds = new DataSet();
            cmd.ExecuteNonQuery();
            var dAdap = new SqlDataAdapter();
            dAdap.SelectCommand = cmd;
            dAdap.Fill(ds, "result");

            String Stats = null;


            try
            {
                if (!sessionTokens.Contains(AuthObj.Token))
                {
                    throw new Exception("User session not active");
                }

                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    String bio = row["Bio"].ToString().Length == 0 ? "EMPTY" : row["Bio"].ToString();
                    String img = row["Image"].ToString().Length == 0 ? "EMPTY" : row["Image"].ToString();


                    Stats = row["Username"].ToString() + " || " + row["Rating"].ToString() + " || " + bio + " || " + img + "\n";


                    

                    res.Code = Code.OK;
                    res.Status = Stats;

                }
                else
                {
                    res.Code = Code.BAD_REQUEST;
                    res.Status = "Statistics Selection failed\n";
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {
                    default:
                        res.Status = "Something went wrong\n";
                        break;
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.Status = ex.Message;
                res.Code = Code.BAD_REQUEST;
            }

        }

        private void ShowTrades(object obj, SqlCommand cmd, HttpRes res)
        {
            AuthToken AuthObj = (AuthToken)obj;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = AuthObj.Token;

            DataSet ds = new DataSet();
            cmd.ExecuteNonQuery();
            var dAdap = new SqlDataAdapter();
            dAdap.SelectCommand = cmd;
            dAdap.Fill(ds, "result");

            String trades = "Trading Deals: \n";


            try
            {
                if (!sessionTokens.Contains(AuthObj.Token))
                {
                    throw new Exception("User session not active");
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        String type = Convert.ToBoolean(row["isSpell1"]) == false ? "monster" : "spell";


                        trades += $" Trade_ID: {row["Id"].ToString()} {{Offered Card: Name -> {row["Name"].ToString()} | Damage -> {row["Damage"].ToString()} | Element -> {row["Element"].ToString()} | isSpell -> {row["isSpell"].ToString()}}} -- {{Traded Card should have: Type -> {type} | Minimum Damage of -> {row["MinDmg"].ToString()}}} \n";


                    }

                    res.Code = Code.OK;
                    res.Status = trades;

                }
                else
                {
                    res.Code = Code.OK;
                    res.Status = "There are currently no open trades\n";
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {
                    default:
                        res.Status = "Something went wrong\n";
                        break;
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.Status = ex.Message;
                res.Code = Code.BAD_REQUEST;
            }
        }

        private void CreateTrade(object obj, SqlCommand sp, HttpRes res)
        {
            TradeDeal TradeDealsObj = (TradeDeal)obj;
            sp.CommandType = CommandType.StoredProcedure;

            try
            {
                sp.Parameters.Add(new SqlParameter("@Username", TradeDealsObj.Token));

               

                sp.Parameters.Add(new SqlParameter("@Id", TradeDealsObj.Id));
                sp.Parameters.Add(new SqlParameter("@CardToTrade", TradeDealsObj.CardToTrade));
                sp.Parameters.Add(new SqlParameter("@isSpell", TradeDealsObj.isSpell));
                sp.Parameters.Add(new SqlParameter("@MinDmg", TradeDealsObj.MinDmg));

                if (!sessionTokens.Contains(TradeDealsObj.Token))
                {
                    throw new Exception("User session not active");
                }

                sp.ExecuteNonQuery();


                res.Code = Code.OK;
                res.Status = "Trade Deal created\n";


            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {
                    case 50000:
                        res.Status = ex.Errors[0].Message;
                        //res.Status = "Trade Deal ID already exists or provided card is not in users possesion\n";
                        res.Code = Code.FORBIDDEN;
                        break;
                    default:
                        res.Status = "Something went wrong\n";
                        res.Code = Code.BAD_REQUEST;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.Status = ex.Message;
                res.Code = Code.BAD_REQUEST;
            }
        }

        private void AcceptTrade(object obj, SqlCommand sp, HttpRes res)
        {
            AcceptTrade AccepTradeObj = (AcceptTrade)obj;
            sp.CommandType = CommandType.StoredProcedure;

            try
            {
                sp.Parameters.Add(new SqlParameter("@Username", AccepTradeObj.Token));



                sp.Parameters.Add(new SqlParameter("@TradeId", AccepTradeObj.TradeId));
                sp.Parameters.Add(new SqlParameter("@CardToTrade", AccepTradeObj.CardToTrade));
   

                if (!sessionTokens.Contains(AccepTradeObj.Token))
                {
                    throw new Exception("User session not active");
                }

                sp.ExecuteNonQuery();


                res.Code = Code.OK;
                res.Status = "Trade Deal accepted\n";


            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {
                    
                    default:
                        res.Status = ex.Errors[0].Message;
                        res.Code = Code.BAD_REQUEST;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.Status = ex.Message;
                res.Code = Code.BAD_REQUEST;
            }
        }

        private void DeleteTrade(object obj, SqlCommand sp, HttpRes res)
        {
            DeleteTrade DeleteTradeObj = (DeleteTrade)obj;
            sp.CommandType = CommandType.StoredProcedure;

            try
            {
                sp.Parameters.Add(new SqlParameter("@Username", DeleteTradeObj.Token));



                sp.Parameters.Add(new SqlParameter("@TradeId", DeleteTradeObj.TradeId));


                if (!sessionTokens.Contains(DeleteTradeObj.Token))
                {
                    throw new Exception("User session not active");
                }

                sp.ExecuteNonQuery();


                res.Code = Code.OK;
                res.Status = "Trade Deal deleted\n";


            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {

                    default:
                        res.Status = ex.Errors[0].Message;
                        res.Code = Code.BAD_REQUEST;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.Status = ex.Message;
                res.Code = Code.BAD_REQUEST;
            }
        }

        private void ShowUserData(object obj, SqlCommand cmd, HttpRes res)
        {
            AuthToken AuthObj = (AuthToken)obj;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = AuthObj.Token;

            DataSet ds = new DataSet();
            cmd.ExecuteNonQuery();
            var dAdap = new SqlDataAdapter();
            dAdap.SelectCommand = cmd;
            dAdap.Fill(ds, "result");

            String UserData = "User Data: \n";


            try
            {
                if (!sessionTokens.Contains(AuthObj.Token))
                {
                    throw new Exception("User session not active");
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        


                        UserData += "Username: " + row["Username"].ToString() + " ||  Bio:  " + row["Bio"].ToString() + " ||  Image:  " + row["Image"].ToString() + "\n";


                    }

                    res.Code = Code.OK;
                    res.Status = UserData;

                }
                else
                {
                    res.Code = Code.BAD_REQUEST;
                    res.Status = "Cannot show userdata\n";
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {
                    default:
                        res.Status = "Something went wrong\n";
                        break;
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.Status = ex.Message;
                res.Code = Code.BAD_REQUEST;
            }
        }


        private void EditUserData(object obj, SqlCommand cmd, HttpRes res)
        {
            EditUserData EditUserDataObj = (EditUserData)obj;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = EditUserDataObj.Token;
            cmd.Parameters.Add("@Bio", SqlDbType.NVarChar).Value = EditUserDataObj.Bio;
            cmd.Parameters.Add("@Image", SqlDbType.NVarChar).Value = EditUserDataObj.Image;


            try
            {
                if (!sessionTokens.Contains(EditUserDataObj.Token))
                {
                    throw new Exception("User session not active");
                }
                
                cmd.ExecuteNonQuery();
                

                
                
                res.Code = Code.OK;
                res.Status = "Profile edited\n";
                
            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                switch (ex.Errors[0].Number)
                {
                    default:
                        res.Status = "Editing Profile failed\n";
                        res.Code = Code.BAD_REQUEST;
                        break;
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.Status = ex.Message;
                res.Code = Code.BAD_REQUEST;
            }
        }


        public Code Query(object obj, string sql, HttpRes res, SQLFunction func)
        {

            // All the info required to reach your db. See connectionstrings.com

            // Prepare a proper parameterized query 
  

            // Create the connection (and be sure to dispose it at the end)
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection to the database. 
                    // This is the first critical step in the process.
                    // If we cannot reach the db then we have connectivity problems
                    con.Open();

                    // Prepare the command to be executed on the db
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        switch (func)
                        {
                            case SQLFunction.LoginUser:
                                LoginUser(obj, cmd, res);
                                break;
                            case SQLFunction.RegisterUser:
                                RegisterUser(obj, cmd, res);
                                break;
                            case SQLFunction.CreatePackage:
                                CreatePackage(obj, cmd, res);
                                break;
                            case SQLFunction.ShowCardCollection:
                                ShowCardCollection(obj, cmd, res);
                                break;

                            case SQLFunction.Battle:
                                Battle(obj, cmd, res);
                                break;
                            case SQLFunction.AcquirePackage:
                                using (SqlCommand sp = new SqlCommand("dbo.acquire_package", con))
                                {
                                    AcquirePackage(obj, sp, res);

                                }
                                break;
                            case SQLFunction.ConfigureDeck:
                                using (SqlCommand sp = new SqlCommand("dbo.add_cards_to_deck", con))
                                {
                                    ConfigureDeck(obj, sp, res);

                                }
                                break;
                            case SQLFunction.ShowScoreboard:
                                ShowScoreboard(obj, cmd, res);
                                break;
                            case SQLFunction.ShowStats:
                                ShowStats(obj, cmd, res);
                                break;
                            case SQLFunction.ShowTrades:
                                ShowTrades(obj, cmd, res);
                                break;
                            case SQLFunction.CreateTrade:
                                using (SqlCommand sp = new SqlCommand("dbo.create_trade", con))
                                {
                                    CreateTrade(obj, sp, res);

                                }
                                
                                break;
                            case SQLFunction.AcceptTrade:
                                using (SqlCommand sp = new SqlCommand("dbo.accept_trade", con))
                                {
                                    AcceptTrade(obj, sp, res);

                                }
                                
                                break;
                            case SQLFunction.DeleteTrade:
                                using (SqlCommand sp = new SqlCommand("dbo.delete_trade", con))
                                {
                                    DeleteTrade(obj, sp, res);

                                }

                               
                                break;
                            case SQLFunction.ShowDeck:
                                ShowDeck(obj, cmd, res);
                                break;

                            case SQLFunction.ShowUserData:
                                ShowUserData(obj, cmd, res);
                                break;
                            case SQLFunction.EditUserData:
                                EditUserData(obj, cmd, res);
                                break;
                                default:
                                Console.WriteLine("Not supported database function");
                                break;
                            
                        }

                        

                    }
                }
                catch (Exception ex)
                {
                    // We should log the error somewhere, 
                    // for this example let's just show a message
                    Console.WriteLine("ERROR:" + ex.Message);
                    return Code.BAD_REQUEST;
                }
            }

            return Code.BAD_REQUEST;
        }

    }

}

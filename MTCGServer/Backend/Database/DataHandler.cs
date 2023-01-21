using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCGServer.Network;
using MTCGServer.Backend.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;



namespace MTCGServer.Backend.Database
{
    public class DataHandler
    {
        public Database db = new Database(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MTCG_DB;Integrated Security=True");
        
    
        public HttpRes RegisterUser(HttpReq req)
        {

            var UserObj = JsonConvert.DeserializeObject<User>(req.Json);
         

            // Prepare a proper parameterized query 
            string sql = "insert into dbo.MTCGUser (Username, Password) values(@Username,@Password)";

            
            var res = new HttpRes("", Code.DEFAULT, "");

            db.Query(UserObj, sql, res, Database.SQLFunction.RegisterUser);

            return res;
        }

        public HttpRes LoginUser(HttpReq req)
        {
            var UserObj = JsonConvert.DeserializeObject<User>(req.Json);

            string sql = "SELECT Password FROM dbo.MTCGUser where Username = @Username;";

        

            var res = new HttpRes("", Code.DEFAULT,"");
            db.Query(UserObj, sql, res, Database.SQLFunction.LoginUser);

            return res;
            
        }

        
        public HttpRes CreatePackage(HttpReq req)
        {
            var PackagesObj = JsonConvert.DeserializeObject<List<Card>>(req.Json);
            string sql = "insert into dbo.MTCGCard (CardID, Name, Damage, Element, isSpell) values(@CardID, @Name, @Damage, @Element, @isSpell)";
            var res = new HttpRes("", Code.DEFAULT,"");
            db.Query(PackagesObj, sql, res, Database.SQLFunction.CreatePackage);

            return res;
        } 

        public HttpRes AcquirePackage(HttpReq req)
        {
            string sql = "placeholder, using stored procedure instead";
            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');

            AuthToken TokenObj = new AuthToken { Token = split[0] };
            var res = new HttpRes("", Code.DEFAULT, "");
            db.Query(TokenObj, sql, res, Database.SQLFunction.AcquirePackage);

            return res;
        }

        public HttpRes AddPackages(HttpReq req)
        {
            return new HttpRes("", Code.NOT_IMPLEMENTED, "Added Packages");
        }

        public HttpRes ShowCardCollection(HttpReq req)
        {
            var res = new HttpRes("", Code.DEFAULT, "");
            
            if (!req.Headers.ContainsKey("Authorization"))
            {
                res.Code = Code.BAD_REQUEST;
                res.Status = "Authorization Token is missing";
                return res;
            }
            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');

            AuthToken TokenObj = new AuthToken { Token = split[0] };

  
            string sql = "SELECT * FROM dbo.MTCGCard WHERE Username = @Username";
            db.Query(TokenObj, sql, res, Database.SQLFunction.ShowCardCollection);



            return res;
        }

        public HttpRes ShowDeck(HttpReq req)
        {
            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');

            var ShowDeckObj = new ShowDeck { Token = split[0] };
            string path = req.Headers["Path"];

            if(path.Length == 18 && path.Substring(5,13) == "?format=plain")
            {
                ShowDeckObj.differentRepresentation = true;
            }
       
            String sql = @"SELECT t1.CardID, Name, Damage, Element, isSpell
                        FROM(
	                        SELECT FirstCard AS CardID, 'FirstCard' AS attr_name FROM dbo.MTCGUser WHERE Username = @Username
	                        UNION ALL
	                        SELECT SecondCard AS CardID, 'SecondCard' AS attr_name FROM dbo.MTCGUser WHERE Username = @Username
	                        UNION ALL
	                        SELECT ThirdCard AS CardID, 'ThirdCard' AS attr_name FROM dbo.MTCGUser WHERE Username = @Username
	                        UNION ALL
	                        SELECT FourthCard AS CardID, 'FourthCard' AS attr_name FROM dbo.MTCGUser WHERE Username = @Username
	                        ) AS t1
	                        INNER JOIN ( SELECT * FROM dbo.MTCGCard WHERE Username = @Username) AS t2
	                        ON t1.CardID = t2.CardID;
                        ";
            var res = new HttpRes("", Code.DEFAULT, "");

            db.Query(ShowDeckObj, sql, res, Database.SQLFunction.ShowDeck);

            return res;
            
        }

        public HttpRes ConfigureDeck(HttpReq req)
        {
            var Deck = JsonConvert.DeserializeObject<List<String>>(req.Json);
            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');

            DeckConfig DeckObj = new DeckConfig { Cards = Deck, Token = split[0] };

            string sql = "placeholder, using stored procedure instead";
            var res = new HttpRes("", Code.DEFAULT, "");
            db.Query(DeckObj, sql, res, Database.SQLFunction.ConfigureDeck);

            return res;
        }

        public HttpRes Battle(HttpReq req)
        {
            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');

            var TokenObj = new AuthToken { Token = split[0] };

            string sql = @"SELECT *
                            FROM(
	                            SELECT FirstCard AS CardID, 'FirstCard' AS attr_name FROM dbo.MTCGUser WHERE Username = @Username
	                            UNION ALL
	                            SELECT SecondCard AS CardID, 'SecondCard' AS attr_name FROM dbo.MTCGUser WHERE Username = @Username
	                            UNION ALL
	                            SELECT ThirdCard AS CardID, 'ThirdCard' AS attr_name FROM dbo.MTCGUser WHERE Username = @Username
	                            UNION ALL
	                            SELECT FourthCard AS CardID, 'FourthCard' AS attr_name FROM dbo.MTCGUser WHERE Username = @Username
	                            ) AS t1
	                            INNER JOIN ( SELECT * FROM dbo.MTCGCard WHERE Username = @Username) AS t2
	                            ON t1.CardID = t2.CardID;"; //load players deck into memory

            var res = new HttpRes("", Code.DEFAULT, "");

            db.Query(TokenObj, sql, res, Database.SQLFunction.Battle);

            return res;
        }

        public HttpRes ShowUserData(HttpReq req)
        {

            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');
            string path = req.Headers["Path"];
            string lastSegment = path.Substring(7);
            var TokenObj = new AuthToken { Token = split[0] };

            var res = new HttpRes("", Code.DEFAULT, "");
            if (lastSegment != TokenObj.Token)
            {
                res.Code = Code.BAD_REQUEST;
                res.Status = "Bad Token\n";
                return res;
            }


            string sql = "SELECT Username, Bio, Image FROM dbo.MTCGUser WHERE Username = @Username;";

            db.Query(TokenObj, sql, res, Database.SQLFunction.ShowUserData);

            return res;
        }

        public HttpRes EditUserData(HttpReq req)
        {

            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');
            string path = req.Headers["Path"];
            string lastSegment = path.Substring(7);
           

            var EditUserDataObj = JsonConvert.DeserializeObject<EditUserData>(req.Json);

            EditUserDataObj.Token = split[0];

            var res = new HttpRes("", Code.DEFAULT, "");
            if(lastSegment != EditUserDataObj.Token)
            {
                res.Code = Code.BAD_REQUEST;
                res.Status = "Bad Token\n";
                return res;
            }


            string sql = "UPDATE dbo.MTCGUser SET Bio = @Bio, Image = @Image WHERE Username = @Username";

            db.Query(EditUserDataObj, sql, res, Database.SQLFunction.EditUserData);

            return res;
        }

        public HttpRes ShowStats(HttpReq req)
        {
            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');

            var TokenObj = new AuthToken { Token = split[0] };

            string sql = "SELECT Username, Rating, Bio, Image FROM dbo.MTCGUser WHERE Username = @Username;";
            var res = new HttpRes("", Code.DEFAULT, "");

            db.Query(TokenObj, sql, res, Database.SQLFunction.ShowStats);

            return res;

        }

        public HttpRes AcceptTrade(HttpReq req)
        {
            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');
            string path = req.Headers["Path"];
            string lastSegment = path.Substring(10);

           
            

            var AcceptTradeObj = new AcceptTrade { Token = split[0] , TradeId = lastSegment, CardToTrade = req.Json.Substring(1,req.Json.Length-2)};

            string sql = "using sp";
            var res = new HttpRes("", Code.DEFAULT, "");

            db.Query(AcceptTradeObj, sql, res, Database.SQLFunction.AcceptTrade);

            return res;
        }

        public HttpRes CreateTrade(HttpReq req)
        {
            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');

            var TradeDealsObj = JsonConvert.DeserializeObject<TradeDeal>(req.Json);

            TradeDealsObj.Token = Token = split[0];

            string sql = "placeholder, using sp";
            var res = new HttpRes("", Code.DEFAULT, "");

            db.Query(TradeDealsObj, sql, res, Database.SQLFunction.CreateTrade);

            return res;
        }

        public HttpRes DeleteTrade(HttpReq req)
        {
            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');
            string path = req.Headers["Path"];
            string lastSegment = path.Substring(10);

            var TokenObj = new DeleteTrade { Token = split[0], TradeId =  lastSegment};

            string sql = "using sp";
            var res = new HttpRes("", Code.DEFAULT, "");

            db.Query(TokenObj, sql, res, Database.SQLFunction.DeleteTrade);

            return res;
        }

        public HttpRes ShowScoreboard(HttpReq req)
        {
            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');

            var TokenObj = new AuthToken { Token = split[0] };

            string sql = "SELECT Username, Rating FROM dbo.MTCGUser ORDER BY Rating desc;";
            var res = new HttpRes("", Code.DEFAULT, "");

            db.Query(TokenObj, sql, res, Database.SQLFunction.ShowScoreboard);

            return res;
        }

        public HttpRes ShowTrades(HttpReq req)
        {
            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');

            var TokenObj = new AuthToken { Token = split[0] };

            string sql = "SELECT Id, c.Name,c.Damage,c.Element,c.isSpell, d.isSpell, MinDmg FROM dbo.MTCGDeals d JOIN dbo.MTCGCard c ON d.CardToTrade = c.CardID;";
            var res = new HttpRes("", Code.DEFAULT, "");

            db.Query(TokenObj, sql, res, Database.SQLFunction.ShowTrades);

            return res;
        }

        
    }
}

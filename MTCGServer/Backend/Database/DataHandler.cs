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
    /*

            switch (req.Headers["Path"])
            {
                case "/users":
                    var RegisterUserObj = JsonConvert.DeserializeObject<User>(req.Json);
                    break;
                case "/packages":
                    List<Card> CardObj = JsonConvert.DeserializeObject<List<Card>>(req.Json);
                    break;
                case "/sessions":
                    var LoginUserObj = JsonConvert.DeserializeObject<User>(req.Json);
                    break;
                case "/transactions/packages":
                    var TokenObj = JsonConvert.DeserializeObject<AuthToken>(req.Json);
                    break;
                case "/cards":
                    //List<Card> CardObj = JsonConvert.DeserializeObject<List<Card>>(req.Json);
                    break;
            }
            */
    public class DataHandler
    {
        public Database db = new Database(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MTCG_DB;Integrated Security=True");
        //all functions, sql statements that access data
        


        public HttpRes RegisterUser(HttpReq req)
        {

            var UserObj = JsonConvert.DeserializeObject<User>(req.Json);

            // All the info required to reach your db. See connectionstrings.com
         

            // Prepare a proper parameterized query 
            string sql = "insert into dbo.MTCGUser (Username, Password) values(@Username,@Password)"; //make sure there are no duplicates

            // Create the connection (and be sure to dispose it at the end)
            var res = new HttpRes("", Code.DEFAULT, "");

            db.Query(UserObj, sql, res, Database.SQLFunction.RegisterUser);

            return res;
        }

        public HttpRes LoginUser(HttpReq req)
        {
            var UserObj = JsonConvert.DeserializeObject<User>(req.Json);

            string sql = "select top 1 * from dbo.MTCGUser where Username = @Username and Password = @Password";

        

            var res = new HttpRes("", Code.DEFAULT,"");
            db.Query(UserObj, sql, res, Database.SQLFunction.LoginUser);

            return res;
            
        }

        
        public HttpRes CreatePackage(HttpReq req)//admin only
        {
            var PackagesObj = JsonConvert.DeserializeObject<List<Card>>(req.Json);
            string sql = "insert into dbo.MTCGCard (CardID, Name, Damage, Element) values(@CardID, @Name, @Damage, @Element)";
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
            string Token = req.Headers["Authorization"].Substring(6);
            string[] split = Token.Split('-');

            AuthToken TokenObj = new AuthToken { Token = split[0] };

  
            string sql = "SELECT * FROM dbo.MTCGCard WHERE Username = @Username";
            var res = new HttpRes("", Code.DEFAULT, "");
            db.Query(TokenObj, sql, res, Database.SQLFunction.ShowCardCollection);



            return res;
        }

        public HttpRes ShowDeck(HttpReq req)
        {
            string path = req.Headers["Path"];

            if(path.Length == 18 && path.Substring(5,13) == "?format=plain")
            {
                return new HttpRes("", Code.NOT_IMPLEMENTED, "Different representation of deck shown");
            }
            else
            {

                return new HttpRes("", Code.NOT_IMPLEMENTED, "Deck shown");
            }
        }

        public HttpRes ConfigureDeck(HttpReq req)
        {
            return new HttpRes("", Code.NOT_IMPLEMENTED, "Deck configured");
        }

        public HttpRes EditUserData(HttpReq req)
        {
            return new HttpRes("", Code.NOT_IMPLEMENTED, "User data edited");
        }

        public HttpRes ShowStats(HttpReq req)
        {
            return new HttpRes("", Code.NOT_IMPLEMENTED, "Stats shown");
        }

        public HttpRes ShowScoreboard(HttpReq req)
        {
            return new HttpRes("", Code.NOT_IMPLEMENTED, "Scoreboard shown");
        }

        public HttpRes ShowTrades(HttpReq req)
        {
            return new HttpRes("", Code.NOT_IMPLEMENTED, "Trades shown");
        }

        
    }
}

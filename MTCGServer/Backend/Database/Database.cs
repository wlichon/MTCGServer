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
        public enum SQLFunction
        {
            LoginUser,
            RegisterUser,
            CreatePackage,
            AcquirePackage,
            ShowCardCollection
        }

        private string connectionString;

        public Database(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private static void LoginUser(object obj, SqlCommand cmd, HttpRes res)
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
                    res.Status = "OK\nUser logged in\n";

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

        private static void RegisterUser(object obj, SqlCommand cmd, HttpRes res)
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

        private static void CreatePackage(object ObjCards, SqlCommand cmd, HttpRes res)
        {
            int cardNumber = 1;
            bool err = false;
            cmd.Parameters.Add("@CardID", SqlDbType.NVarChar);
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar);
            cmd.Parameters.Add("@Damage", SqlDbType.NVarChar);
            cmd.Parameters.Add("@Element", SqlDbType.NVarChar);
            foreach (Card card in (List<Card>)ObjCards)
            {
                cmd.Parameters["@CardID"].Value = card.Id;
                cmd.Parameters["@Name"].Value = card.Name;
                cmd.Parameters["@Damage"].Value = card.Damage;
                cmd.Parameters["@Element"].Value = card.Element;
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

        private static void AcquirePackage(object obj, SqlCommand sp, HttpRes res)
        {
            AuthToken TokenObj = (AuthToken)obj;
            sp.CommandType = CommandType.StoredProcedure;

            sp.Parameters.Add(new SqlParameter("@Username", TokenObj.Token));
            try
            {

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
                    default:
                        res.Status = "Something went wrong\n";
                        res.Code = Code.BAD_REQUEST;
                        break;
                }
            }
        }

        private static void ShowCardCollection(object obj, SqlCommand cmd, HttpRes res)
        {
            AuthToken AuthObj = (AuthToken)obj;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = AuthObj.Token;

            DataSet ds = new DataSet();
            cmd.ExecuteNonQuery();
            var dAdap = new SqlDataAdapter();
            dAdap.SelectCommand = cmd;
            dAdap.Fill(ds, "result");

            var cards = new List<Card>();

            //DataTable Cards = new DataTable("Cards");
       
            try
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Console.WriteLine(row["Name"] + ", " + row["Damage"]);
                        var Card = new Card { Name = row["Name"].ToString(), Damage = Convert.ToDouble(row["Damage"].ToString()), Element = row["Element"].ToString() };
                        cards.Add(Card);
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
                    res.Status = JsonConvert.SerializeObject(cards);

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
                            case SQLFunction.AcquirePackage:
                                using (SqlCommand sp = new SqlCommand("dbo.acquire_package", con))
                                {
                                    AcquirePackage(obj, sp, res);

                                }
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

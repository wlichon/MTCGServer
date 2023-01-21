using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCGServer.Backend.Database;
using MTCGServer.Network;
using Newtonsoft;
using System.Text.RegularExpressions;

namespace MTCGServer.Backend.BusinessLogic
{
    public class Logic
    {
        private DataHandler dh = new DataHandler();

        public HttpRes? HandleRequest(HttpReq req)
        {

            HttpRes? res;

            if (!req.Headers.ContainsKey("Path"))
            {
                res = null;
                return res;
            }

            switch (req.Headers["Path"])
            {
                case "/users":
                    res = dh.RegisterUser(req); 
                    break;
                case "/packages":
                    res = dh.CreatePackage(req); 
                    break;
                case "/sessions":
                    res = dh.LoginUser(req); 
                    break;
                case "/transactions/packages":
                    res = dh.AcquirePackage(req); 
                    break;
                case "/cards":
                    res = dh.ShowCardCollection(req);
                    break;
                case "/stats":
                    res = dh.ShowStats(req);
                    break;
                case "/tradings":
                    if(req.Method == Network.HttpMethod.Get)
                    {
                        res = dh.ShowTrades(req);

                    }
                    else if(req.Method == Network.HttpMethod.Post)
                    {
                        res = dh.CreateTrade(req);
                    }
                    else
                    {

                        res = null;
                    }
                    break;
                case var someURL when new Regex(@"/tradings/[a-zA-Z0-9-]+").IsMatch(someURL):
                    if(req.Method == Network.HttpMethod.Post)
                    {
                        res = dh.AcceptTrade(req);

                    }
                    else if (req.Method == Network.HttpMethod.Delete)
                    {
                        res = dh.DeleteTrade(req);

                    }
                    else
                    {
                        res = null;
                    }
                    
                    break;
                case "/battles":
                    res = dh.Battle(req);
                    break;
                case var someURL when new Regex(@"/users/[a-zA-Z]+").IsMatch(someURL): //matches /users/user where user is a user profile to be edited
                    if(req.Method == Network.HttpMethod.Get)
                    {
                        res = dh.ShowUserData(req);
                    }
                    else
                    {

                        res = dh.EditUserData(req);
                    }
                    break;
                case "/score":
                    res = dh.ShowScoreboard(req);
                    break;
                case var someURL when new Regex(@"/deck(\?[a-zA-Z]+=[a-zA-Z]+)?").IsMatch(someURL):
                    if (req.Method == Network.HttpMethod.Get)
                    {
                        res = dh.ShowDeck(req);

                    }
                    else if (req.Method == Network.HttpMethod.Put)
                    {
                        res = dh.ConfigureDeck(req);

                    }
                    else
                    {
                        res = null;
                    }
                    break;
                default:
                    res = null;
                    break;
            }

            return res;
        }

    }
}

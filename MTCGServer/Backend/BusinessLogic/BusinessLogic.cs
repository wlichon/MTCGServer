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
                    res = dh.RegisterUser(req); // done
                    break;
                case "/packages":
                    res = dh.CreatePackage(req); // done
                    break;
                case "/sessions":
                    res = dh.LoginUser(req); // done
                    break;
                case "/transactions/packages":
                    res = dh.AcquirePackage(req); // done, not sure about edge cases where there arent 5 cards free
                    break;
                case "/cards":
                    res = dh.ShowCardCollection(req); //
                    break;
                case "/stats":
                    res = dh.ShowStats(req);
                    break;
                case "/tradings":
                    res = dh.ShowTrades(req);
                    break;
                case var someURL when new Regex(@"/users/[a-zA-Z]+").IsMatch(someURL): //matches /users/user where user is a user profile to be edited
                    res = dh.EditUserData(req);
                    break;
                case var someURL when new Regex(@"/deck(\?[a-zA-Z]+=[a-zA-Z]+)?").IsMatch(someURL):
                    res = dh.ShowDeck(req);
                    break;
                default:
                    res = null;
                    break;
            }

            return res;
        }

    }
}

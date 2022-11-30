using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCGServer.Backend.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Damage { get; set; }
        public string Element { get; set; }
    }

    public class AuthToken
    {
        public string Token { get; set; }
    }
}

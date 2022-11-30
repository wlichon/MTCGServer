using MTCGServer.Network;
using MTCGServer.Backend;
using System.Text.RegularExpressions;

namespace MTCGServer
{
    class Program
    {
        public static int Main(string[] args)
        {
            /*
            string s = "/deck?format=plain";
            switch (s)
            {
                case var someURL when new Regex(@"/users/[a-zA-Z]+").IsMatch(someURL):
                    Console.WriteLine("editing specific user");
                    break;
                case var someURL when new Regex(@"/deck\?[a-zA-Z]+=[a-zA-Z]+").IsMatch(someURL):
                    Console.WriteLine("different format deck");
                    break;
                case "/users":
                    Console.WriteLine("registering user");
                    break;
            }
            return 1;
            */
            var server = new Network.MyTcpListener();

            server.StartServer();

            return 0;

        }
    }
}
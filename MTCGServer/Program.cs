using MTCGServer.Network;
using MTCGServer.Backend;
using System.Text.RegularExpressions;

namespace MTCGServer
{
    class Program
    {
        public static int Main(string[] args)
        {
           
            var server = new Network.MyTcpListener();

            server.StartServer();

            return 0;

        }
    }
}
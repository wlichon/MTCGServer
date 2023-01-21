using MTCGServer;
using Newtonsoft.Json;
using MTCGServer.Backend.Models;
using MTCGServer.Backend.BusinessLogic;
using MTCGServer.Network;
using HttpMethod = MTCGServer.Network.HttpMethod;

namespace MTCGServer.Backend
{
    public interface IServiceHandler
    {
        public HttpRes Request(HttpReq req);

        
    }

    public class ServiceHandler : IServiceHandler
    {
        private Logic logic = new Logic();
        public HttpRes Request(HttpReq req)
        {
          

            HttpRes? res = logic.HandleRequest(req);


            return res;
            
        }

    }
}
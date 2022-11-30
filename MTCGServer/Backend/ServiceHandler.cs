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
        public HttpRes Request(HttpReq req)
        {
            //string? path = req.Headers.ContainsKey("Path") ? req.Headers["Path"] : null;
            string? method;
           
            switch (req.Method)
            {
                case HttpMethod.Get:
                    method = "GET";
                    break;
                case HttpMethod.Put:
                    method = "PUT";
                    break;
                case HttpMethod.Post:
                    method = "POST";
                    break;
                default:
                    method = null;
                    break;
            }

            var logic = new Logic();

            HttpRes? res = logic.HandleRequest(req);


            return res;
            
        }

    }
}
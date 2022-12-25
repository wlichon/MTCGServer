using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.RegularExpressions;
using MTCGServer.Backend;

namespace MTCGServer.Network
{
    public enum HttpMethod
    {
        Get,
        Post,
        Put
    }

    public enum Code { 
        DEFAULT = 0,
        OK = 200, 
        BAD_REQUEST = 400, 
        UNAUTHORIZED = 401, 
        FORBIDDEN = 403, 
        NOT_FOUND = 404, 
        INTERNAL_SERVER_ERROR = 500, 
        NOT_IMPLEMENTED = 501, 
        BAD_GATEWAY = 502 
    };

    public class HttpReq
    {
       
        public HttpReq(Dictionary<string, string> headers, HttpMethod? method)
        {
            this.headers = headers;
            this.method = method;
        }
        private Dictionary<string, string> headers;
        public Dictionary<string, string> Headers
        {
            get { return headers; }
            set { headers = value; }
        }
        private HttpMethod? method;
        public HttpMethod? Method
        {
            get { return method; }
            set { method = value; }
        }

        private string json;

        public string Json
        {
            get { return json; }
            set { json = value; }
        }


    }

    public class HttpRes
    {
        private string body;
        private Code code;
        private string status;


        public HttpRes(string body,Code code, string status)
        {
            
            this.body = body;
            this.code = code;
            this.status = status;

        }

        public Code Code { get { return code; }  set { code = value; } }

        public string Status { get { return status; } set { status = value; } }
        public string Body
        {
            get { return body; }
            set { body = value; }
        }
    }
    public class MyTcpListener
    {

        public HttpReq FillDictionary(List<string> headers)
        {
            //HttpMethod? method = null;
            var dict = new Dictionary<string, string>();
            foreach (string header in headers)
            {
                string? key = null;
                string? value = null;
                bool readKey = true;

                for (int i = 0; i < header.Length; i++)
                {
                    /*
                    if (value == "GET")
                    {
                        method = HttpMethod.Get;
                        value = null;
                        i++;
                        continue;
                    }

                    if (value == "POST")
                    {
                        method = HttpMethod.Post;
                        value = null;
                        i++;
                        continue;
                    }
                    */


                    if (header[i] == ':')
                    {
                        readKey = false;
                        i++; //um blankspace zu skippen
                        continue;

                    }

                    if (readKey)
                    {
                        key += header[i];

                    }
                    else
                    {
                        value += header[i];
                    }


                }
                dict.Add(key, value);

            }

            return new HttpReq(dict, null);


        }

        public HttpReq ParseStringForHeaders(string streamstring)
        {


            string patternToken = @"Authorization: Basic [a-z]+-mtcgToken";
            Match matchToken = Regex.Match(streamstring, patternToken);

            string patternContentType = @"Content-Type: [a-zA-Z]+\/[a-zA-Z]+";
            Match matchContentType = Regex.Match(streamstring, patternContentType);

            string patternURL = @"(/[a-zA-Z]+)+(\?[a-zA-Z]+=[a-zA-Z]+)*";
            Match matchURL = Regex.Match(streamstring, patternURL);


            List<string> list = new List<string>();
            if (matchToken.Success)
                list.Add(matchToken.Value);
            if (matchContentType.Success)
                list.Add(matchContentType.Value);
            if (matchURL.Success)
                list.Add("Path: " + matchURL.Value);

            HttpReq req = FillDictionary(list);
            if (streamstring.Substring(0, 4) == "POST")
            {
                req.Method = HttpMethod.Post;

            }
            else if (streamstring.Substring(0, 3) == "PUT")
            {
                req.Method = HttpMethod.Put;
            }
            else
            {
                req.Method = HttpMethod.Get;
            }

            return req;
        }
        public string StreamStringToJsonString(string streamstring)
        {
            string jsonString = "";
            int newlineCounter = 0;

            for (int i = 0; i < streamstring.Length; i++)
            {

                if (newlineCounter == 2)
                {
                    for (int j = i; j < streamstring.Length; j++)
                    {
                        jsonString += streamstring[j];
                    }
                }
                if (streamstring[i] == '\r' && streamstring[i + 1] == '\n')
                {
                    newlineCounter++;
                    i++;
                    continue;
                }

                newlineCounter = 0;
            }
            return jsonString;
        }

        public byte[] Response(Code HttpStatus, string? data)
        {
            byte[] msg;
            if (data != null)
            {
                data = HttpStatus.ToString() + '\n' + data;
                msg = System.Text.Encoding.ASCII.GetBytes(data);
            }
            else
            {
                msg = System.Text.Encoding.ASCII.GetBytes(HttpStatus.ToString());
            }

            return msg;
            
        }


        public void StartServer()
        {
            TcpListener server = null;
            var handler = new ServiceHandler();
            try
            {

                Int32 port = 10001;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");


                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                byte[] bytes = new byte[1024];
                string data = null;


                // Enter the listening loop.
                while (true)
                {


                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.

                    using TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int readBytes;

                    // Loop to receive all the data sent by the client.
                    while (client.Available > 0 && (readBytes = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, readBytes);

                        HttpReq req = ParseStringForHeaders(data);


                        string jsonString = StreamStringToJsonString(data);
                        req.Json = jsonString;

                        Console.WriteLine("Http Method is = {0}", req.Method);

                        foreach (var header in req.Headers)
                        {
                            Console.WriteLine("header key = {0}, header value = {1}", header.Key, header.Value);
                        }




                        Console.WriteLine("Received: {0}", data);


                       

                        HttpRes res = handler.Request(req);

                        byte[] msg = null;

                        if(res == null)
                        {
                            msg = Response(Code.NOT_FOUND, null);
                        }
                        else
                        {
                            msg = Response(res.Code, res.Status);
                        }

                        stream.Write(msg, 0, msg.Length);





                        //var jsonObj = JsonConvert.DeserializeObject<User>(jsonString);
                        //var jsonObj = JsonConvert.DeserializeObject<List<Cards>>(jsonString);


                        //Console.WriteLine("Der Username im Request ist: " + jsonObj.Username + "und Passwort ist: " + jsonObj.Password);

                        //Console.WriteLine(jsonObj.Host);





                        //var obj = jsonString.

                        // Process the data sent by the client.
                        //data = data.ToUpper();



                        // Send back a response.

                        //Console.WriteLine("Sent: {0}", msg);
                    }

                    // Shutdown and end the connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}

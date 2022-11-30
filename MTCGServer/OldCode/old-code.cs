/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;      //required
using System.Net.Sockets;    //required

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 13000);
            // we set our IP address as server's address, and we also set the port: 9999

            server.Start();  // this will start the server

            while (true)   //we wait for a connection
            {
                TcpClient client = server.AcceptTcpClient();  //if a connection exists, the server will accept it
                Console.WriteLine("Connected\n");
                NetworkStream ns = client.GetStream(); //networkstream is used to send/receive messages

                byte[] hello = new byte[100];   //any message must be serialized (converted to byte array)
                hello = Encoding.Default.GetBytes("hello world");  //conversion string => byte array

                ns.Write(hello, 0, hello.Length);     //sending the message

                while (client.Connected)  //while the client is connected, we look for incoming messages
                {
                    byte[] msg = new byte[1024];     //the messages arrive as byte array
                    ns.Read(msg, 0, msg.Length);   //the same networkstream reads the message sent by the client
                    Console.WriteLine(System.Text.Encoding.ASCII.GetString(msg).Trim(' ')); //now , we write the message as string
                }
            }

        }
    }
}


*/

/*
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;




namespace MTCGServer
{
    

    class MyTcpListener
    {
        public static void Main()
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    using TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int readBytes;

                    stream.Read(bytes, 0, bytes.Length);

                    // Loop to receive all the data sent by the client.
                    while ((readBytes = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, readBytes);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        //data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes("Sending back response\n");

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", System.Text.Encoding.ASCII.GetString(msg));
                        /*
                        try
                        {
                            stream.Read(bytes, 0, bytes.Length);
                        }catch(Exception ex)
                        {
                            Console.WriteLine("SocketException: {0}", ex);
                        }
                        break;
                    }
                    client.Close();

                    // Shutdown and end the connection
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

                        */


/*
class Program
{
    private static byte[] _buffer = new byte[1024];
    private static List<Socket> _clientSockets = new List<Socket>();
    private static Socket _serverSocket = new Socket
        (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


    static void Main(string[] args)
    {

    }

    private void SetupServer()
    {
        Console.WriteLine("Setting up server... \n");
        _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 10001));
        _serverSocket.Listen(1);
        _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
    }

    private static void AcceptCallback(IAsyncResult AR)
    {
        Socket socket = _serverSocket.EndAccept(AR);
        _clientSockets.Add(socket);
        socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
    }

    private static void ReceiveCallback(IAsyncResult AR)
    {
        Socket socket = (Socket)AR.AsyncState;
        int received = socket.EndReceive(AR);
        byte[] dataBuf = new byte[received];
        Array.Copy(_buffer, dataBuf, received);

        string text = Encoding.ASCII.GetString(dataBuf);

        Console.WriteLine("Text received: " + text);

        if(text.ToLower() == "get time")
        {
            SendText(DateTime.Now.ToLongTimeString(),socket);
        }
        else
        {
            SendText("Invalid Request\n", socket);
        }



    }

    private static void SendText(string text, Socket socket)
    {
        byte[] data = Encoding.ASCII.GetBytes(text);
        socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
        socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
    }

    private static void SendCallback(IAsyncResult AR)
    {
        Socket socket = (Socket)AR.AsyncState;
        socket.EndSend(AR);
    }

}
}

*/
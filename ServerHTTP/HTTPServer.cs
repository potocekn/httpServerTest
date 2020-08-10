using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace ServerHTTP
{
    public class HTTPServer
    {
        public const string MSG_DIR = "/Root/Msg/";
        public const string WEB_DIR = "/Root/Web/";
        public const string VERSION = "HTTP/1.1";
        public const string NAME = "Test Server Zapoctak";
        private bool running = false;

        private TcpListener listener;

        public HTTPServer(int port)
        {
            this.listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            Thread serverThread = new Thread(new ThreadStart(Run));
            serverThread.Start();
        }

        private void Run()
        {
            running = true;
            listener.Start();

            while (running)
            {
                Console.WriteLine("Waiting for connection");
                TcpClient client = listener.AcceptTcpClient();

                Console.WriteLine("Client connected!");

                HandleClient(client);

                client.Close();
            }

            running = false;
            listener.Stop();
        }

        private void HandleClient(TcpClient client)
        {
            StreamReader reader = new StreamReader(client.GetStream());

            string msg = "";
            while (reader.Peek() != -1)
            {
                msg += reader.ReadLine() + '\n';
            }

            Debug.WriteLine("Request: \n" + msg);

            Request request = Request.GetRequest(msg);
            Response response = Response.From(request);
            response.Post(client.GetStream());
        }
    }
}

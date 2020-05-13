using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Deep_Jira_Server
{
    class Server
    {
        TcpListener server = null;

        public Server(string ip, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            server.Start();
            StartListener();
        }

        public void StartListener()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        public void HandleDeivce(object obj)
        {
            TcpClient client = (TcpClient)obj;

            try
            {
                byte[] bytes = new byte[1024];
                Stream stream = client.GetStream();
                HttpProcessor httpProcessor = new HttpProcessor();
                HttpRequest request = httpProcessor.GetRequest(stream);
                Console.WriteLine(request.Method);
                //httpProcessor.WriteRequest();
                //HttpRequest request = HttpRequest();
                //dynamic req = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes));//<object>(Encoding.UTF8.GetString(bytes),settings);
                //Console.WriteLine(request.ToString());
                //Console.WriteLine("sended request");
                //client.Close();
                //httpProcessor.WriteRequest((string)req.issue.key, (string)req.user.key, (string)req.issue.fields.customfield_10404[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                client.Close();
            }
        }
    }
}

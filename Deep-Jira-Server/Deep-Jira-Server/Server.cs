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
        string jiraLogin;
        string deeplLogin;

        public Server(string ip, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            setLoginData();
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
                    Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        public void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;

            try
            {
                Stream stream = client.GetStream();
                HttpProcessor httpProcessor = new HttpProcessor();
                HttpRequest request = httpProcessor.GetRequest(stream);
                httpProcessor.SendResponse(stream);
                JiraConnector jiraConnector = new JiraConnector(jiraLogin, deeplLogin);
                jiraConnector.GetWebhook(request.Url, request.Content);
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                client.Close();
            }
        }

        private void setLoginData()
        {
            string[] loginData = System.IO.File.ReadAllLines(@"C:\Users\bashx\Desktop\Login.txt");
            jiraLogin = loginData[0];
            deeplLogin = loginData[1];
        }
    }
}

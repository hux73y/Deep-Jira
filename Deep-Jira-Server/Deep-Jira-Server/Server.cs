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
        HttpListener webServer = null;

        public Server(string [] prefixes)
        {
            webServer = new HttpListener();
            foreach (string s in prefixes)
                webServer.Prefixes.Add(s);

            webServer.Start();
            StartListener();
        }

        public void StartListener()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    HttpListenerContext context = webServer.GetContext();
                    Console.WriteLine("Connected!");
                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(context);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                webServer.Stop();
            }
        }

        public void HandleDeivce(object obj)
        {
            HttpListenerContext context = (HttpListenerContext)obj;

            try
            {
                HttpListenerRequest request = context.Request;

                HttpListenerResponse response = context.Response;

                string responseString = "<HTML><Body> Hello world!</Body></HTML>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
            }
        }
    }
}

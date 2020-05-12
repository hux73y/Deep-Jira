using System;
using System.Threading;

namespace Deep_Jira_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread t = new Thread(delegate ()
            {
                //string ip = Console.ReadLine();
                // replace the IP with your system IP Address...
                //Server crowdServer = new Server("10.242.22.241", 5050);
                string[] prefixes = { "http://192.168.178.39:80/start/" }; 
                Server server = new Server(prefixes);
            });
            t.Start();

            Console.WriteLine("Server Started");
            //HttpProcessor httpProcessor = new HttpProcessor();
            //httpProcessor.WriteRequest();
        }
    }
}

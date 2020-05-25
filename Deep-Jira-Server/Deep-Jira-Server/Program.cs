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
                Server server = new Server("192.168.178.39", 80);
            });
            t.Start();

            Console.WriteLine("Server Started");
            //HttpProcessor processor = new HttpProcessor();
            //processor.Response("test");
        }
    }
}

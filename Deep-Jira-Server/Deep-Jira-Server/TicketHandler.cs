using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Deep_Jira_Server
{
    class TicketHandler
    {
        static readonly System.Object lockThis = new System.Object();
        public TicketHandler()
        {

        }
        public void writeTicket(string issueKey, string sourceLanguage)
        {
                try
                {
                    lock (lockThis)
                    {
                        using (StreamWriter outStream = File.AppendText("C:\\Users\\bashx\\source\\repos\\Deep-Jira\\Deep-Jira-Server\\Deep-Jira-Server\\Tickets.txt"))
                        {
                            outStream.WriteLine(issueKey + " " + sourceLanguage);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while reading File");
                }
        }
        public string readLanguage(string issueKey)
        {
            
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                lock (lockThis)
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader("C:\\Users\\bashx\\source\\repos\\Deep-Jira\\Deep-Jira-Server\\Deep-Jira-Server\\Tickets.txt"))
                    {
                        string line;
                        // Read and display lines from the file until the end of
                        // the file is reached.
                        while ((line = sr.ReadLine()) != null)
                        {
                            string [] pairs = line.Split(" ", 2);
                            if (pairs[0].Equals(issueKey))
                                return pairs[1];
                        }
                        return "noLanguage";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while reading File");
                return "noLanguage";
            }
        }
        public void deleteTicket(string issueKey)
        {
            string tempFile = Path.GetTempFileName();

            using (var sr = new System.IO.StreamReader("C:\\Users\\bashx\\source\\repos\\Deep-Jira\\Deep-Jira-Server\\Deep-Jira-Server\\Tickets.txt"))
            using (var sw = new StreamWriter(tempFile))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] pairs = line.Split(" ", 2);
                    if (pairs[0] != issueKey)
                        sw.WriteLine(line);
                }
            }

            File.Delete("C:\\Users\\bashx\\source\\repos\\Deep-Jira\\Deep-Jira-Server\\Deep-Jira-Server\\Tickets.txt");
            File.Move(tempFile, "C:\\Users\\bashx\\source\\repos\\Deep-Jira\\Deep-Jira-Server\\Deep-Jira-Server\\Tickets.txt");
        }
    }
}

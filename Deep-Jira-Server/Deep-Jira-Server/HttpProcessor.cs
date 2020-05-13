using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Deep_Jira_Server
{
    class HttpProcessor
    {
        public HttpProcessor()
        {

        }
        private string ReadLine(Stream stream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = stream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; }
                data += Convert.ToChar(next_char);
            }
            return data;
        }
        public HttpRequest GetRequest(Stream inputStream)
        {
            string request = ReadLine(inputStream);

            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            string method = tokens[0].ToUpper();
            string url = tokens[1];
            string protocolVersion = tokens[2];

            Dictionary<string, string> headers = new Dictionary<string, string>();
            string line;
            while ((line = ReadLine(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    break;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                string name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++;
                }

                string value = line.Substring(pos, line.Length - pos);
                headers.Add(name, value);
            }

            string content = null;
            if (headers.ContainsKey("Content-Length"))
            {
                int totalBytes = Convert.ToInt32(headers["Content-Length"]);
                int bytesLeft = totalBytes;
                byte[] bytes = new byte[totalBytes];

                while (bytesLeft > 0)
                {
                    byte[] buffer = new byte[bytesLeft > 1024 ? 1024 : bytesLeft];
                    int n = inputStream.Read(buffer, 0, buffer.Length);
                    buffer.CopyTo(bytes, totalBytes - bytesLeft);

                    bytesLeft -= n;
                }

                content = Encoding.ASCII.GetString(bytes);
            }

            return new HttpRequest()
            {
                Method = method,
                Url = url,
                Headers = headers,
                Content = content
            };
        }
        public void WriteResponse(Stream output)
        {
            string responseString = "<HTML><Body> Hello world!</Body></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            output.Write(buffer, 0, buffer.Length);
        }
        public void WriteRequest()
        {
            string url = "https://team-percy.atlassian.net/rest/servicedeskapi/auth/issue/LIS-2";
            //string url = "https://appme-d.miele.com/operations/rest/auth/1/session";

            WebRequest wr = WebRequest.Create(url);

            //NetworkCredential nc = new NetworkCredential("dewuenp", "h-!u232xl?ey");
            //wr.Credentials = nc;

            wr.ContentType = "application/json";
            wr.Method = "POST";
            string json = "{ \"username\": \"dewuenp\", \"password\": \"h-!u232xl?ey\" } ";
            byte[] content = Encoding.UTF8.GetBytes(json);
            wr.ContentLength = content.Length;
            Stream dataStream = wr.GetRequestStream();
            dataStream.Write(content, 0, content.Length);
            dataStream.Close();
            WebResponse response = wr.GetResponse();
            dynamic jsonObject = JsonConvert.DeserializeObject(StreamReader.ReadString(response));

            Uri target = new Uri("https://appme-d.miele.com/operations/rest/api/2/issue/" + 12 + "/comment");
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(target);
            hwr.CookieContainer = new CookieContainer();
            hwr.CookieContainer.Add(new Cookie((string)jsonObject.session.name, (string)jsonObject.session.value) { Domain = target.Host });
            hwr.Method = "POST";
            hwr.ContentType = "application/json";

            //DBManager dbm = new DBManager();
            //dbm.write("INSERT INTO license (key, created_at, expires_at, licensee, tool, expired) VALUES" +
            //    " (" + licenseKey + ", "+ DateTime.Now.ToString() + "," + "01.01.2021" + ", " + user + ", " + tool + "," + 0.ToString() + ")");

            json = " ";
            content = Encoding.UTF8.GetBytes(json);
            hwr.ContentLength = content.Length;
            dataStream = hwr.GetRequestStream();
            dataStream.Write(content, 0, content.Length);
            dataStream.Close();
            response = hwr.GetResponse();
            Console.WriteLine(StreamReader.ReadString(response));
            //JiraIssue issue = new JiraIssue("Low");
            //string json = JsonSerializer.Serialize(issue);
            //byte[] content = Encoding.ASCII.GetBytes(json);
            //request.ContentLength = content.Length;
            //request.ContentType = "application/json";
            //Stream dataStream = request.GetRequestStream();
            //dataStream.Write(content, 0, content.Length);
            //dataStream.Close();
        }
    }
}

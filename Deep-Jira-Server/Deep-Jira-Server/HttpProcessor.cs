using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
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
        public void Request()
        {
            Uri url = new Uri("https://team-percy.atlassian.net/rest/servicedeskapi/request");
            WebRequest myWebRequest = WebRequest.Create(url);
            string encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("percy.wuensch@gmx.de:g7LKzdmjXetO76N8AZcb8982"));
            myWebRequest.Headers.Add("Authorization", "Basic " + encoded);
            myWebRequest.ContentType = "application/json";
            myWebRequest.Method = "GET";
            myWebRequest.PreAuthenticate = true;
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Console.WriteLine(StreamReader.ReadString(myWebResponse));
        }
        public void Response(string content)
        {
          
            dynamic jsonObject = JsonConvert.DeserializeObject(content);
            
            string url = "https://team-percy.atlassian.net/rest/api/2/issue/" + (string)jsonObject.issue.key;
            
            WebRequest wr = WebRequest.Create(url);
            string encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("percy.wuensch@gmx.de:g7LKzdmjXetO76N8AZcb8982"));
            wr.Headers.Add("Authorization", "Basic " + encoded);
            wr.ContentType = "application/json";
            wr.Method = "PUT";
            string json = "{\"fields\": {\"description\": \"die neue beschreibung\"} }";
            byte[] responseContent = Encoding.UTF8.GetBytes(json);
            wr.ContentLength = responseContent.Length;
            Stream dataStream = wr.GetRequestStream();
            dataStream.Write(responseContent, 0, responseContent.Length);
            dataStream.Close();
            WebResponse response = wr.GetResponse();
            Console.WriteLine(StreamReader.ReadString(response));
        }
    }
}

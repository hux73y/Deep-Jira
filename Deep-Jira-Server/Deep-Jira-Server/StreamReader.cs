using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Deep_Jira_Server
{
    class StreamReader
    {
        public static string ReadString(WebResponse response)
        {
            var encoding = ASCIIEncoding.UTF8;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
                return responseText;
            }
        }
    }
}

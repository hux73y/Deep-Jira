using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Deep_Jira_Server
{
    class DeeplConnector
    {
        private string dLogin;
        
        public DeeplConnector(string deeplLogin)
        {
            dLogin = deeplLogin;
        }
       
        public Translation Translate(string targetLanguage, string text)
        {
            string deeplUrl = "https://api.deepl.com/v2/translate";
            WebRequest wr = WebRequest.Create(deeplUrl);
           
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.Method = "POST";
            string content = "auth_key=7a789262-601c-678c-bae3-49af96a92acb&text=" + text + "&target_lang=" + targetLanguage;
            return WriteContent(content, wr);

        }
        private Translation WriteContent(string content, WebRequest wr)
        {
            byte[] responseContent = Encoding.UTF8.GetBytes(content);
            wr.ContentLength = responseContent.Length;
            Stream dataStream = wr.GetRequestStream();
            dataStream.Write(responseContent, 0, responseContent.Length);
            dataStream.Close();
            WebResponse response = wr.GetResponse();
            dynamic jsonObject = JsonConvert.DeserializeObject(StreamReader.ReadString(response));
            return new Translation((string)jsonObject.translations[0].detected_source_language, (string)jsonObject.translations[0].text);
        }
    }
}

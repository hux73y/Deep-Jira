using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Deep_Jira_Server
{
    class JiraConnector
    {
        private string jiraUrl = "https://team-percy.atlassian.net/rest/api/2/issue/";

        private DeeplConnector dc;
        public JiraConnector()
        {
            dc = new DeeplConnector();
        }
        private WebRequest CreateRequest(string url)
        {
            WebRequest wr = WebRequest.Create(url);
            string encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("percy.wuensch@gmx.de:g7LKzdmjXetO76N8AZcb8982"));
            wr.Headers.Add("Authorization", "Basic " + encoded);
            wr.ContentType = "application/json";
            wr.Method = "PUT";

            return wr;
        }
        private void WriteContent(string content, WebRequest wr)
        {
            byte[] responseContent = Encoding.UTF8.GetBytes(content);
            wr.ContentLength = responseContent.Length;
            Stream dataStream = wr.GetRequestStream();
            dataStream.Write(responseContent, 0, responseContent.Length);
            dataStream.Close();
            WebResponse response = wr.GetResponse();
            Console.WriteLine(response.ToString());
        }
        private void CreateRequest(string issueKey, string summary, string description)
        {

            //Get tranlation from deepl here and store info
            Translation translationSummary = dc.Translate("EN",summary);
            Translation translationDescription = dc.Translate("EN", description);
            TicketHandler th = new TicketHandler();
            th.writeTicket(issueKey,translationDescription.SourceLanguage);
            
            //Create Request to Jira API for editing the description of an issue
            string url = jiraUrl + issueKey;
            WebRequest wr = CreateRequest(url);
            //Add translation from deepl here
            string json = "{\"fields\": {\"description\": \"" + translationDescription.Text + "\", \"summary\": \"" + translationSummary.Text + "\"} }";

            WriteContent(json, wr);
        }
        private void ResponseAsigneeComment(string issueKey, string commentId, string comment)
        {
            
            string url = jiraUrl + issueKey + "/comment/" + commentId;
            WebRequest wr = CreateRequest(url);

            //get customer language and deepl translation here
            TicketHandler th = new TicketHandler();
            string customerLanguage = th.readLanguage(issueKey);
            Translation commentTranslation = dc.Translate(customerLanguage, comment);
            string json = "{\"body\": \"" + commentTranslation.Text + "\" }";
            WriteContent(json, wr);
        }
        private void ResponseCustomerComment(string issueKey, string commentId, string comment)
        {

            string url = jiraUrl + issueKey + "/comment/" + commentId;
            WebRequest wr = CreateRequest(url);

            //get Deepl translation here
            Translation commentTranslation = dc.Translate("EN", comment);
            string json = "{\"body\": \"" + commentTranslation.Text + "\" }";
            
            WriteContent(json, wr);
        }
        private void ResponseDeleteTicket(string issueKey)
        {
            TicketHandler tc = new TicketHandler();
            tc.deleteTicket(issueKey);
        }
        
        public void GetWebhook(string path, string content)
        {
            dynamic jsonObject = JsonConvert.DeserializeObject(content);

            if (path.Equals("/create"))
                CreateRequest((string)jsonObject.issue.key, (string)jsonObject.issue.fields.summary, (string)jsonObject.issue.fields.description);
            else if (path.Equals("/asigneeComment"))
                ResponseAsigneeComment((string)jsonObject.issue.key, (string)jsonObject.comment.id, (string)jsonObject.comment.body);
            else if (path.Equals("/customerComment"))
                ResponseCustomerComment((string)jsonObject.issue.key, (string)jsonObject.comment.id, (string)jsonObject.comment.body);
            else if (path.Equals("/deleteTicket"))
                ResponseDeleteTicket((string)jsonObject.issue.key);
        }
    }
}

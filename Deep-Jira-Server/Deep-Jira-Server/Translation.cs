using System;
using System.Collections.Generic;
using System.Text;

namespace Deep_Jira_Server
{
    class Translation
    {
        public string SourceLanguage { get; set; }
        public string Text { get; set; }
        public Translation (string sourceLanguage, string text)
        {
            SourceLanguage = sourceLanguage;
            Text = text;
        }
    }
}

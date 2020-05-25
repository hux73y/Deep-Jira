using System;
using System.Collections.Generic;
using System.Text;

namespace Deep_Jira_Server
{
    class TicketInfo
    {
        public string Key { get; set; }
        public string customerLanguage { get; set; }
        public string customerName { get; set; }
        public string customerId { get; set; }

        public TicketInfo(string key, string language, string name, string id)
        {
            Key = key;
            customerLanguage = language;
            customerName = name;
            customerId = id;
        }
    }
}

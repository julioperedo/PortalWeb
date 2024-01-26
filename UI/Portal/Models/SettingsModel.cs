using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Models
{
    public class MailSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public List<MailAccount> Accounts { get; set; }
        public List<MailTask> Tasks { get; set; }
    }

    public class MailAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }

    public class MailTask
    {
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Name { get; set; }
        public MailRecipient Sender { get; set; }
        public List<MailRecipient> CopiesTo { get; set; }
        public List<MailRecipient> BlindCopiesTo { get; set; }
    }

    public class MailRecipient
    {
        public string Name { get; set; }
        public string EMail { get; set; }
    }

    public class FireBaseSettings
    {
        public string AppKey { get; set; }
        public string SenderId { get; set; }
    }

    public class MicrosoftESDSettings
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string BaseURL { get; set; }
    }

    public class iQuoteSettings
    {
        public string GateKeeperUrl { get; set; }
    }

    public class WebSiteSettings
    {
        public CustomMailSettings WorkMail { get; set; }
        public CustomMailSettings ContactMail { get; set; }
        public CustomMailSettings ClientRMAMail { get; set; }
        public CustomMailSettings DistributorRMAMail { get; set; }
    }

    public class CustomMailSettings
    {
        public MailRecipient Sender { get; set; }
    }

    //public class ReportSettings
    //{
    //    public string ServiceName { get; set; }
    //}
}

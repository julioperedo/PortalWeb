using System.Collections.Generic;

namespace ProviderService.Models
{
    public class PrestaSettings
    {
        public string BaseUrl { get; set; }
        public string Account { get; set; }
    }

    public class LogSettings
    {
        public string Directory { get; set; }
        public List<MailAccount> SendTo { get; set; }
    }

    public class MailSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public List<MailAccount> Accounts { get; set; }
    }

    public class MailAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}

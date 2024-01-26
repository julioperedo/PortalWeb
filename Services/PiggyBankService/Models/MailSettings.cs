namespace PiggyBankService.Models
{
    public class MailSettings
    {
        public string Server { get; set; } = "";
        public int Port { get; set; } = 0;
        public List<MailAccount> Accounts { get; set; } = new List<MailAccount>();

    }

    public class MailAccount
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string User { get; set; } = "";
        public string Password { get; set; } = "";

        public MailAccount() { }
    }
}

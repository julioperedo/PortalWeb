using Microsoft.Extensions.Configuration;
using Portal.Interfaces;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Portal.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender(IConfiguration Configuration)
        {
            _config = Configuration;
        }
        public Task SendEmail(string Subject, string MessageBody, Dictionary<string, string> Destinataries, Dictionary<string, string> Copies = null, Dictionary<string, string> BlindCopies = null, Tuple<string, string> From = null)
        {
            var settingsSection = _config.GetSection("MailSettings");
            var settings = settingsSection.Get<MailSettings>();
            var account = settings.Accounts.Find(x => x.Id == "soporte-santacruz");

            SmtpClient smtpClient = new(settings.Server, settings.Port) { Credentials = new NetworkCredential(account.User, account.Password), DeliveryMethod = SmtpDeliveryMethod.Network, UseDefaultCredentials = false };

            MailMessage message = new();
            MailAddress fromAddress = new(account.EMail, account.Name);
            if (From != null && !string.IsNullOrWhiteSpace(From.Item1) && !string.IsNullOrWhiteSpace(From.Item2))
            {
                fromAddress = new MailAddress(From.Item1, From.Item2);
                message.ReplyToList.Add(fromAddress);
            }
            message.From = fromAddress;
            if (Destinataries?.Count > 0)
            {
                foreach (var item in Destinataries)
                {
                    message.To.Add(new MailAddress(item.Key, item.Value));
                }
            }
            if (Copies?.Count > 0)
            {
                foreach (var item in Copies)
                {
                    message.CC.Add(new MailAddress(item.Key, item.Value));
                }
            }
            if (BlindCopies?.Count > 0)
            {
                foreach (var item in BlindCopies)
                {
                    message.Bcc.Add(new MailAddress(item.Key, item.Value));
                }
            }
            message.Subject = Subject;
            message.IsBodyHtml = true;
            message.Body = MessageBody;

            smtpClient.Send(message);

            message.Dispose();
            smtpClient.Dispose();

            return Task.CompletedTask;
        }

    }
}

using Microsoft.Extensions.Configuration;
using ProviderService.Interfaces;
using ProviderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProviderService.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration Configuration)
        {
            _config = Configuration;
        }

        public Task SendEmail(string Subject, string MessageBody, List<MailAccount> Destinataries, List<MailAccount> Copies = null, List<MailAccount> BlindCopies = null, MailAccount From = null)
        {
            var settingsSection = _config.GetSection("MailSettings");
            var settings = settingsSection.Get<MailSettings>();
            var account = settings.Accounts.Find(x => x.Id == "soporte-santacruz");

            MailMessage message = new();
            MailAddress fromAddress = new(account.EMail, account.Name);
            if (From != null && !string.IsNullOrWhiteSpace(From.EMail) && !string.IsNullOrWhiteSpace(From.EMail))
            {
                fromAddress = new MailAddress(From.EMail, From.Name);
                message.ReplyToList.Add(fromAddress);
            }
            message.From = fromAddress;
            if (Destinataries?.Count > 0)
            {
                Destinataries.ForEach(x=> message.To.Add(new MailAddress(x.EMail, x.Name)));
            }
            if (Copies?.Count > 0)
            {
                Copies.ForEach(x => message.CC.Add(new MailAddress(x.EMail, x.Name)));
            }
            if (BlindCopies?.Count > 0)
            {
                BlindCopies.ForEach(x => message.Bcc.Add(new MailAddress(x.EMail, x.Name)));
            }
            message.Subject = Subject;
            message.IsBodyHtml = true;
            message.Body = MessageBody;

            SmtpClient smtpClient = new(settings.Server, settings.Port) { Credentials = new NetworkCredential(account.User, account.Password), DeliveryMethod = SmtpDeliveryMethod.Network };
            smtpClient.Send(message);
            message.Dispose();
            smtpClient.Dispose();

            return Task.CompletedTask;
        }

    }
}

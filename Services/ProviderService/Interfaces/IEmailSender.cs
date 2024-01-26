using ProviderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProviderService.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmail(string Subject, string MessageBody, List<MailAccount> Destinataries, List<MailAccount> Copies = null, List<MailAccount> BlindCopies = null, MailAccount From = null);
    }
}

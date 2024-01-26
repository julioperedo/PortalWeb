using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmail(string Subject, string MessageBody, Dictionary<string, string> Destinataries, Dictionary<string, string> Copies = null, Dictionary<string, string> BlindCopies = null, Tuple<string, string> From = null);
    }
}

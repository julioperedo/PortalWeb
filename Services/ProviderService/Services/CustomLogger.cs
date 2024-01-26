using Microsoft.Extensions.Configuration;
using ProviderService.Interfaces;
using ProviderService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProviderService.Services
{
    public class CustomLogger : ICustomLogger
    {
        private readonly IConfiguration _config;
        private readonly IEmailSender _sender;

        public CustomLogger(IConfiguration Configuration, IEmailSender sender)
        {
            _config = Configuration;
            _sender = sender;
        }

        public void Log(string Message, string Subdirectory)
        {
            try
            {
                var config = _config.GetSection("LogSettings").Get<LogSettings>();
                string dirName = $"{config.Directory}\\{Subdirectory}", fileName = $"{dirName}\\{Subdirectory}-{DateTime.Now:yyyyMMdd-HHmmss}.txt";
                if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);

                using StreamWriter writer = File.CreateText(fileName);
                writer.WriteAsync(Message);
            }
            catch (Exception) { }
        }

        public async Task ReportErrorByMail(string Message)
        {
            var config = _config.GetSection("LogSettings").Get<LogSettings>();
            List<MailAccount> destinataries = config.SendTo;
            await _sender.SendEmail("Error en el servicio de Proveedores", Message, destinataries);
        }
    }
}

using Microsoft.Extensions.Configuration;
using SmartTeethCare.Core.Interfaces.Services.SecurityModule;
using System.Net;
using System.Net.Mail;

namespace SmartTeethCare.Service.SecurityModule
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_config["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_config["EmailSettings:Port"]),
                Credentials = new NetworkCredential(
                    _config["EmailSettings:SenderEmail"],
                    _config["EmailSettings:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:SenderEmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        
        private async Task<string> GetTemplateAsync(string templateName)
        {
            var path = Path.Combine(
                AppContext.BaseDirectory,
                "EmailTemplates",
                $"{templateName}.html"
            );

            return await File.ReadAllTextAsync(path);
        }

        
        private string ReplacePlaceholders(string body, Dictionary<string, string> data)
        {
            foreach (var item in data)
            {
                body = body.Replace($"{{{item.Key}}}", item.Value);
            }

            return body;
        }

        
        public async Task SendTemplateEmailAsync(
            string toEmail,
            string subject,
            string templateName,
            Dictionary<string, string> data)
        {
            var body = await GetTemplateAsync(templateName);

            body = ReplacePlaceholders(body, data);

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
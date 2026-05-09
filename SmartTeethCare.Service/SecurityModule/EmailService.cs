using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using SmartTeethCare.Core.Interfaces.Services.SecurityModule;

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
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(
                "SmartTeethCare",
                _config["EmailSettings:SenderEmail"]));

            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _config["EmailSettings:Host"],
                int.Parse(_config["EmailSettings:Port"]),
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
               _config["EmailSettings:SmtpLogin"],
               _config["EmailSettings:Password"]);




            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
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

        private async Task<string> GetTemplateAsync(string templateName)
        {
            var path = Path.Combine(
                AppContext.BaseDirectory,
                "EmailTemplates",
                $"{templateName}.html");

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
    }
}

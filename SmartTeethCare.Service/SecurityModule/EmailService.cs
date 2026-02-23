using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SmartTeethCare.Core.DTOs.SecurityModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.SecurityModule;
using System.Net;
using System.Net.Mail;

namespace SmartTeethCare.Service.SecurityModule
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
       
        public EmailService(IConfiguration config )
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

        
    }

}

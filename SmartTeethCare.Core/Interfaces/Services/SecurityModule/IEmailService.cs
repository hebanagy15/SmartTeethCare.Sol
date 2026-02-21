using SmartTeethCare.Core.DTOs.SecurityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.SecurityModule
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        
    }
}

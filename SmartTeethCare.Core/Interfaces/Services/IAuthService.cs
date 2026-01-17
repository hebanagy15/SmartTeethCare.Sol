using Microsoft.AspNetCore.Identity;
using SmartTeethCare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> CreateTokenAsync(User user , UserManager<User> userManager);
    }
}

using Microsoft.AspNetCore.Identity;
using SmartTeethCare.Core.DTOs.SecurityModule;
using SmartTeethCare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.SecurityModule
{
    public interface IAuthService
    {
        Task<string> CreateTokenAsync(User user, UserManager<User> userManager);
        Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
        Task ConfirmEmailAsync(ConfirmEmailDTO dto);
        Task ForgotPasswordAsync(ForgotPasswordDTO dto);
        Task ResetPasswordAsync(ResetPasswordDTO dto);
    }
}

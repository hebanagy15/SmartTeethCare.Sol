using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartTeethCare.Core.DTOs.SecurityModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.SecurityModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;


using System.Text;


namespace SmartTeethCare.Service.SecurityModule
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        public AuthService(IConfiguration configuration , IUnitOfWork unitOfWork, UserManager<User> userManager , IEmailService emailService)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
        }
        public async Task<string> CreateTokenAsync(User user, UserManager<User> userManager)
        {
            // Private Claims (user-defined)
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),

            };

            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

            // Registered Claims (predefined)

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JWT:AccessTokenExpiryInMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            var token = await _unitOfWork.Repository<RefreshToken>()
                .Query()
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (token == null || token.IsRevoked || token.ExpiresOn <= DateTime.UtcNow)
                throw new Exception("Invalid refresh token");

            // Revoke old token
            token.IsRevoked = true;
            token.RevokedOn = DateTime.UtcNow;

            // Generate new refresh token
            var newRefreshToken = GenerateRefreshToken();
            newRefreshToken.UserId = token.UserId;
            await _unitOfWork.Repository<RefreshToken>().AddAsync(newRefreshToken);

            await _unitOfWork.CompleteAsync();
            // Generate new Access Token
            var jwt = await CreateTokenAsync(token.User , _userManager);

            return new AuthResponseDTO
            {
                Token = jwt,
                RefreshToken = newRefreshToken.Token,
                Expiration = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JWT:AccessTokenExpiryInMinutes"]))
            };
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var token = await _unitOfWork.Repository<RefreshToken>()
                .Query()
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (token != null && !token.IsRevoked)
            {
                token.IsRevoked = true;
                token.RevokedOn = DateTime.UtcNow;


                await _unitOfWork.CompleteAsync();
            }
        }


        private RefreshToken GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                ExpiresOn = DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:RefreshTokenExpiryInDays"])),
                IsRevoked = false
            };
        }

        public async Task ConfirmEmailAsync(ConfirmEmailDTO dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user == null)
                throw new Exception("Invalid user.");

            var result = await _userManager.ConfirmEmailAsync(user, dto.Token);

            if (!result.Succeeded)
                throw new Exception("Email confirmation failed.");
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return; // عشان security منقولش الإيميل موجود ولا لا

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var resetLink =
                $"{_configuration["AppSettings:BaseUrl"]}/api/account/reset-password?email={user.Email}&token={encodedToken}";

            await _emailService.SendEmailAsync(
                user.Email,
                "Reset Password",
                $"Click here to reset your password: <a href='{resetLink}'>Reset Password</a>");
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new Exception("Invalid request.");

            var decodedToken = Uri.UnescapeDataString(dto.Token);

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                dto.NewPassword);

            if (!result.Succeeded)
                throw new Exception("Password reset failed.");
        }
    }
}



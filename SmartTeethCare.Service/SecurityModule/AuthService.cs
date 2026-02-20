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

        public AuthService(IConfiguration configuration , IUnitOfWork unitOfWork, UserManager<User> userManager )
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
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
    }
}



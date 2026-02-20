using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.SecurityModule
{
    public class LoginResponseDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Role { get; set; }

        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}

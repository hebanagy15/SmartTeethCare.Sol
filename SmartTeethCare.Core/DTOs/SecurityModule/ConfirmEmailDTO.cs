using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.SecurityModule
{
    public class ConfirmEmailDTO
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}

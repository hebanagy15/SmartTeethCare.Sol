using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }

        public DateTime ExpiresOn { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime? RevokedOn { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }
    }
}

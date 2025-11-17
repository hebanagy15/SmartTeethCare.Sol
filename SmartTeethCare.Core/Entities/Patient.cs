using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class Patient  : BaseEntity
    {
        public string MedicalHistory { get; set; }

        // Relation with User
        public string UserId { get; set; }
        public User User { get; set; }
    }
}

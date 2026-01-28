using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class Patient : BaseEntity
    {
        public string MedicalHistory { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();  
        public ICollection<Review> Reviews { get; set; } = new List<Review>();             // Empty list to avoid null reference
        public ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();


    }
}

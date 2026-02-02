using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class Appointment : BaseEntity
    {
        public int DoctorID { get; set; } 
        public int PatientID { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public Doctor doctor { get; set; }
        public Patient patient { get; set; }
        public Prescription Prescription { get; set; }

    }
}

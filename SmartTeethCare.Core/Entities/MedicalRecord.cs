using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class MedicalRecord : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public int? AppointmentId { get; set; }   // optional
        public Appointment? Appointment { get; set; }

        public string Diagnosis { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}

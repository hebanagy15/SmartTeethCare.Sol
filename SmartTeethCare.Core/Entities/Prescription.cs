using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class Prescription : BaseEntity
    {
        public DateTime Date { get; set; }

        public int PatientId { get; set; }
        public int DoctorId { get; set; }

       
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }  

        public Doctor doctor { get; set; }
        public Patient Patient { get; set; }

        public ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; }

    }
}

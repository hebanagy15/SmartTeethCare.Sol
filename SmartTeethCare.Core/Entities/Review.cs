using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class Review : BaseEntity

    {
        public int DentistID { get; set; }
        public int PatientID { get; set; }
        public int AppointmentID { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Doctor doctor { get; set; }
        public Patient Patient { get; set; }
    }
}

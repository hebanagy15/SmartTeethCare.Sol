using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.PatientModule
{
    public class AppointmentDetailsDTO
    {
        
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }

    }
}


using SmartTeethCare.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.DoctorModule
{
    public class DoctorAppointmentDto
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public DateTime CreatedAt { get; set; }
        public AppointmentStatus Status { get; set; }
    }

}

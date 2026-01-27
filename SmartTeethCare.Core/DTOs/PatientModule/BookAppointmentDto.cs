using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.PatientModule
{
    public class BookAppointmentDto
    {
        public int DentistId { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}

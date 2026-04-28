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
        public DateTime AppointmentDate { get; set; }   //  يوم الكشف
        public TimeSpan StartTime { get; set; }          //  الساعة كام
        public TimeSpan EndTime { get; set; }            //  بيخلص إمتى
        public AppointmentStatus Status { get; set; }
    }

}

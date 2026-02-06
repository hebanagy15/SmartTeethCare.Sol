using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.DoctorModule
{
    
        public class DoctorDashboardDto
        {
            public int TodayAppointments { get; set; }
            public int PendingAppointments { get; set; }
            public int CompletedAppointments { get; set; }
        
        }


}

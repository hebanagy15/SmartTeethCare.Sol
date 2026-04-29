using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.AdminModule
{
    public class UpdateDoctorDto
    {
        public string? DisplayName { get; set; }
        public int Salary { get; set; }
        public int WorkingHours { get; set; }
        public int? SpecialityID { get; set; }
    }

}

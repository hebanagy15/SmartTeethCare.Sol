using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.AdminModule
{
    public class CreateDoctorDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public int Salary { get; set; }
        public int WorkingHours { get; set; }
        public DateTime HiringDate { get; set; }

        public int? SpecialityID { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
    }

}

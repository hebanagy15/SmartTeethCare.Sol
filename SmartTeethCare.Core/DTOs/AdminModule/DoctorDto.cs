using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.AdminModule
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Salary { get; set; }
        public int WorkingHours { get; set; }
        public DateTime HiringDate { get; set; }
        public string? SpecialityName { get; set; }
    }
}


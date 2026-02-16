using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.Lookup
{
    public class DoctorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public int? SpecializationId { get; set; } 
        public string? SpecializationName { get; set; } 
    }
}

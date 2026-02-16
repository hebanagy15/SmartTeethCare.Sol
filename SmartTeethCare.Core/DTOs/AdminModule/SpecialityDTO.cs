using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.AdminModule
{
    public class SpecialityDTO
    {
        public int? Id { get; set; } // optional for create
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

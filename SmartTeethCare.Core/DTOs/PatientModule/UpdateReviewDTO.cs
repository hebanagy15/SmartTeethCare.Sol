using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.PatientModule
{
    public class UpdateReviewDTO
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

}

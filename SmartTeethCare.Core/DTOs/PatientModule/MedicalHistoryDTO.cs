using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.PatientModule
{
    public class MedicalHistoryDTO
    {
        public int Id { get; set; }

        public string ConditionName { get; set; }
        public string? Notes { get; set; }
        public bool IsChronic { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DoctorName { get; set; }   
    }
}

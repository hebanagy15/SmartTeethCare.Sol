using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class MedicalHistory : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public string ConditionName { get; set; }   // Diabetes, Hypertension, Heart Disease
        public string? Notes { get; set; }           // any extra details
        public bool IsChronic { get; set; }           
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.MedicalRecordModule
{
    public class MedicalRecordDto
    {
        public int Id { get; set; }

        public string PatientName { get; set; }
        public string DoctorName { get; set; }

        public string Diagnosis { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

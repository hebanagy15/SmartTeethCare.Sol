using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.PatientModule
{
    public class PrescriptionDetailsDTO
    {
        public int PrescriptionId { get; set; }
        public DateTime Date { get; set; }

        public string DoctorName { get; set; }
        public string PatientName { get; set; }

        public List<string> Medicines { get; set; } = new();
    }

}

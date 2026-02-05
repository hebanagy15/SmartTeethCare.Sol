using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.DoctorModule
{
    public class PrescriptionMedicineDetailsDto
    {
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public int DurationInDays { get; set; }
        public int Quantity { get; set; }
        public string Instructions { get; set; }
    }

}

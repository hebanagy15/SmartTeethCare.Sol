using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.DoctorModule
{

    public class CreatePrescriptionDto
    {
        public int AppointmentId { get; set; }
        public List<PrescriptionMedicineDto> Medicines { get; set; } = new();
    }

}



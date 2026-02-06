using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class PrescriptionMedicine : BaseEntity
    {
        public int PrescriptionID { get; set; }
        public int MedicineID { get; set; }

        public string Dosage { get; set; }        // 10mg
        public string Frequency { get; set; }     // Twice daily
        public int DurationInDays { get; set; }   // 30
        public int Quantity { get; set; }          // 30 tablets

        public Prescription Prescription { get; set; }
        public Medicine Medicine { get; set; }
        public string Instructions { get; set; }    // After meals

    }

}

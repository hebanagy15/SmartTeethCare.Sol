using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class PrescriptionMedicine
    {

        public int PrescriptionID { get; set; }
        public int MedicineID { get; set; }

       
        public Prescription Prescription { get; set; }
        public Medicine Medicine { get; set; }
    }
}

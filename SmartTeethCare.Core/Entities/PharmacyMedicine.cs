using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class PharmacyMedicine : BaseEntity
    {
        // Composite Key: PharmacyID + MedicineID
        public int PharmacyID { get; set; }
        public int MedicineID { get; set; }


        public int StockQuantity { get; set; }

        
        public Pharmacy Pharmacy { get; set; }
        public Medicine Medicine { get; set; }
    }
}

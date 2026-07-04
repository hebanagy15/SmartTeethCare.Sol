using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.Pharmacy
{
    public class CreatePharmacyMedicineDto
    {
        public int PharmacyID { get; set; }

        public int MedicineID { get; set; }

        public int StockQuantity { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.Pharmacy
{
    public class PharmacyMedicineDto
    {
        public int Id { get; set; }

        public int PharmacyID { get; set; }

        public string PharmacyName { get; set; }

        public int MedicineID { get; set; }

        public string MedicineName { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
    }

}

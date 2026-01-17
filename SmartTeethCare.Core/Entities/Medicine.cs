using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class Medicine : BaseEntity
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }


        public ICollection<PrescriptionMedicine>? PrescriptionMedicines { get; set; }
        public ICollection<PharmacyMedicine>? PharmacyMedicines { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.Pharmacy
{
    public class AvailablePharmacyDto
    {
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; } = null!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public double DistanceInKm { get; set; }
    }
}

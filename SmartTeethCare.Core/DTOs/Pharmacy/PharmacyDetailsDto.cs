using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.Pharmacy
{
    public class PharmacyDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string WorkingHours { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}

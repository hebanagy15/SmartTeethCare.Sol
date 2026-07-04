using SmartTeethCare.Core.DTOs.Pharmacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.Pharmacy
{
    public interface IPharmacySearchService
    {
        Task<IEnumerable<AvailablePharmacyDto>> GetAvailablePharmaciesAsync(int medicineId,double patientLatitude,double patientLongitude);
    }
}

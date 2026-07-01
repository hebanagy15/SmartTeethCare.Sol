using SmartTeethCare.Core.DTOs.Pharmacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.Pharmacy
{
    public interface IPharmacyService
    {
        Task<IEnumerable<PharmacyDto>> GetAllAsync();

        Task<PharmacyDetailsDto> GetByIdAsync(int id);

        Task AddAsync(CreatePharmacyDto dto);

        Task UpdateAsync(UpdatePharmacyDto dto);

        Task DeleteAsync(int id);
    }
}

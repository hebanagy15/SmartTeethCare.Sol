using SmartTeethCare.Core.DTOs.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.Lookup
{
    public interface ILookupService
    {
        Task<IEnumerable<DoctorDTO>> GetDoctorsAsync();
        Task<IEnumerable<SpecializationDTO>> GetSpecializationsAsync();
        Task<IEnumerable<DoctorDTO>> GetDoctorsBySpecialityAsync(int specialityId);

    }
}

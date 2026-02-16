using SmartTeethCare.Core.DTOs.AdminModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.AdminModule
{
    public interface IAdminSpecialityService
    {
        Task<IEnumerable<SpecialityDTO>> GetAllAsync();
        Task<SpecialityDTO> GetByIdAsync(int id);
        Task<SpecialityDTO> CreateAsync(SpecialityDTO dto, ClaimsPrincipal user);
        Task<bool> UpdateAsync(SpecialityDTO dto, ClaimsPrincipal user);
        Task<bool> DeleteAsync(int id, ClaimsPrincipal user);
    }
}

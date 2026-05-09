using SmartTeethCare.Core.DTOs.PatientModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.PatientModule
{
    public interface IPatientProfileService
    {
        Task<PatientProfileDto> GetProfileAsync(string userId);

        Task UpdateProfileAsync(string userId, UpdatePatientProfileDto dto);
    }
}

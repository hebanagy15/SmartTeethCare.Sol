using SmartTeethCare.Core.DTOs.MedicalRecordModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.MedicalRecordModule
{
    public interface IMedicalRecordService
    {
        Task AddAsync(CreateMedicalRecordDto dto, string userId);
        Task<IEnumerable<MedicalRecordDto>> GetMyMedicalRecords(string userId); // For patients to get their own medical records
        Task<IEnumerable<MedicalRecordDto>> GetMyCreatedMedicalRecords(string userId); // For doctors to get the medical records they created
        Task<MedicalRecordDto> GetDetailsAsync(int id, ClaimsPrincipal user);
    }
}

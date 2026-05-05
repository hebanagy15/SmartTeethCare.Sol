using SmartTeethCare.Core.DTOs.MedicalRecordModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.MedicalRecordModule
{
    public interface IMedicalRecordService
    {
        Task AddAsync(CreateMedicalRecordDto dto);
        Task<IEnumerable<MedicalRecordDto>> GetByPatientId(int patientId);
        Task<MedicalRecordDto> GetById(int id);
    }
}

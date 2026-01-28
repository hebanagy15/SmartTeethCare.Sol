using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System.Security.Claims;

namespace SmartTeethCare.Service.PatientModule
{
    public class PatientMedicalHistoryService : IPatientMedicalHistoryService
    {
        private readonly IUnitOfWork _uow;

        public PatientMedicalHistoryService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }
        public async Task<IEnumerable<MedicalHistoryDTO>> GetMyMedicalHistoryAsync(ClaimsPrincipal user)
        {
            // Extract PatientId from token
            var patientId = int.Parse(
                user.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            // Get medical history for this patient
            var histories = await _uow.Repository<MedicalHistory>().FindAsync(h => h.PatientId == patientId,include: q => q.Include(x => x.Doctor).ThenInclude(d => d.User));


            // Map Entity → DTO
            return histories.Select(h => new MedicalHistoryDTO
            {
                Id = h.Id,
                ConditionName = h.ConditionName,
                Notes = h.Notes,
                IsChronic = h.IsChronic,
                CreatedAt = h.CreatedAt,
                DoctorName = h.Doctor?.User?.UserName ?? "Unknown Doctor"

            });
        }

    }
}

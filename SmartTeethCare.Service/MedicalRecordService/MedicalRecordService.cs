using SmartTeethCare.Core.DTOs.MedicalRecordModule;
using SmartTeethCare.Core.Interfaces.Services.MedicalRecordModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Service.MedicalRecordModule
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicalRecordService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(CreateMedicalRecordDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var record = new MedicalRecord
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentId = dto.AppointmentId,
                Diagnosis = dto.Diagnosis ?? string.Empty,
                Notes = dto.Notes ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<MedicalRecord>().AddAsync(record);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetByPatientId(int patientId)
        {
            var records = await _unitOfWork.Repository<MedicalRecord>()
                .FindAsync(r => r.PatientId == patientId);

            if (records == null || !records.Any())
                return Enumerable.Empty<MedicalRecordDto>();

            return records.Select(r => new MedicalRecordDto
            {
                Id = r.Id,
                Diagnosis = r.Diagnosis ?? string.Empty,
                Notes = r.Notes ?? string.Empty,
                CreatedAt = r.CreatedAt
            });
        }

        public async Task<MedicalRecordDto?> GetById(int id)
        {
            var record = await _unitOfWork.Repository<MedicalRecord>()
                .GetByIdAsync(id);

            if (record == null)
                return null;

            return new MedicalRecordDto
            {
                Id = record.Id,
                Diagnosis = record.Diagnosis ?? string.Empty,
                Notes = record.Notes ?? string.Empty,
                CreatedAt = record.CreatedAt
            };
        }
    }
}
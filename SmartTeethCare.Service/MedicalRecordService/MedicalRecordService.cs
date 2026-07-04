using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.MedicalRecordModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.MedicalRecordModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System.Security.Claims;

namespace SmartTeethCare.Service.MedicalRecordModule
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicalRecordService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(CreateMedicalRecordDto dto, string userId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // هات الدكتور من الـ JWT
            var doctors = await _unitOfWork.Repository<Doctor>()
                .FindAsync(d => d.UserId == userId);

            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
                throw new Exception("Doctor not found");

            // هات الموعد
            var appointment = await _unitOfWork.Repository<Appointment>()
                .GetByIdAsync(dto.AppointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found");

            // تأكد إن الموعد يخص الدكتور اللي عامل Login
            if (appointment.DoctorID != doctor.Id)
                throw new Exception("You are not authorized to create a medical record for this appointment.");

            var record = new MedicalRecord
            {
                PatientId = appointment.PatientID,
                DoctorId = appointment.DoctorID,
                AppointmentId = appointment.Id,
                Diagnosis = dto.Diagnosis ?? string.Empty,
                Notes = dto.Notes ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<MedicalRecord>().AddAsync(record);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetMyMedicalRecords(string userId)
        {
            var patients = await _unitOfWork.Repository<Patient>()
                .FindAsync(p => p.UserId == userId);

            var patient = patients.FirstOrDefault();

            if (patient == null)
                throw new Exception("Patient not found");

            var records = await _unitOfWork.Repository<MedicalRecord>()
                          .FindAsync(
                                r => r.PatientId == patient.Id,
                                include: q => q
                                    .Include(r => r.Doctor)
                                        .ThenInclude(d => d.User)
                                    .Include(r => r.Patient)
                                        .ThenInclude(p => p.User)
    );
            return records.Select(r => new MedicalRecordDto
            {
                Id = r.Id,
                DoctorName = string.IsNullOrWhiteSpace(r.Doctor?.User?.DisplayName)
                                    ? r.Doctor?.User?.UserName
                                    : r.Doctor.User.DisplayName,

                PatientName = string.IsNullOrWhiteSpace(r.Patient?.User?.DisplayName)
                                    ? r.Patient?.User?.UserName
                                    : r.Patient.User.DisplayName,
                Diagnosis = r.Diagnosis ?? string.Empty,
                Notes = r.Notes ?? string.Empty,
                CreatedAt = r.CreatedAt
            });
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetMyCreatedMedicalRecords(string userId)
        {
            var doctor = (await _unitOfWork.Repository<Doctor>()
                .FindAsync(d => d.UserId == userId))
                .FirstOrDefault();

            if (doctor == null)
                throw new Exception("Doctor not found");

            var records = await _unitOfWork.Repository<MedicalRecord>()
                .FindAsync(
                    r => r.DoctorId == doctor.Id,
                    include: q => q
                        .Include(r => r.Doctor)
                            .ThenInclude(d => d.User)
                        .Include(r => r.Patient)
                            .ThenInclude(p => p.User));

            return records.Select(r => new MedicalRecordDto
            {
                Id = r.Id,
                DoctorName = string.IsNullOrWhiteSpace(r.Doctor?.User?.DisplayName)
                    ? r.Doctor?.User?.UserName
                    : r.Doctor.User.DisplayName,

                PatientName = string.IsNullOrWhiteSpace(r.Patient?.User?.DisplayName)
                    ? r.Patient?.User?.UserName
                    : r.Patient.User.DisplayName,

                Diagnosis = r.Diagnosis ?? string.Empty,
                Notes = r.Notes ?? string.Empty,
                CreatedAt = r.CreatedAt
            });
        }


        public async Task<MedicalRecordDto?> GetDetailsAsync(int id, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new Exception("User not authenticated");

            var records = await _unitOfWork.Repository<MedicalRecord>()
                .FindAsync(
                    r => r.Id == id,
                    include: q => q
                        .Include(r => r.Doctor)
                            .ThenInclude(d => d.User)
                        .Include(r => r.Patient)
                            .ThenInclude(p => p.User)
                );

            var record = records.FirstOrDefault();

            if (record == null)
                return null;
            if (user.IsInRole("Doctor"))
            {
                var doctor = (await _unitOfWork.Repository<Doctor>()
                    .FindAsync(d => d.UserId == userId))
                    .FirstOrDefault();

                if (doctor == null)
                    throw new Exception("Doctor not found");

                if (record.DoctorId != doctor.Id)
                    throw new UnauthorizedAccessException("You are not authorized to view this medical record.");
            }
            else if (user.IsInRole("Patient"))
            {
                var patient = (await _unitOfWork.Repository<Patient>()
                    .FindAsync(p => p.UserId == userId))
                    .FirstOrDefault();

                if (patient == null)
                    throw new Exception("Patient not found");

                if (record.PatientId != patient.Id)
                    throw new UnauthorizedAccessException("You are not authorized to view this medical record.");
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid role.");
            }
            return new MedicalRecordDto
            {
                Id = record.Id,

                PatientName = string.IsNullOrWhiteSpace(record.Patient?.User?.DisplayName)
                    ? record.Patient?.User?.UserName
                    : record.Patient.User.DisplayName,

                DoctorName = string.IsNullOrWhiteSpace(record.Doctor?.User?.DisplayName)
                    ? record.Doctor?.User?.UserName
                    : record.Doctor.User.DisplayName,

                Diagnosis = record.Diagnosis ?? string.Empty,
                Notes = record.Notes ?? string.Empty,
                CreatedAt = record.CreatedAt
            };
        }
    }
}
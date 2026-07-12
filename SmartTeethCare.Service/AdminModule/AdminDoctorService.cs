using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.DTOs.DoctorModule;
using SmartTeethCare.Core.DTOs.MedicalRecordModule;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using SmartTeethCare.Core.Interfaces.Services.NotificationService;

namespace SmartTeethCare.Service.AdminModule
{
    public class AdminDoctorService : IAdminDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly INotificationService _notificationService;

        public AdminDoctorService(
     IUnitOfWork unitOfWork,
     UserManager<User> userManager,
     INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _unitOfWork
                .Repository<Doctor>()
                .FindAsync(include: q => q
                    .Include(d => d.User)
                    .Include(d => d.Speciality));

            return doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                UserId = d.UserId,
                FullName = d.User.DisplayName ?? d.User.UserName,
                Email = d.User.Email,
                Salary = d.Salary,
                WorkingHours = d.WorkingHours,
                HiringDate = d.HiringDate,
                SpecialityName = d.Speciality?.Name,
                IsActive = d.IsActive
            });
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(int id)
        {
            var doctor = (await _unitOfWork
                .Repository<Doctor>()
                .FindAsync(d => d.Id == id,
                    q => q.Include(d => d.User)
                          .Include(d => d.Speciality)))
                .FirstOrDefault();

            if (doctor == null)
                return null;

            return new DoctorDto
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                FullName = doctor.User.DisplayName ?? doctor.User.UserName,
                Email = doctor.User.Email,
                Salary = doctor.Salary,
                WorkingHours = doctor.WorkingHours,
                HiringDate = doctor.HiringDate,
                SpecialityName = doctor.Speciality?.Name,
                IsActive = doctor.IsActive
            };
        }

        public async Task AddDoctorAsync(CreateDoctorDto dto)
        {
            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                DisplayName = dto.FullName.StartsWith("Dr.")
                    ? dto.FullName
                    : $"Dr. {dto.FullName}",
                Address = dto.Address ?? "Not Provided",
                Gender = dto.Gender ?? "Not Specified",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Doctor");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            var doctor = new Doctor
            {
                UserId = user.Id,
                Salary = dto.Salary,
                WorkingHours = dto.WorkingHours,
                HiringDate = dto.HiringDate,
                SpecialtyID = dto.SpecialityID,
                IsActive = true
            };

            await _unitOfWork.Repository<Doctor>().AddAsync(doctor);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateDoctorAsync(int id, UpdateDoctorDto dto)
        {
            var doctor = await _unitOfWork.Repository<Doctor>()
                .FindAsync(d => d.Id == id,
                    include: q => q.Include(d => d.User));

            var doctorEntity = doctor.FirstOrDefault();

            if (doctorEntity == null)
                throw new Exception("Doctor not found");

            doctorEntity.Salary = dto.Salary;
            doctorEntity.WorkingHours = dto.WorkingHours;
            doctorEntity.SpecialtyID = dto.SpecialityID;
            doctorEntity.ImageUrl = dto.ImageUrl;
            doctorEntity.ConsultationFee = dto.ConsultationFee;
            doctorEntity.YearsOfExperience = dto.YearsOfExperience;

            if (!string.IsNullOrEmpty(dto.DisplayName))
            {
                doctorEntity.User.DisplayName = dto.DisplayName.StartsWith("Dr.")
                    ? dto.DisplayName
                    : $"Dr. {dto.DisplayName}";
            }

            await _unitOfWork.Repository<Doctor>().UpdateAsync(doctorEntity);
            await _unitOfWork.CompleteAsync();
        }
        public async Task DeleteDoctorAsync(int id)
        {
            var doctor = await _unitOfWork.Repository<Doctor>()
                .FindAsync(
                    d => d.Id == id,
                    include: q => q.Include(d => d.Schedules));

            var doctorEntity = doctor.FirstOrDefault();

            if (doctorEntity == null)
                throw new Exception("Doctor not found");

            // لو عنده مواعيد مستقبلية مينفعش يتعمله Disable
            var hasFutureAppointments = (await _unitOfWork.Repository<Appointment>()
                .FindAsync(a =>
                    a.DoctorID == id &&
                    
                    a.Status != AppointmentStatus.Cancelled &&
                    a.Status != AppointmentStatus.Completed &&
                    a.Status != AppointmentStatus.Rejected))
                .Any();

            if (hasFutureAppointments)
                throw new Exception("Doctor has upcoming appointments. Wait until all appointments are finished.");

            // Disable الحساب
            var user = await _userManager.FindByIdAsync(doctorEntity.UserId);

            if (user != null)
            {
                user.LockoutEnd = DateTimeOffset.MaxValue;
                await _userManager.UpdateAsync(user);
            }

            // تغيير حالة الدكتور
            doctorEntity.IsActive = false;

            // حذف الـ Schedule
            foreach (var schedule in doctorEntity.Schedules.ToList())
            {
                await _unitOfWork.Repository<DoctorSchedule>().DeleteAsync(schedule);
            }

            await _unitOfWork.Repository<Doctor>().UpdateAsync(doctorEntity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task ToggleDoctorStatusAsync(int id, bool cancelAppointments = false)
        {
            var doctor = await _unitOfWork.Repository<Doctor>()
                .FindAsync(
                    d => d.Id == id,
                    include: q => q.Include(d => d.Schedules));

            var doctorEntity = doctor.FirstOrDefault();

            if (doctorEntity == null)
                throw new Exception("Doctor not found");

            var user = await _userManager.FindByIdAsync(doctorEntity.UserId);

            if (user == null)
                throw new Exception("User not found");

            // Enable
            if (user.LockoutEnd != null &&
                user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                user.LockoutEnd = null;
                doctorEntity.IsActive = true;
            }
            else
            {
                // جيب كل المواعيد المستقبلية
                var futureAppointments = (await _unitOfWork.Repository<Appointment>()
                    .FindAsync(a =>
                        a.DoctorID == id &&
                       
                        a.Status != AppointmentStatus.Completed &&
                        a.Status != AppointmentStatus.Cancelled &&
                        a.Status != AppointmentStatus.Rejected))
                    .ToList();

                // لو فيه مواعيد ولسه الأدمن مأكدش
                if (futureAppointments.Any() && !cancelAppointments)
                {
                    throw new Exception(
                        $"Doctor has {futureAppointments.Count} upcoming appointments. " +
                        $"Call again with cancelAppointments=true to cancel them and disable the doctor.");
                }

                // إلغاء المواعيد لو الأدمن أكد
                if (cancelAppointments)
                {
                    foreach (var appointment in futureAppointments)
                    {
                        appointment.Status = AppointmentStatus.Cancelled;

                        var patient = await _unitOfWork.Repository<Patient>()
                            .GetByIdAsync(appointment.PatientID);

                        // if (patient != null)
                        // {
                        //     await _notificationService.CreateAsync(
                        //         patient.UserId,
                        //         "Appointment Cancelled",
                        //         $"Your appointment on {appointment.Date:dd/MM/yyyy} at {appointment.StartTime} has been cancelled because your doctor is no longer available.",
                        //         true
                        //     );
                        // }
                    }
                }

                // Disable الحساب
                user.LockoutEnd = DateTimeOffset.MaxValue;
                doctorEntity.IsActive = false;

                // حذف الـ Schedule
                foreach (var schedule in doctorEntity.Schedules.ToList())
                {
                    await _unitOfWork.Repository<DoctorSchedule>()
                        .DeleteAsync(schedule);
                }
            }

            await _userManager.UpdateAsync(user);
            await _unitOfWork.Repository<Doctor>().UpdateAsync(doctorEntity);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<IEnumerable<MedicalRecordDto>> GetDoctorMedicalRecordsAsync(int doctorId)
        {
            var doctor = await _unitOfWork.Repository<Doctor>()
                .GetByIdAsync(doctorId);

            if (doctor == null)
                throw new Exception("Doctor not found");

            var records = await _unitOfWork.Repository<MedicalRecord>()
                .FindAsync(
                    r => r.DoctorId == doctorId,
                    include: q => q
                        .Include(r => r.Doctor)
                            .ThenInclude(d => d.User)
                        .Include(r => r.Patient)
                            .ThenInclude(p => p.User));

            return records.Select(r => new MedicalRecordDto
            {
                Id = r.Id,
                DoctorName = string.IsNullOrWhiteSpace(r.Doctor.User.DisplayName)
                    ? r.Doctor.User.UserName
                    : r.Doctor.User.DisplayName,

                PatientName = string.IsNullOrWhiteSpace(r.Patient.User.DisplayName)
                    ? r.Patient.User.UserName
                    : r.Patient.User.DisplayName,

                Diagnosis = r.Diagnosis ?? string.Empty,
                Notes = r.Notes ?? string.Empty,
                CreatedAt = r.CreatedAt
            });
        }

        public async Task<IEnumerable<PrescriptionDetailsDTO>> GetDoctorPrescriptionsAsync(int doctorId)
        {
            var doctor = await _unitOfWork.Repository<Doctor>()
                .GetByIdAsync(doctorId);

            if (doctor == null)
                throw new Exception("Doctor not found");

            var prescriptions = await _unitOfWork.Repository<Prescription>()
                .FindAsync(
                    p => p.DoctorId == doctorId,
                    include: q => q
                        .Include(p => p.doctor)
                            .ThenInclude(d => d.User)
                        .Include(p => p.Patient)
                            .ThenInclude(p => p.User)
                        .Include(p => p.PrescriptionMedicines)
                            .ThenInclude(pm => pm.Medicine));

            return prescriptions.Select(p => new PrescriptionDetailsDTO
            {
                PrescriptionId = p.Id,
                Date = p.Date,

                DoctorName = string.IsNullOrWhiteSpace(p.doctor.User.DisplayName)
                    ? p.doctor.User.UserName
                    : p.doctor.User.DisplayName,

                PatientName = string.IsNullOrWhiteSpace(p.Patient.User.DisplayName)
                    ? p.Patient.User.UserName
                    : p.Patient.User.DisplayName,

                Medicines = p.PrescriptionMedicines
                    .Select(pm => new PrescriptionMedicineDetailsDto
                    {
                        MedicineName = pm.Medicine.Name,
                        Dosage = pm.Dosage,
                        Frequency = pm.Frequency,
                        DurationInDays = pm.DurationInDays,
                        Quantity = pm.Quantity,
                        Instructions = pm.Instructions
                    }).ToList()
            });
        }
    }

}
        
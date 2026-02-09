using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;

namespace SmartTeethCare.Services.AppointmentModule
{
    public class AdminAppointmentService : IAdminAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminAppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AppointmentResponseDTO> CreateAppointmentByAdminAsync(CreateAppointmentByAdminDTO dto)
        {
            // 1️⃣ check doctor exists
            var doctor = await _unitOfWork.Repository<Doctor>()
                .GetByIdAsync(dto.DoctorID);

            if (doctor == null)
                throw new Exception("Doctor not found");

            // 2️⃣ check patient exists
            var patient = await _unitOfWork.Repository<Patient>()
                .GetByIdAsync(dto.PatientID);

            if (patient == null)
                throw new Exception("Patient not found");

            // 3️⃣ create appointment
            var appointment = new Appointment
            {
                DoctorID = dto.DoctorID,
                PatientID = dto.PatientID,
                Date = dto.Date,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,

                Status = AppointmentStatus.Pending,
                PaymentStatus = dto.PaymentStatus,

                CreatedByAdmin = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Appointment>()
                .AddAsync(appointment);

            await _unitOfWork.CompleteAsync();

            // 4️⃣ return response
            return new AppointmentResponseDTO
            {
                Id = appointment.Id,
                DoctorID = appointment.DoctorID,
                PatientID = appointment.PatientID,
                Date = appointment.Date,
                Amount = appointment.Amount,
                Status = appointment.Status,
                PaymentMethod = appointment.PaymentMethod,
                PaymentStatus = appointment.PaymentStatus,
                CreatedByAdmin = appointment.CreatedByAdmin,
                CreatedAt = appointment.CreatedAt
            };
        }
    }
}

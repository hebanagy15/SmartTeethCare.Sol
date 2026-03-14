using SmartTeethCare.Core.DTOs.DoctorModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Core.Interfaces.Services.DoctorModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace SmartTeethCare.Service.DoctorModule
{
    public class DoctorAppointmentService : IDoctorAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorAppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private async Task<int> GetDoctorIdAsync(string userId)
        {
            var doctor = (await _unitOfWork.Repository<Doctor>()
                .FindAsync(d => d.UserId == userId))
                .FirstOrDefault();

            if (doctor == null)
                throw new UnauthorizedAccessException("Doctor not found");

            return doctor.Id;
        }

        public async Task<List<DoctorAppointmentDto>> GetDoctorAppointmentsAsync(
            string userId,
            AppointmentStatus? status,
            DateTime? date,
            string? search)
        {
            var doctorId = await GetDoctorIdAsync(userId);

            var appointments = await _unitOfWork.Repository<Appointment>()
                .FindAsync(
                    a => a.DoctorID == doctorId,
                    q => q
                        .Include(a => a.patient)
                        .ThenInclude(p => p.User)
                );

            if (status.HasValue)
                appointments = appointments
                    .Where(a => a.Status == status)
                    .ToList();

            if (date.HasValue)
                appointments = appointments
                    .Where(a => a.CreatedAt.Date == date.Value.Date)
                    .ToList();

            if (!string.IsNullOrEmpty(search))
            {
                appointments = appointments
                    .Where(a =>
                        a.patient != null &&
                        a.patient.User != null &&
                        a.patient.User.Email.Contains(search))
                    .ToList();
            }

            return appointments.Select(a => new DoctorAppointmentDto
            {
                AppointmentId = a.Id,
                PatientName = a.patient?.User?.Email ?? "Unknown",
                CreatedAt = a.CreatedAt,
                Status = a.Status
            }).ToList();
        }

        public async Task CompleteAppointmentAsync(int appointmentId, string userId)
        {
            var doctorId = await GetDoctorIdAsync(userId);

            var appointment = await _unitOfWork.Repository<Appointment>()
                .GetByIdAsync(appointmentId);

            if (appointment == null || appointment.DoctorID != doctorId)
                throw new KeyNotFoundException("Appointment not found");

            appointment.Status = AppointmentStatus.Completed;

            await _unitOfWork.CompleteAsync();
        }

        public async Task CancelAppointmentAsync(int appointmentId, string userId)
        {
            var doctorId = await GetDoctorIdAsync(userId);

            var appointment = await _unitOfWork.Repository<Appointment>()
                .GetByIdAsync(appointmentId);

            if (appointment == null || appointment.DoctorID != doctorId)
                throw new KeyNotFoundException("Appointment not found");

            appointment.Status = AppointmentStatus.Rejected;

            await _unitOfWork.CompleteAsync();
        }

        public async Task<DoctorDashboardDto> GetDoctorDashboardAsync(string userId)
        {
            var doctorId = await GetDoctorIdAsync(userId);

            var today = DateTime.Today;

            var appointments = await _unitOfWork.Repository<Appointment>()
                .FindAsync(a => a.DoctorID == doctorId);

            return new DoctorDashboardDto
            {
                TodayAppointments = appointments
                    .Count(a => a.CreatedAt.Date == today),

                PendingAppointments = appointments
                    .Count(a => a.Status == AppointmentStatus.Pending),

                CompletedAppointments = appointments
                    .Count(a => a.Status == AppointmentStatus.Completed)
            };
        }
    }
}
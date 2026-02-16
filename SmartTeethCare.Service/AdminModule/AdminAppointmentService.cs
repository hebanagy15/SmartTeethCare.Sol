using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTeethCare.Services.AppointmentModule
{
    public class AdminAppointmentService : IAdminAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminAppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ---------------- Create ----------------
        public async Task<AppointmentResponseDTO> CreateAppointmentByAdminAsync(CreateAppointmentByAdminDTO dto)
        {
            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(dto.DoctorID);
            if (doctor == null) throw new Exception("Doctor not found");

            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(dto.PatientID);
            if (patient == null) throw new Exception("Patient not found");

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

            await _unitOfWork.Repository<Appointment>().AddAsync(appointment);
            await _unitOfWork.CompleteAsync();

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

        // ---------------- Get All ----------------
        public async Task<IEnumerable<AppointmentDetailsDTO>> GetAllAppointmentsAsync()
        {
            var appointments = await _unitOfWork.Repository<Appointment>()
                .FindAsync(include: q => q
                    .Include(a => a.doctor).ThenInclude(d => d.User)
                    .Include(a => a.patient).ThenInclude(p => p.User));

            return appointments.Select(a => new AppointmentDetailsDTO
            {
                Id = a.Id,
                DoctorId = a.DoctorID,
                DoctorName = a.doctor.User.UserName,
                PatientId = a.PatientID,
                PatientName = a.patient.User.UserName,
                Amount = a.Amount,
                Date = a.Date,
                CreatedAt = a.CreatedAt,
                Status = a.Status.ToString()
            });
        }

        // ---------------- Get By Id ----------------
        public async Task<AppointmentDetailsDTO?> GetAppointmentByIdAsync(int id)
        {
            var a = await _unitOfWork.Repository<Appointment>()
                .FindAsync(q => q.Id == id,
                    include: inc => inc
                        .Include(x => x.doctor).ThenInclude(d => d.User)
                        .Include(x => x.patient).ThenInclude(p => p.User));

            var appointment = a.FirstOrDefault();
            if (appointment == null) return null;

            return new AppointmentDetailsDTO
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorID,
                DoctorName = appointment.doctor.User.UserName,
                PatientId = appointment.PatientID,
                PatientName = appointment.patient.User.UserName,
                Amount = appointment.Amount,
                Date = appointment.Date,
                CreatedAt = appointment.CreatedAt,
                Status = appointment.Status.ToString()
            };
        }

        // ---------------- Cancel ----------------
        public async Task CancelAppointmentAsync(int id)
        {
            var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(id);
            if (appointment == null) throw new Exception("Appointment not found");

            if (appointment.Status == AppointmentStatus.Completed)
                throw new Exception("Cannot cancel completed appointment");

            appointment.Status = AppointmentStatus.Cancelled;

            await _unitOfWork.Repository<Appointment>().UpdateAsync(appointment);
            await _unitOfWork.CompleteAsync();
        }

        // ---------------- Change Status ----------------
        public async Task ChangeAppointmentStatusAsync(int id, AppointmentStatus newStatus)
        {
            var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(id);
            if (appointment == null) throw new Exception("Appointment not found");

            if (appointment.Status == AppointmentStatus.Completed || appointment.Status == AppointmentStatus.Cancelled)
                throw new Exception("Cannot modify completed or cancelled appointment");

            appointment.Status = newStatus;

            await _unitOfWork.Repository<Appointment>().UpdateAsync(appointment);
            await _unitOfWork.CompleteAsync();
        }
    }
}

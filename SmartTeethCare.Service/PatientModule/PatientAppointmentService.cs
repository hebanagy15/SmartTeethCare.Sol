using Hangfire;
using Microsoft.AspNetCore.Identity;
using SmartTeethCare.Core.DTOs.DoctorModule;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Core.Interfaces.Services.DoctorModule;
using SmartTeethCare.Core.Interfaces.Services.NotificationService;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using SmartTeethCare.Core.Interfaces.Services.Stripe;
using System;
using System.Security.Claims;

namespace SmartTeethCare.Service.PatientModule
{
    public class PatientAppointmentService : IPatientAppointmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<User> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IAppointmentBookingService _bookingService;
        private readonly IPaymentService _paymentService;

        public PatientAppointmentService(
            IUnitOfWork uow, 
            UserManager<User> userManager, 
            INotificationService notificationService, 
            IAppointmentBookingService bookingService,
            IPaymentService paymentService)
        {
            _uow = uow;
            _userManager = userManager;
            _notificationService = notificationService;
            _bookingService = bookingService;
            _paymentService = paymentService;
        }

   

        public async Task CancelAppointment(int appointmentId, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not authenticated");

            var patients = await _uow.Repository<Patient>()
                .FindAsync(p => p.UserId == userId);

            var patient = patients.FirstOrDefault();
            if (patient == null)
                throw new Exception("Patient not found");

            var patientId = patient.Id;

            var target = (await _uow.Repository<Appointment>()
                .FindAsync(a => a.Id == appointmentId && a.PatientID == patientId))
                .FirstOrDefault();


            if (target == null)
                throw new Exception("Appointment not found or not yours.");

            if (target.Status != AppointmentStatus.Pending && target.Status != AppointmentStatus.Approved)
            {
                throw new Exception("Only pending or approved appointments can be cancelled.");
            }

            if (target.Date <= DateTime.Now.AddHours(1))
                throw new Exception("Cannot cancel appointment less than 1 hour before it starts.");

            // Refund logic
            if (target.PaymentStatus == AppointmentPaymentStatus.Paid && !string.IsNullOrEmpty(target.PaymentIntentId))
            {
                var appointmentTimeUtc = target.Date.Date + target.StartTime;
                // If more than 24 hours
                if ((appointmentTimeUtc - DateTime.UtcNow).TotalHours >= 24)
                {
                    bool refunded = await _paymentService.RefundPayment(target.PaymentIntentId);
                    if (refunded)
                    {
                        target.PaymentStatus = AppointmentPaymentStatus.Refunded;
                    }
                }
            }

            target.Status = AppointmentStatus.Cancelled;
            await _notificationService.CreateAsync(
                  patient.UserId,
                  "Appointment Cancelled",
                  $"Your appointment on {target.Date} has been cancelled.",
                  true
                   );
            await _uow.Repository<Appointment>().UpdateAsync(target);
            await _uow.CompleteAsync();
        }

        public async Task<List<Appointment>> GetMyAppointments(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not authenticated");

            var patients = await _uow.Repository<Patient>()
                .FindAsync(p => p.UserId == userId);

            var patient = patients.FirstOrDefault();
            if (patient == null)
                throw new Exception("Patient not found");

            var patientId = patient.Id;

            var appointments = await _uow.Repository<Appointment>()
                .FindAsync(a => a.PatientID == patientId);

            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            return appointments
                .OrderBy(a => a.Date)
                .Select(a =>
                {
                    a.Date = TimeZoneInfo.ConvertTimeFromUtc(a.Date, egyptTimeZone);
                    a.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(a.CreatedAt, egyptTimeZone); // هنا زدنا CreatedAt
                    return a;
                })
                .ToList();

        }

        public async Task<AppointmentDetailsDTO> GetAppointmentDetails(int appointmentId, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("User not authenticated");

            var patient = (await _uow.Repository<Patient>()
                .FindAsync(p => p.UserId == userId))
                .FirstOrDefault();

            if (patient == null)
                throw new Exception("Patient not found");

            var appointment = (await _uow.Repository<Appointment>()
                .FindAsync(a => a.Id == appointmentId))
                .FirstOrDefault();

            if (appointment == null)
                throw new Exception("Appointment not found");

            // Security Check
            if (appointment.PatientID != patient.Id)
                throw new UnauthorizedAccessException("You are not authorized to view this appointment.");

            var doctor = await _uow.Repository<Doctor>()
                .GetByIdAsync(appointment.DoctorID);

            var doctorUser = doctor != null
                ? await _userManager.FindByIdAsync(doctor.UserId)
                : null;

            var patientUser = await _userManager.FindByIdAsync(patient.UserId);

            return new AppointmentDetailsDTO
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorID,
                DoctorName = string.IsNullOrWhiteSpace(doctorUser?.DisplayName)
                    ? doctorUser?.UserName ?? "Unknown"
                    : doctorUser.DisplayName,

                PatientId = appointment.PatientID,
                PatientName = string.IsNullOrWhiteSpace(patientUser?.DisplayName)
                    ? patientUser?.UserName ?? "Unknown"
                    : patientUser.DisplayName,

                Date = appointment.Date,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status.ToString(),
                CreatedAt = appointment.CreatedAt
            };
        }

    }
}

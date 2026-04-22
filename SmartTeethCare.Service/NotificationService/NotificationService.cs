using Microsoft.AspNetCore.Identity;
using SmartTeethCare.Core.DTOs.Notification;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.NotificationService;
using SmartTeethCare.Core.Interfaces.Services.SecurityModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;

namespace SmartTeethCare.Service.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;

        public NotificationService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task CreateAsync(string userId, string title, string message, bool sendEmail = false , Dictionary<string, string>? data = null)
        {
            // 🔹 1. Save notification in DB
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Notification>().AddAsync(notification);
            await _unitOfWork.CompleteAsync();

            // 🔹 2. Send Email (using template)
            if (sendEmail)
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var templateName = GetTemplateName(title);

                    var templateData = data ?? new Dictionary<string, string>();

                    var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

                    templateData["DATE"] = TimeZoneInfo
                        .ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone)
                        .ToString("dd/MM/yyyy hh:mm tt");
                    templateData["MESSAGE"] = message;
                   
                    

                    await _emailService.SendTemplateEmailAsync(
                        user.Email,
                        title,
                        templateName,
                        templateData
                    );
                }
            }
        }

        public async Task SendReminderAsync(int appointmentId)
        {
            var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(appointmentId);

            if (appointment == null)
                return;

            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(appointment.DoctorID);
            var doctorUser = await _userManager.FindByIdAsync(doctor.UserId);

            var doctorName = doctorUser?.UserName ?? "Doctor";

            var data = new Dictionary<string, string>
    {
        { "DATE", appointment.Date.ToString("dd/MM/yyyy hh:mm tt") },
        { "DOCTOR", doctorName }
    };

            await CreateAsync(
                appointment.PatientID.ToString(),
                "Reminder",
                "Your appointment is coming soon",
                true,
                data
            );
        }

        // 🔹 تحديد نوع الـ template حسب نوع الإشعار
        private string GetTemplateName(string title)
        {
            return title switch
            {
                "Appointment Booked" => "AppointmentBooked",
                "Reminder" => "ReminderEmail",
                "Appointment Cancelled" => "CancelAppointment",
                "Confirm Email" => "EmailConfirmation",
                _ => "Default"
            };
        }

        public async Task<IReadOnlyList<NotificationDto>> GetUserNotificationsAsync(string userId)
        {
            var all = await _unitOfWork.Repository<Notification>().GetAllAsync();

            return all
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToList();
        }

        public async Task MarkAsReadAsync(int id)
        {
            var repo = _unitOfWork.Repository<Notification>();

            var notification = await repo.GetByIdAsync(id);

            if (notification == null)
                throw new Exception("Not Found");

            notification.IsRead = true;

            await repo.UpdateAsync(notification);
            await _unitOfWork.CompleteAsync();
        }
    }
}
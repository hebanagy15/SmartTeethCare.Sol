using SmartTeethCare.Core.DTOs.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.NotificationService
{
    public interface INotificationService
    {
        Task CreateAsync(string userId, string title, string message, bool sendEmail = false ,Dictionary<string, string>? data = null);
        Task SendReminderAsync(int appointmentId);

        Task<IReadOnlyList<NotificationDto>> GetUserNotificationsAsync(string userId);

        Task MarkAsReadAsync(int notificationId);
    }
}

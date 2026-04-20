
using Microsoft.AspNetCore.Identity;
using SmartTeethCare.Core.DTOs.Notification;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.NotificationService;
using SmartTeethCare.Core.Interfaces.Services.SecurityModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task CreateAsync(string userId, string title, string message, bool sendEmail = false)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message
            };

            await _unitOfWork.Repository<Notification>().AddAsync(notification);
            await _unitOfWork.CompleteAsync();

            if (sendEmail)
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    await _emailService.SendEmailAsync(user.Email, title, message);
                }
            }
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

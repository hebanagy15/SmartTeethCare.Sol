// SmartTeethCare.Service/Jobs/SlotCleanupJob.cs

using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.UnitOfWork;

namespace SmartTeethCare.Service.Jobs
{
    public class SlotCleanupJob
    {
        private readonly IUnitOfWork _unitOfWork;

        public SlotCleanupJob(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Execute()
        {
            // جيب كل الـ Reservations اللي انتهت
            var expired = await _unitOfWork.Repository<SlotReservation>()
                .FindAsync(r => r.ExpiresAt < DateTime.UtcNow);

            foreach (var reservation in expired)
                await _unitOfWork.Repository<SlotReservation>().DeleteAsync(reservation);

            if (expired.Any())
                await _unitOfWork.CompleteAsync();
        }
    }
}
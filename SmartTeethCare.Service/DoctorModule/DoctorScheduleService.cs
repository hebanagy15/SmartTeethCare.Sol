using SmartTeethCare.Core.DTOs.DoctorModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Core.Interfaces.Services.DoctorModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;

namespace SmartTeethCare.Service.DoctorModule
{
    public class DoctorScheduleService : IDoctorScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DoctorScheduleDto>> GetDoctorScheduleAsync(int doctorId)
        {
            var schedules = await _unitOfWork.Repository<DoctorSchedule>()
                .FindAsync(s => s.DoctorId == doctorId);

            return schedules.Select(s => new DoctorScheduleDto
            {
                Id = s.Id,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                SlotDurationMinutes = s.SlotDurationMinutes
            }).ToList();
        }

        public async Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            // 1. جيب الـ Schedule بتاع اليوم ده
            var dayOfWeek = date.DayOfWeek;

            var schedules = await _unitOfWork.Repository<DoctorSchedule>()
                .FindAsync(s => s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek);

            var schedule = schedules.FirstOrDefault();

            if (schedule == null)
                return new List<AvailableSlotDto>(); // الدكتور مش شغال اليوم ده

            // 2. جيب المواعيد المحجوزة في اليوم ده
            var bookedAppointments = await _unitOfWork.Repository<Appointment>()
                .FindAsync(a =>
                    a.DoctorID == doctorId &&
                    a.Date.Date == date.Date &&
                    a.Status != AppointmentStatus.Rejected &&
                    a.Status != AppointmentStatus.Cancelled);

            // 3. اعمل Generate لكل الـ Slots
            var allSlots = new List<AvailableSlotDto>();
            var currentTime = schedule.StartTime;

            // الوقت دلوقتي بتوقيت مصر
            var egyptZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            var nowInEgypt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptZone);
            var isToday = date.Date == nowInEgypt.Date;

            while (currentTime + TimeSpan.FromMinutes(schedule.SlotDurationMinutes) <= schedule.EndTime)
            {
                var slotEnd = currentTime + TimeSpan.FromMinutes(schedule.SlotDurationMinutes);

                var isBooked = bookedAppointments.Any(a =>
                    a.StartTime == currentTime && a.EndTime == slotEnd);

                //  مش متاح لو اليوم ده النهارده والوقت فات  
                var isTimePassed = isToday && currentTime <= nowInEgypt.TimeOfDay;

                allSlots.Add(new AvailableSlotDto
                {
                    Date = date.Date,
                    StartTime = currentTime,
                    EndTime = slotEnd,
                    IsAvailable = !isBooked && !isTimePassed
                });

                currentTime = slotEnd;
            }
            return allSlots;
        }
        public async Task AddScheduleAsync(CreateDoctorScheduleDto dto)
        {
            // تأكد إن مفيش Schedule لنفس الدكتور في نفس اليوم
            var existing = await _unitOfWork.Repository<DoctorSchedule>()
                .FindAsync(s => s.DoctorId == dto.DoctorId && s.DayOfWeek == dto.DayOfWeek);

            if (existing.Any())
                throw new InvalidOperationException("Schedule already exists for this day");

            var schedule = new DoctorSchedule
            {
                DoctorId = dto.DoctorId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                SlotDurationMinutes = dto.SlotDurationMinutes
            };

            await _unitOfWork.Repository<DoctorSchedule>().AddAsync(schedule);
            await _unitOfWork.CompleteAsync();
        }

        // Get DoctorId from UserId (extracted from JWT Token)
        public async Task<int?> GetDoctorIdByUserIdAsync(string userId)
        {
            var doctor = (await _unitOfWork.Repository<Doctor>()
                .FindAsync(d => d.UserId == userId))
                .FirstOrDefault();

            return doctor?.Id;
        }

        public async Task DeleteScheduleAsync(int scheduleId, string userId, bool isAdmin)
        {
            var schedule = await _unitOfWork.Repository<DoctorSchedule>().GetByIdAsync(scheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            if (!isAdmin)
            {
                var doctorId = await GetDoctorIdByUserIdAsync(userId);
                if (doctorId == null || schedule.DoctorId != doctorId.Value)
                    throw new UnauthorizedAccessException("You are not authorized to delete this schedule.");
            }

            // Check if there are any future appointments booked for this doctor on this DayOfWeek
            var futureAppointments = await _unitOfWork.Repository<Appointment>()
                .FindAsync(a => 
                    a.DoctorID == schedule.DoctorId && 
                    a.Date.Date >= DateTime.UtcNow.Date &&
                    a.Status != AppointmentStatus.Rejected && 
                    a.Status != AppointmentStatus.Cancelled);

            var appointmentsOnDay = futureAppointments
                .Where(a => a.Date.DayOfWeek == schedule.DayOfWeek)
                .ToList();

            if (appointmentsOnDay.Any())
            {
                throw new InvalidOperationException("Cannot cancel schedule because patients have already booked appointments on this day.");
            }

            await _unitOfWork.Repository<DoctorSchedule>().DeleteAsync(schedule);
            await _unitOfWork.CompleteAsync();
        }
    }
}
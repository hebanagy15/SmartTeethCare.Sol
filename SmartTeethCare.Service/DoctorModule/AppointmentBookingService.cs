using SmartTeethCare.Core.DTOs.DoctorModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Core.Interfaces.Services.DoctorModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using Stripe;

namespace SmartTeethCare.Service.DoctorModule
{
    public class AppointmentBookingService : IAppointmentBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentBookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BookAppointmentResultDto> BookAppointmentAsync(BookAppointmentDto dto)
        {
            // Validation 1: الموعد في المستقبل
            if (dto.Date.Date < DateTime.Today)
            {
                return new BookAppointmentResultDto
                {
                    Success = false,
                    Message = "Cannot book appointment in the past"
                };
            }

            if (dto.Date.Date == DateTime.Today && dto.StartTime < DateTime.Now.TimeOfDay)
            {
                return new BookAppointmentResultDto
                {
                    Success = false,
                    Message = "Cannot book appointment in the past"
                };
            }

            // Validation 2: الدكتور شغال اليوم ده
            var dayOfWeek = dto.Date.DayOfWeek;

            var schedules = await _unitOfWork.Repository<DoctorSchedule>()
                .FindAsync(s => s.DoctorId == dto.DoctorId && s.DayOfWeek == dayOfWeek);

            var schedule = schedules.FirstOrDefault();

            if (schedule == null)
            {
                return new BookAppointmentResultDto
                {
                    Success = false,
                    Message = "Doctor is not available on this day"
                };
            }

            // Validation 3: الوقت جوه ساعات العمل
            var endTime = dto.StartTime + TimeSpan.FromMinutes(schedule.SlotDurationMinutes);

            if (dto.StartTime < schedule.StartTime || endTime > schedule.EndTime)
            {
                return new BookAppointmentResultDto
                {
                    Success = false,
                    Message = "Selected time is outside working hours"
                };
            }

            // Validation 4: الـ Slot فاضي
            var existingAppointments = await _unitOfWork.Repository<Appointment>()
                .FindAsync(a =>
                    a.DoctorID == dto.DoctorId &&
                    a.Date.Date == dto.Date.Date &&
                    a.StartTime == dto.StartTime &&
                    a.Status != AppointmentStatus.Rejected);

            if (existingAppointments.Any())
            {
                return new BookAppointmentResultDto
                {
                    Success = false,
                    Message = "This time slot is already booked"
                };
            }

            //  if all above validation pass success ==> can appointment
            var appointment = new Appointment
            {
                DoctorID = dto.DoctorId,
                PatientID = dto.PatientId,
                Date = dto.Date.Date,
                StartTime = dto.StartTime,
                EndTime = endTime,
                Amount = dto.Amount,
                Status = AppointmentStatus.Pending,
                PaymentStatus = AppointmentPaymentStatus.Unpaid,
                CreatedByAdmin = dto.CreatedByAdmin,
                PaymentIntentId = dto.PaymentIntentId,
                
            };

            await _unitOfWork.Repository<Appointment>().AddAsync(appointment);
            await _unitOfWork.CompleteAsync();

            return new BookAppointmentResultDto
            {
                Success = true,
                Message = "Appointment booked successfully",
                AppointmentId = appointment.Id
            };
        }
    }
}
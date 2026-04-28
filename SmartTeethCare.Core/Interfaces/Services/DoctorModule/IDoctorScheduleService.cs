using SmartTeethCare.Core.DTOs.DoctorModule;

namespace SmartTeethCare.Core.Interfaces.Services.DoctorModule
{
    public interface IDoctorScheduleService
    {
        Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(int doctorId, DateTime date);
        Task<List<DoctorScheduleDto>> GetDoctorScheduleAsync(int doctorId);
        Task AddScheduleAsync(CreateDoctorScheduleDto dto);
        Task<int?> GetDoctorIdByUserIdAsync(string userId);
    }
}
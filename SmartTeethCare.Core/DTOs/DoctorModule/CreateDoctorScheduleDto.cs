namespace SmartTeethCare.Core.DTOs.DoctorModule
{
    public class CreateDoctorScheduleDto
    {
        public int DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; }
    }
}
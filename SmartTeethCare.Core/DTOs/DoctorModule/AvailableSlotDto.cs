using System;

namespace SmartTeethCare.Core.DTOs.DoctorModule
{
    public class AvailableSlotDto
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}
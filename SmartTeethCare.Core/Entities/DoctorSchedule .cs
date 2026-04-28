using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTeethCare.Core.Entities
{
    public class DoctorSchedule : BaseEntity
    {
        public int DoctorId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }  // Sunday = 0, Saturday = 6

        public TimeSpan StartTime { get; set; }    // e.g. 16:00

        public TimeSpan EndTime { get; set; }      // e.g. 20:00

        public int SlotDurationMinutes { get; set; } // e.g. 15, 30

        [ForeignKey(nameof(DoctorId))]
        public Doctor Doctor { get; set; }
    }
}
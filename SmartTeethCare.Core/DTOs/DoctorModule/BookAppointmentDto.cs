using System;

namespace SmartTeethCare.Core.DTOs.DoctorModule
{
    public class BookAppointmentDto
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public int Amount { get; set; }
        public string PaymentMethod { get; set; }
        public bool CreatedByAdmin { get; set; } = false;
        public string? PaymentIntentId { get; set; }
    }
}
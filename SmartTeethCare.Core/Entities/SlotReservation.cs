namespace SmartTeethCare.Core.Entities
{
    public class SlotReservation : BaseEntity
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public string PaymentIntentId { get; set; }
        public DateTime ExpiresAt { get; set; }  // UtcNow + 10 دقايق
    }
}
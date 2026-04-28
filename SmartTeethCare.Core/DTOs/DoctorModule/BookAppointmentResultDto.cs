namespace SmartTeethCare.Core.DTOs.DoctorModule
{
    public class BookAppointmentResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? AppointmentId { get; set; }
    }
}
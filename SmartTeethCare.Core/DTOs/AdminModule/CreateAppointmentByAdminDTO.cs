using SmartTeethCare.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartTeethCare.Core.DTOs.AdminModule
{
    public class CreateAppointmentByAdminDTO
    {
        [Required]
        public int DoctorID { get; set; }

        [Required]
        public int PatientID { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public AppointmentPaymentMethod PaymentMethod { get; set; }
        public AppointmentPaymentStatus PaymentStatus { get; set; }
    }
}

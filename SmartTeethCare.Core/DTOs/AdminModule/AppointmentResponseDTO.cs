using SmartTeethCare.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.AdminModule
{
    public class AppointmentResponseDTO
    {
        public int Id { get; set; }
        public int DoctorID { get; set; }
        public int PatientID { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public AppointmentStatus Status { get; set; }
        public AppointmentPaymentMethod PaymentMethod { get; set; }
        public AppointmentPaymentStatus PaymentStatus { get; set; }
        public bool CreatedByAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

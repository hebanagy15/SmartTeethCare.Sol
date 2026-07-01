using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.Stripe
{
    public class CreatePaymentRequest
    {
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public string PaymentMethod { get; set; } = "Visa";
        // ✅ PatientId بنجيبه من Token في الـ Controller

    }
}

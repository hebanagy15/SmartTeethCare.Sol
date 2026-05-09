using SmartTeethCare.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class Appointment : BaseEntity
    {
        public int DoctorID { get; set; }
        public int PatientID { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ---- الجديد ----
        public DateTime Date { get; set; }               // يوم الكشف
        public TimeSpan StartTime { get; set; }          // الساعة كام (16:00)
        public TimeSpan EndTime { get; set; }            // بيخلص إمتى (16:15)
        //public string? PaymentIntentId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentStatus Status { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentPaymentMethod PaymentMethod { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentPaymentStatus PaymentStatus { get; set; }

        public bool CreatedByAdmin { get; set; } = false;
        public int? MedicalRecordId { get; set; }
        public MedicalRecord? MedicalRecord { get; set; }

        public Doctor doctor { get; set; }

        [JsonIgnore]
        public Patient patient { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    }
}
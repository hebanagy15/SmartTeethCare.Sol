
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SmartTeethCare.Core.Entities
{
    public class Doctor : BaseEntity
    {
        
        public int Salary { get; set; }
        public int WorkingHours { get; set; }
        public DateTime HiringDate { get; set; }

        public string? ImageUrl { get; set; }
        public decimal? ConsultationFee { get; set; }   
        public int? YearsOfExperience { get; set; }     


        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<DentistQualifications> DentistQualifications { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();


        public int? SpecialtyID { get; set; }

        [ForeignKey(nameof(SpecialtyID))]
        public Speciality? Speciality { get; set; }

        public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();  // New 
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    }
} 

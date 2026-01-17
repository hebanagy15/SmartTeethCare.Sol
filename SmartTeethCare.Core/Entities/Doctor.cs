
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartTeethCare.Core.Entities
{
    public class Doctor : BaseEntity
    {
        public int SpecialtyID { get; set; }
        public int Salary { get; set; }
        public int WorkingHours { get; set; }
        public DateTime HiringDate { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
        public ICollection<DentistQualifications> DentistQualifications { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public Speciality Speciality { get; set; }

        public ICollection<Appointment> Appointments { get; set; }

    }
} 

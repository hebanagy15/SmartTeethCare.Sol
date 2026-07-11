using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartTeethCare.Core.Attributes;

namespace SmartTeethCare.Core.DTOs.PatientModule
{
    public class UpdatePatientProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }

        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        [ImageMagicBytes(ErrorMessage = "Profile image must be a valid JPG or PNG file.")]
        public IFormFile? ProfileImage { get; set; }
    }
}

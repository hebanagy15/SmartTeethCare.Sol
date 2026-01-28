using System.ComponentModel.DataAnnotations;

namespace SmartTeethCare.API.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Role { get; set; } = "Patient";

        public String Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

    }
}

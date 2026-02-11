using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
namespace SmartTeethCare.Core.Entities
{
    public class User : IdentityUser
    {
        public string Address { get; set; }

        public string Gender { get; set; }

        [NotMapped]
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
        public Admin? Admin { get; set; }
    }
}

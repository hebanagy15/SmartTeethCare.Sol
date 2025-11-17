using Microsoft.AspNetCore.Identity;
namespace SmartTeethCare.Core.Entities
{
    public class User : IdentityUser
    {
        public string Address { get; set; }

        public string Gender { get; set; }

        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
	}
}

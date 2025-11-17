namespace SmartTeethCare.Core.Entities;

public class Doctor : BaseEntity
{
    public int Salary { get; set; }
    public int WorkingHours { get; set; }
    public DateTime HiringDate { get; set; }

    // Relation with ApplicationUser
    public string UserId { get; set; }
    public User User { get; set; }
}

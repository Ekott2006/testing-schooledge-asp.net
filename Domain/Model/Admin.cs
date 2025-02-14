namespace Domain.Model;

public class Admin
{
    public string UserId { get; set; }
    public User User { get; set; }
    public Guid Id { get; set; }

    public string PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
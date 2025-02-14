namespace Domain.Dto.Admin;

public interface IAdminRequest
{
    public DateTime? DateOfBirth { get; set; }

    public string PhoneNumber { get; set; }
}
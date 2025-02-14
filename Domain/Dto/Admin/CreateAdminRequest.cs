using Domain.Dto.User;

namespace Domain.Dto.Admin;

public class CreateAdminRequest : RegisterWithPasswordRequest, IAdminRequest
{

    public Model.Admin ToAdmin(string userId) => new()
    {
        UserId = userId,
        PhoneNumber = PhoneNumber,
        DateOfBirth = DateOfBirth,
    };

    public DateTime? DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }
}
using Domain.Dto.User;

namespace Domain.Dto.Admin;

public class UpdateAdminRequest : UpdateUserRequest, IAdminRequest
{
    public DateTime? DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }
}
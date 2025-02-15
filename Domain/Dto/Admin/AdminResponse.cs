using Domain.Dto.User;

namespace Domain.Dto.Admin;

public class AdminResponse(): UserResponse
{
    public Guid AdminId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }

    public AdminResponse(Model.Admin? admin): this()
    {
        if (admin == null ) return;
        UserId = admin.UserId;
        UserName = admin.User?.UserName;
        Email = admin.User?.Email;
        FirstName = admin.User?.FirstName;
        MiddleName = admin.User?.MiddleName;
        LastName = admin.User?.LastName;
        ProfileImageUrl = admin.User?.ProfileImageUrl;
        AdminId = admin.Id;
        DateOfBirth = admin.DateOfBirth;
        PhoneNumber = admin.PhoneNumber;
    }
}
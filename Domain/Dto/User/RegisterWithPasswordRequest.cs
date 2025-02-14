namespace Domain.Dto.User;

public class RegisterWithPasswordRequest : RegisterRequest
{
    public string Password { get; set; }
}
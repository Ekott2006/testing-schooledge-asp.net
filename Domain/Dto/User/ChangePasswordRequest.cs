namespace Domain.Dto.User;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
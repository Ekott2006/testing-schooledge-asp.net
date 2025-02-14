namespace Domain.Dto.User;

public record AuthResponse(string Id, string AccessToken, string RefreshToken);
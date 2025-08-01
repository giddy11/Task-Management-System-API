namespace TaskManagement.Application.Features.Auth.Dtos;

public class RefreshTokenResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}

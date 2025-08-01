using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Application.Features.Auth.Dtos;

public class LoginResponse
{
    public string Token { get; set; } = default!;
    public Guid UserId { get; set; }
    public string Email { get; set; } = default!;
    public AccountTypes AccountType { get; set; } = default!;
}

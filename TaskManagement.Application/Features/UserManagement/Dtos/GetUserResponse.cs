using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Application.Features.UserManagement.Dtos;

public class GetUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public AccountTypes AccountType { get; set; }
    public UserStatus UserStatus { get; set; }
}

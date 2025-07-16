using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Application.UserManagement.Dtos;

public class CreateUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public AccountTypes AccountType { get; set; }
    public UserStatus UserStatus { get; set; }
}

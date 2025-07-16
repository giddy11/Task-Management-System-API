using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Application.UserManagement.Dtos;

public class UpdateUserRequest
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public AccountTypes AccountType { get; set; }
    public UserStatus UserStatus { get; set; }
}

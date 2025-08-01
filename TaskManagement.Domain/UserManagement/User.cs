using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.Domain.UserManagement;

public class User
{
    public Guid Id { get; init; }
    public string Email { get; set; } = default!;
    public string? PasswordHash { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public IList<TodoTask> TodoTasks { get; set; } = [];
    public AccountTypes AccountType { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }
    public string? VerificationCode { get; set; } // For email verification
    public DateTime? VerificationCodeExpiry { get; set; }
    public UserStatus UserStatus { get; set; } = UserStatus.Active;
}

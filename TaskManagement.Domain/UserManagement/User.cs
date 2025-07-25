using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.Domain.UserManagement;

public class User
{
    public Guid Id { get; init; }
    public string Email { get; set; } = default!;
    public string? PasswordHash { get; protected set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public IList<TodoTask> TodoTasks { get; set; } = [];
    public AccountTypes AccountType { get; set; }
    public UserStatus UserStatus { get; protected set; } = UserStatus.Active;
}

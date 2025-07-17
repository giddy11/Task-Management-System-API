using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.Domain.UserManagement;

public class User
{
    protected User() { }
    public static User New(string email, string firstName, string lastName, AccountTypes accountType = AccountTypes.User, Guid? Id = null)
    {
        return new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            AccountType = accountType,
            Id = Id ?? Guid.Empty
        };
    }

    public void Update(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public Guid Id { get; init; }
    public string Email { get; set; } = default!;
    public string? PasswordHash { get; protected set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public IList<TodoTask> TodoTasks { get; set; } = new List<TodoTask>();

    public AccountTypes AccountType { get; protected set; }
    public UserStatus UserStatus { get; protected set; } = UserStatus.Active;
}

using TaskManagement.Domain.TodoTasks;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Domain;

public class Comment
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public Guid TodoTaskId { get; set; }
    public User User { get; set; } = default!;
    public TodoTask TodoTask { get; set; } = default!;
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}

using TaskManagement.Domain.TodoTasks;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Domain;

public class Label
{
    public Guid Id { get; init; }
    public string Name { get; set; } = default!;
    public string? Color { get; set; }
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = default!;
    public IList<TodoTask> TodoTasks { get; set; } = [];
}

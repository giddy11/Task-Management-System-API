using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.Domain;

public class Label
{
    public Guid Id { get; init; }
    public string Name { get; set; } = default!;
    public string? Color { get; set; }
    public IList<TodoTask> TodoTasks { get; set; } = new List<TodoTask>();
}

using TaskManagement.Domain.Projects;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Domain.TodoTasks;

public class TodoTask
{
    public Guid Id { get; init; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public User CreatedBy { get; set; } = default!;
    public IList<User> Assignees { get; set; } = [];
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public IList<Comment> Comments { get; set; } = [];
    public IList<Label> Labels { get; set; } = [];
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = default!;
    public TaskStatus TaskStatus { get; set; } = TaskStatus.Todo;
    public PriorityStatus PriorityStatus { get; set; } = PriorityStatus.Low;
}

using TaskManagement.Domain.TodoTasks;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Domain.Projects;

public class Project
{
    public Guid Id { get; init; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.Not_Started;
    public IList<TodoTask>? Tasks { get; set; } = new List<TodoTask>();
}

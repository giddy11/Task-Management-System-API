using TaskManagement.Domain.Projects;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Domain.TodoTasks;

public class TodoTask
{
    protected TodoTask() { }

    public static TodoTask New(
        string title,
        Guid createdById,
        Guid projectId,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        TodoTaskStatus status = TodoTaskStatus.Todo,
        PriorityStatus priority = PriorityStatus.Low,
        Guid? id = null)
    {
        return new TodoTask
        {
            Id = id ?? Guid.NewGuid(),
            Title = title,
            Description = description,
            CreatedById = createdById,
            ProjectId = projectId,
            StartDate = startDate,
            EndDate = endDate,
            TodoTaskStatus = status,
            PriorityStatus = priority
        };
    }

    public Guid Id { get; init; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = default!;
    public IList<User> Assignees { get; set; } = [];
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public IList<Comment> Comments { get; set; } = [];
    public IList<Label> Labels { get; set; } = [];
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = default!;
    public TodoTaskStatus TodoTaskStatus { get; set; } = TodoTaskStatus.Todo;
    public PriorityStatus PriorityStatus { get; set; } = PriorityStatus.Low;
}

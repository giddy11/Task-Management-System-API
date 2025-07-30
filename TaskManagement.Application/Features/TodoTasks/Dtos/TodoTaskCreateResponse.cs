using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.Application.Features.TodoTasks.Dtos;

public class TodoTaskCreateResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public Guid CreatedById { get; set; }
    public Guid ProjectId { get; set; }
    public TodoTaskStatus TodoTaskStatus { get; set; }
    public PriorityStatus PriorityStatus { get; set; }
}

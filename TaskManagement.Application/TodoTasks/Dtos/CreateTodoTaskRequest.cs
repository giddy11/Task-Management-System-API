using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.Application.TodoTasks.Dtos;

public class CreateTodoTaskRequest
{
    [Required]
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    [Required]
    public Guid CreatedById { get; set; }
    [Required]
    public Guid ProjectId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TodoTaskStatus Status { get; set; } = TodoTaskStatus.Todo;
    public PriorityStatus Priority { get; set; } = PriorityStatus.Low;
}

using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.TodoTasks.Dtos;

public class UpdateTodoTaskRequest
{
    [Required]
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

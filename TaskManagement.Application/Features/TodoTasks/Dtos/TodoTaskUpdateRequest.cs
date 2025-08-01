using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Features.TodoTasks.Dtos;

public class TodoTaskUpdateRequest
{
    [Required]
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

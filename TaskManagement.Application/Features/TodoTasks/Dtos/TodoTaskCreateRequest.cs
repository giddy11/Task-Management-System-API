using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.Application.Features.TodoTasks.Dtos;

public class TodoTaskCreateRequest
{
    [Required]
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    [Required]
    public Guid CreatedById { get; set; }
    [Required]
    public Guid ProjectId { get; set; }
}

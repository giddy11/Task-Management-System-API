using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Features.TodoTasks.Dtos;

public class LabelTaskRequest
{
    [Required]
    public Guid TodoTaskId { get; set; }

    [Required]
    public Guid LabelId { get; set; }
}
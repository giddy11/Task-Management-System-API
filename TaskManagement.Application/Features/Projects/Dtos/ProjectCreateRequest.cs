using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Features.Projects.Dtos;

public class ProjectCreateRequest
{
    [Required]
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    [Required]
    public Guid CreatedById { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
}

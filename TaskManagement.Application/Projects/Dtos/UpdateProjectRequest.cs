using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Projects.Dtos;

public class UpdateProjectRequest
{
    [Required]
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
}

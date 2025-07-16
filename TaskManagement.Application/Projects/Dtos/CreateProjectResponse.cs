using TaskManagement.Domain.Projects;

namespace TaskManagement.Application.Projects.Dtos;

public class CreateProjectResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public Guid CreatedById { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ProjectStatus ProjectStatus { get; set; }
}
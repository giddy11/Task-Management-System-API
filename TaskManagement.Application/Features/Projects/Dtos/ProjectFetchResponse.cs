using TaskManagement.Domain.Projects;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Application.Features.Projects.Dtos;

public class ProjectFetchResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ProjectStatus ProjectStatus { get; set; }
}

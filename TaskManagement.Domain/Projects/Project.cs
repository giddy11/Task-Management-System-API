using TaskManagement.Domain.TodoTasks;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Domain.Projects;

public class Project
{
    protected Project() { }

    public static Project New(string title, string? description, Guid createdById,
        DateTime startDate, DateTime endDate, ProjectStatus status = ProjectStatus.Not_Started, Guid? id = null)
    {
        return new Project
        {
            Id = id ?? Guid.NewGuid(),
            Title = title,
            Description = description,
            CreatedById = createdById,
            StartDate = startDate,
            EndDate = endDate,
            ProjectStatus = status
        };
    }

    public void Update(string title, string? description, DateTime startDate, DateTime endDate)
    {
        Title = title;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
    }

    public Project ChangeStatus(ProjectStatus status)
    {
        ProjectStatus = status;
        return this;
    }

    public Guid Id { get; init; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.Not_Started;
    public IList<TodoTask>? TodoTasks { get; set; } = [];
}

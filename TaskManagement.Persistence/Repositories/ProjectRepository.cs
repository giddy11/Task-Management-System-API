using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Domain.Projects;

namespace TaskManagement.Persistence.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(TaskManagementDbContext context) : base(context)
    {
    }
}

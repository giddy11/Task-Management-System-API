using TaskManagement.Application.Projects.Dtos;
using TaskManagement.Application.Utils;

namespace TaskManagement.Application.Projects;

public interface IProjectRepository
{
    Task<OperationResponse<CreateProjectResponse>> CreateAsync(CreateProjectRequest request);
}

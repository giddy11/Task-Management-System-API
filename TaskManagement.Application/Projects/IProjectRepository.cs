using TaskManagement.Application.Projects.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.Projects;

namespace TaskManagement.Application.Projects;

public interface IProjectRepository
{
    Task<OperationResponse<CreateProjectResponse>> CreateAsync(CreateProjectRequest request);
    Task<OperationResponse<GetProjectResponse>> GetByIdAsync(Guid id);
    Task<OperationResponse<List<GetProjectResponse>>> GetAllAsync(int page, int pageSize);
    Task<OperationResponse<GetProjectResponse>> UpdateAsync(Guid id, UpdateProjectRequest request);
    Task<OperationResponse<string>> DeleteAsync(Guid id);
    Task<OperationResponse<GetProjectResponse>> ChangeStatusAsync(Guid id, ProjectStatus status);
}

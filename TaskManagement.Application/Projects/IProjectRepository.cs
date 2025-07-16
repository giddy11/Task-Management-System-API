using TaskManagement.Application.Projects.Dtos;
using TaskManagement.Application.Utils;

namespace TaskManagement.Application.Projects;

public interface IProjectRepository
{
    Task<OperationResponse<CreateProjectResponse>> CreateAsync(CreateProjectRequest request);
    Task<OperationResponse<GetProjectResponse>> GetByIdAsync(Guid id);
    Task<OperationResponse<List<GetProjectResponse>>> GetAllAsync(int page, int pageSize);
    Task<OperationResponse<GetProjectResponse>> UpdateAsync(Guid id, UpdateProjectRequest request);
}

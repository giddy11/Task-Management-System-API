using TaskManagement.Application.UserManagement.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Application.UserManagement;

public interface IUserRepository
{
    Task<OperationResponse<CreateUserResponse>> CreateAsync(User request);
    Task<OperationResponse<GetUserResponse>> GetByIdAsync(Guid id);
    Task<OperationResponse<List<GetUserResponse>>> GetAllAsync(int page, int pageSize);
    Task<OperationResponse> UpdateAsync(Guid id, User request);
    Task<OperationResponse<string>> DeleteAsync(Guid id);
}

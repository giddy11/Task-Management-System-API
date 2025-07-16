using TaskManagement.Application.UserManagement.Dtos;
using TaskManagement.Application.Utils;

namespace TaskManagement.Application.UserManagement;

public interface IUserRepository
{
    Task<OperationResponse<CreateUserResponse>> CreateAsync(CreateUserRequest request);
}

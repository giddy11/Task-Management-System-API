using TaskManagement.Application.Auth.Dtos;
using TaskManagement.Application.UserManagement.Dtos;
using TaskManagement.Application.Utils;

namespace TaskManagement.Application.Auth;

public interface IAuthRepository
{
    Task<OperationResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<OperationResponse<CreateUserResponse>> RegisterAsync(CreateUserRequest request);
}

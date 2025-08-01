using TaskManagement.Domain;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Application.Contracts.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmailAndResetTokenAsync(string email, string token);
    Task<User> GetByEmailAndVerificationCodeAsync(string email, string code);
    Task<User?> GetUserByEmailAsync(string email);
}
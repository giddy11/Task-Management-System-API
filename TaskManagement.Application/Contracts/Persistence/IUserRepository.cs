using TaskManagement.Domain;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Application.Contracts.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByEmailAsync(string email);
}
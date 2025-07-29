using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(TaskManagementDbContext context) : base(context)
    {
    }
}
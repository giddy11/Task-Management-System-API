using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(TaskManagementDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return await _dbContext.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    protected readonly TaskManagementDbContext _dbContext;
}
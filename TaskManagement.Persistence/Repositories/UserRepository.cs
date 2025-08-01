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

    public async Task<User?> GetByEmailAndResetTokenAsync(string email, string token)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            return null;

        return await _dbContext.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && u.PasswordResetToken == token);
    }

    public async Task<User?> GetByEmailAndVerificationCodeAsync(string email, string code)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            return null;

        return await _dbContext.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && u.VerificationCode == code);
    }

    protected readonly TaskManagementDbContext _dbContext;
}
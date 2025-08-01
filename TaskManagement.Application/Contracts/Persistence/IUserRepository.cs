using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Application.Contracts.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task AddRefreshTokenAsync(RefreshToken newRefreshTokenEntity);
    Task<User> GetByEmailAndResetTokenAsync(string email, string token);
    Task<User> GetByEmailAndVerificationCodeAsync(string email, string code);
    Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
    Task<User?> GetUserByEmailAsync(string email);
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
}
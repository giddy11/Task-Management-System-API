using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Persistence.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(TaskManagementDbContext context) : base(context) { }
}

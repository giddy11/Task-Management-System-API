using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Domain;

namespace TaskManagement.Persistence.Repositories;

public class LabelRepository : Repository<Label>, ILabelRepository
{
    public LabelRepository(TaskManagementDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Label?> GetByNameAndCreatorAsync(string name, Guid createdById)
    {
        if (string.IsNullOrWhiteSpace(name) || createdById == Guid.Empty)
            return null;

        return await _dbContext.Set<Label>()
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Name == name && l.CreatedById == createdById);
    }

    protected readonly TaskManagementDbContext _dbContext;
}

using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Contracts.Persistence;

namespace TaskManagement.Persistence.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly TaskManagementDbContext _dbContext;

    public Repository(TaskManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<TEntity>().Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(bool asNoTracking = true)
    {
        if (asNoTracking)
            return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        return await _dbContext.Set<TEntity>().ToListAsync();
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<TEntity>().Update(entity);
        await Task.CompletedTask;
    }

    public async Task<TEntity?> GetByIdAsync<TKey>(TKey id)
    {
        return await _dbContext.Set<TEntity>().FindAsync(id);
    }
}

namespace TaskManagement.Application.Contracts.Persistence;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync<TKey>(TKey id);
    Task<IReadOnlyList<TEntity>> GetAllAsync(bool asNoTracking = true);
    Task AddAsync (TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}

using TaskManagement.Domain;

namespace TaskManagement.Application.Contracts.Persistence;

public interface ILabelRepository : IRepository<Label>
{
    Task<Label?> GetByNameAndCreatorAsync(string name, Guid createdById);
}

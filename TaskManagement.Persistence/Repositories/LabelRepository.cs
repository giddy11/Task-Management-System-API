using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Domain;

namespace TaskManagement.Persistence.Repositories;

public class LabelRepository : Repository<Label>, ILabelRepository
{
    public LabelRepository(TaskManagementDbContext context) : base(context)
    {

    }
}

using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Domain;

namespace TaskManagement.Persistence.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(TaskManagementDbContext context) : base(context)
    {
        
    }
}

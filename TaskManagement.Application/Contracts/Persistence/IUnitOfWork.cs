using TaskManagement.Application.Utils;

namespace TaskManagement.Application.Contracts.Persistence;

public interface IUnitOfWork
{
    ICommentRepository CommentRepository { get; }
    ILabelRepository LabelRepository { get; }
    IProjectRepository ProjectRepository { get; }
    ITodoTaskRepository TodoTaskRepository { get; }
    IUserRepository UserRepository { get; }
    Task<OperationResponse> SaveChangesAsync();
}

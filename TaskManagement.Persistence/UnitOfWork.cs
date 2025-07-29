using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Application.Utils;
using TaskManagement.Persistence.Repositories;

namespace TaskManagement.Persistence;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(TaskManagementDbContext context, ILogger<UnitOfWork> logger)
    {
        _dbContext = context;
        _logger = logger;
    }

    private ICommentRepository? _commentRepository;
    public ICommentRepository CommentRepository =>
        _commentRepository ??= new CommentRepository(_dbContext);

    private ILabelRepository? _labelRepository;
    public ILabelRepository LabelRepository =>
        _labelRepository ??= new LabelRepository(_dbContext);

    private IProjectRepository? _projectRepository;
    public IProjectRepository ProjectRepository =>
        _projectRepository ??= new ProjectRepository(_dbContext);

    private ITodoTaskRepository? _todoTaskRepository;
    public ITodoTaskRepository TodoTaskRepository =>
        _todoTaskRepository ??= new TodoTaskRepository(_dbContext);

    private IUserRepository? _userRepository;
    public IUserRepository UserRepository =>
        _userRepository ??= new UserRepository(_dbContext);

    public async Task<OperationResponse> SaveChangesAsync()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
            return OperationResponse.SuccessfulResponse();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Message.Contains("REFERENCE constraint"))
        {
            var errorMessage = "Entity cannot be deleted, it's currently in use by other records";
            _logger.LogWarning(errorMessage, ex);

            return OperationResponse.FailedResponse(StatusCode.BadRequest)
                .AddError(errorMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = "Error occured while saving changes.";
            _logger.LogWarning(errorMessage, ex);

            return OperationResponse.FailedResponse(StatusCode.InternalServerError)
                .AddError(errorMessage);
        }
    }

    private readonly TaskManagementDbContext _dbContext;
    private readonly ILogger _logger;
}

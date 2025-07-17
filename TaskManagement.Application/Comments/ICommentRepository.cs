using TaskManagement.Application.Comments.Dtos;
using TaskManagement.Application.Utils;

namespace TaskManagement.Application.Comments;

public interface ICommentRepository
{
    Task<OperationResponse<GetCommentResponse>> GetByIdAsync(Guid id);
    Task<OperationResponse<CreateCommentResponse>> CreateAsync(CreateCommentRequest request);
    Task<OperationResponse<List<GetCommentResponse>>> GetAllForTaskAsync(Guid taskId, int page, int pageSize);
}

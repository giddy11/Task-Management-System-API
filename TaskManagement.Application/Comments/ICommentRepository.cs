using TaskManagement.Application.Comments.Dtos;
using TaskManagement.Application.Utils;

namespace TaskManagement.Application.Comments;

public interface ICommentRepository
{
    Task<OperationResponse<GetCommentResponse>> GetByIdAsync(Guid id);
    Task<OperationResponse<CreateCommentResponse>> CreateAsync(CreateCommentRequest request);
    Task<OperationResponse<List<GetCommentResponse>>> GetAllForTaskAsync(Guid taskId);
    Task<OperationResponse<GetCommentResponse>> UpdateAsync(Guid id, UpdateCommentRequest request);
    Task<OperationResponse<string>> DeleteAsync(Guid id);
}

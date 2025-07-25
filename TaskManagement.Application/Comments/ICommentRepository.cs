using TaskManagement.Application.Comments.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain;

namespace TaskManagement.Application.Comments;

public interface ICommentRepository
{
    Task<OperationResponse<CreateCommentResponse>> CreateAsync(Comment request);
    Task<OperationResponse<GetCommentResponse>> UpdateAsync(Guid id, Comment request);
    Task<OperationResponse> DeleteAsync(Guid id);
}

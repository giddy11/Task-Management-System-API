using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Comments.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain;
using TaskManagement.Persistence;

namespace TaskManagement.Application.Comments;

public class CommentRepository : ICommentRepository
{
    private readonly TaskManagementDbContext _context;
    private readonly IMapper _mapper;

    public CommentRepository(TaskManagementDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OperationResponse<CreateCommentResponse>> CreateAsync(Comment request)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
        {
            return OperationResponse<CreateCommentResponse>
                .FailedResponse(StatusCode.NotFound)
                .AddError("User not found");
        }

        var taskExists = await _context.Tasks.AnyAsync(t => t.Id == request.TodoTaskId);
        if (!taskExists)
        {
            return OperationResponse<CreateCommentResponse>
                .FailedResponse(StatusCode.NotFound)
                .AddError("Task not found");
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            TodoTaskId = request.TodoTaskId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        var newComment = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.TodoTask)
            .FirstOrDefaultAsync(c => c.Id == comment.Id);

        var response = _mapper.Map<CreateCommentResponse>(newComment);
        return OperationResponse<CreateCommentResponse>.SuccessfulResponse(response);
    }

    public async Task<OperationResponse<GetCommentResponse>> UpdateAsync(Guid id, Comment request)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment is null)
        {
            return OperationResponse<GetCommentResponse>
                .FailedResponse(StatusCode.NotFound)
                .AddError("Comment not found");
        }

        comment.Content = request.Content;
        comment.UpdatedAt = DateTime.UtcNow;

        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();

        var updatedComment = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.TodoTask)
            .FirstOrDefaultAsync(c => c.Id == id);

        var mapped = _mapper.Map<GetCommentResponse>(updatedComment);
        return OperationResponse<GetCommentResponse>.SuccessfulResponse(mapped);
    }

    public async Task<OperationResponse> DeleteAsync(Guid id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment is null)
        {
            return OperationResponse<string>
                .FailedResponse(StatusCode.NotFound)
                .AddError("Comment not found");
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return OperationResponse.SuccessfulResponse();
    }
}
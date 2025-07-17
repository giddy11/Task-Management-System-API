using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Comments.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain;
using TaskManagement.Persistence;

namespace TaskManagement.Application.Comments
{
    public class CommentRepository : ICommentRepository
    {
        private readonly TaskManagementDbContext _context;
        private readonly IMapper _mapper;

        public CommentRepository(TaskManagementDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OperationResponse<CreateCommentResponse>> CreateAsync(CreateCommentRequest request)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists)
            {
                return OperationResponse<CreateCommentResponse>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError("User not found");
            }

            var taskExists = await _context.Tasks.AnyAsync(t => t.Id == request.TaskId);
            if (!taskExists)
            {
                return OperationResponse<CreateCommentResponse>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError("Task not found");
            }

            var comment = Comment.New(request.UserId, request.TaskId, request.Content);
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            var newComment = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.TodoTask)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            var response = _mapper.Map<CreateCommentResponse>(newComment);
            return OperationResponse<CreateCommentResponse>.SuccessfulResponse(response);
        }

        public async Task<OperationResponse<List<GetCommentResponse>>> GetAllForTaskAsync(Guid taskId)
        {
            var comments = await _context.Comments
                .Where(c => c.TodoTaskId == taskId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var mapped = _mapper.Map<List<GetCommentResponse>>(comments);
            return OperationResponse<List<GetCommentResponse>>.SuccessfulResponse(mapped);
        }

        public async Task<OperationResponse<GetCommentResponse>> GetByIdAsync(Guid id)
        {
            var comment = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.TodoTask)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment is null)
            {
                return OperationResponse<GetCommentResponse>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError("Comment not found");
            }

            var mapped = _mapper.Map<GetCommentResponse>(comment);
            return OperationResponse<GetCommentResponse>.SuccessfulResponse(mapped);
        }

        public async Task<OperationResponse<GetCommentResponse>> UpdateAsync(Guid id, UpdateCommentRequest request)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment is null)
            {
                return OperationResponse<GetCommentResponse>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError("Comment not found");
            }

            comment.Update(request.Content);
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            // Reload with relationships
            var updatedComment = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.TodoTask)
                .FirstOrDefaultAsync(c => c.Id == id);

            var mapped = _mapper.Map<GetCommentResponse>(updatedComment);
            return OperationResponse<GetCommentResponse>.SuccessfulResponse(mapped);
        }

        public async Task<OperationResponse<string>> DeleteAsync(Guid id)
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
            return OperationResponse<string>.SuccessfulResponse("Comment deleted successfully");
        }
    }
}

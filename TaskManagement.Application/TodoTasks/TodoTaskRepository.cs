using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.TodoTasks.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.TodoTasks;
using TaskManagement.Persistence;

namespace TaskManagement.Application.TodoTasks
{
    public class TodoTaskRepository : ITodoTaskRepository
    {
        private readonly TaskManagementDbContext _context;
        private readonly IMapper _mapper;
        public TodoTaskRepository(TaskManagementDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OperationResponse<CreateTodoTaskResponse>> CreateAsync(CreateTodoTaskRequest request)
        {
            var creatorExists = await _context.Users.AnyAsync(u => u.Id == request.CreatedById);
            if (!creatorExists)
            {
                return OperationResponse<CreateTodoTaskResponse>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError("Creator user not found");
            }

            var projectExists = await _context.Projects.AnyAsync(p => p.Id == request.ProjectId);
            if (!projectExists)
            {
                return OperationResponse<CreateTodoTaskResponse>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError("Project not found");
            }

            var task = TodoTask.New(
                request.Title,
                request.CreatedById,
                request.ProjectId,
                request.Description
                );

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<CreateTodoTaskResponse>(task);
            return OperationResponse<CreateTodoTaskResponse>.SuccessfulResponse(response);
        }

        public async Task<OperationResponse<List<GetTodoTaskResponse>>> GetAllAsync(int page, int pageSize)
        {
            var tasks = await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.Project)
                .Include(t => t.Assignees)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _mapper.Map<List<GetTodoTaskResponse>>(tasks);
            return OperationResponse<List<GetTodoTaskResponse>>.SuccessfulResponse(mapped);
        }

        public async Task<OperationResponse<GetTodoTaskResponse>> GetByIdAsync(Guid id)
        {
            var task = await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.Project)
                .Include(t => t.Assignees)
                .Include(t => t.Comments)
                .Include(t => t.Labels)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task is null)
            {
                return OperationResponse<GetTodoTaskResponse>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError("Task not found");
            }

            var mapped = _mapper.Map<GetTodoTaskResponse>(task);
            return OperationResponse<GetTodoTaskResponse>.SuccessfulResponse(mapped);
        }

        public async Task<OperationResponse<GetTodoTaskResponse>> UpdateAsync(Guid id, UpdateTodoTaskRequest request)
        {
            var task = await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task is null)
            {
                return OperationResponse<GetTodoTaskResponse>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError("Task not found");
            }

            task.Update(request.Title, request.Description, request.StartDate, request.EndDate);
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            var mapped = _mapper.Map<GetTodoTaskResponse>(task);
            return OperationResponse<GetTodoTaskResponse>.SuccessfulResponse(mapped);
        }

        public async Task<OperationResponse<GetTodoTaskResponse>> ChangeStatusAsync(Guid id, TodoTaskStatus status)
        {
            var task = await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task is null)
            {
                return OperationResponse<GetTodoTaskResponse>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError("Task not found");
            }

            task.ChangeStatus(status);
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            var mapped = _mapper.Map<GetTodoTaskResponse>(task);
            return OperationResponse<GetTodoTaskResponse>.SuccessfulResponse(mapped);
        }

        public async Task<OperationResponse<GetTodoTaskResponse>> ChangePriorityAsync(Guid id, PriorityStatus priority)
        {
            var task = await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task is null)
            {
                return OperationResponse<GetTodoTaskResponse>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError("Task not found");
            }

            task.ChangePriority(priority);
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            var mapped = _mapper.Map<GetTodoTaskResponse>(task);
            return OperationResponse<GetTodoTaskResponse>.SuccessfulResponse(mapped);
        }

        public async Task<OperationResponse<GetTodoTaskResponse>> AssignUserAsync(Guid taskId, Guid userId)
        {
            var task = await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.Project)
                .Include(t => t.Assignees)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            var user = await _context.Users.FindAsync(userId);

            if (task is null || user is null)
            {
                return OperationResponse<GetTodoTaskResponse>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError(task is null ? "Task not found" : "User not found");
            }

            task.AssignToUser(user);
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            var mapped = _mapper.Map<GetTodoTaskResponse>(task);
            return OperationResponse<GetTodoTaskResponse>.SuccessfulResponse(mapped);
        }

        public async Task<OperationResponse<GetTodoTaskResponse>> RemoveAssigneeAsync(Guid taskId, Guid userId)
        {
            var task = await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.Project)
                .Include(t => t.Assignees)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            var user = await _context.Users.FindAsync(userId);

            if (task is null || user is null)
            {
                return OperationResponse<GetTodoTaskResponse>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError(task is null ? "Task not found" : "User not found");
            }

            task.RemoveAssignee(user);
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            var mapped = _mapper.Map<GetTodoTaskResponse>(task);
            return OperationResponse<GetTodoTaskResponse>.SuccessfulResponse(mapped);
        }

        public async Task<OperationResponse<string>> DeleteAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task is null)
            {
                return OperationResponse<string>
                    .FailedResponse(StatusCode.NotFound)
                    .AddError("Task not found");
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return OperationResponse<string>.SuccessfulResponse("Task deleted successfully");
        }
    }
}

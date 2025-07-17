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
    }
}

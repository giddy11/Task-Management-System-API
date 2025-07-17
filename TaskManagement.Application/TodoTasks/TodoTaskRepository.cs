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
                request.Description,
                request.StartDate,
                request.EndDate);

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<CreateTodoTaskResponse>(task);
            return OperationResponse<CreateTodoTaskResponse>.SuccessfulResponse(response);
        }
    }
}

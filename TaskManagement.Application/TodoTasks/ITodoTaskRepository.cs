using TaskManagement.Application.TodoTasks.Dtos;
using TaskManagement.Application.Utils;

namespace TaskManagement.Application.TodoTasks;

public interface ITodoTaskRepository
{
    Task<OperationResponse<CreateTodoTaskResponse>> CreateAsync(CreateTodoTaskRequest request);
    Task<OperationResponse<List<GetTodoTaskResponse>>> GetAllAsync(int page, int pageSize);
    Task<OperationResponse<GetTodoTaskResponse>> GetByIdAsync(Guid id);
    Task<OperationResponse<GetTodoTaskResponse>> UpdateAsync(Guid id, UpdateTodoTaskRequest request);
}

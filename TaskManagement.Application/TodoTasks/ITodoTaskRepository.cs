using TaskManagement.Application.TodoTasks.Dtos;
using TaskManagement.Application.Utils;
using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.Application.TodoTasks;

public interface ITodoTaskRepository
{
    Task<OperationResponse<CreateTodoTaskResponse>> CreateAsync(CreateTodoTaskRequest request);
    Task<OperationResponse<List<GetTodoTaskResponse>>> GetAllAsync(int page, int pageSize);
    Task<OperationResponse<GetTodoTaskResponse>> GetByIdAsync(Guid id);
    Task<OperationResponse<GetTodoTaskResponse>> UpdateAsync(Guid id, UpdateTodoTaskRequest request);
    Task<OperationResponse<GetTodoTaskResponse>> ChangeStatusAsync(Guid id, TodoTaskStatus status);
    Task<OperationResponse<GetTodoTaskResponse>> ChangePriorityAsync(Guid id, PriorityStatus priority);
    Task<OperationResponse<GetTodoTaskResponse>> AssignUserAsync(Guid taskId, Guid userId);
    Task<OperationResponse<GetTodoTaskResponse>> RemoveAssigneeAsync(Guid taskId, Guid userId);
}

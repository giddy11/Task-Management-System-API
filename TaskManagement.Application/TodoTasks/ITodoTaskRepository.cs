using TaskManagement.Application.TodoTasks.Dtos;
using TaskManagement.Application.Utils;

namespace TaskManagement.Application.TodoTasks;

public interface ITodoTaskRepository
{
    Task<OperationResponse<CreateTodoTaskResponse>> CreateAsync(CreateTodoTaskRequest request);
}

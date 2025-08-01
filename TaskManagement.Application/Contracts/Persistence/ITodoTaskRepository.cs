using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.Application.Contracts.Persistence;

public interface ITodoTaskRepository : IRepository<TodoTask>
{

}

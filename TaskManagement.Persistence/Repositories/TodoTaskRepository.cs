using TaskManagement.Application.Contracts.Persistence;
using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.Persistence.Repositories;

public class TodoTaskRepository : Repository<TodoTask>, ITodoTaskRepository
{
    public TodoTaskRepository(TaskManagementDbContext context) : base(context)
    {
    }
}

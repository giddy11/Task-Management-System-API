using TaskManagement.Application.TodoTasks.Dtos;

namespace TaskManagement.Application.Labels.Dtos;

public class GetLabelResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;
    public Guid CreatedById { get; set; }
    public UserDto CreatedBy { get; set; } = default!;
}

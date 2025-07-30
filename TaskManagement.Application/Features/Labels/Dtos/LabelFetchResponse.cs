using TaskManagement.Application.Features.TodoTasks.Dtos;

namespace TaskManagement.Application.Features.Labels.Dtos;

public class LabelFetchResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;
    public Guid CreatedById { get; set; }
    public UserDto CreatedBy { get; set; } = default!;
}

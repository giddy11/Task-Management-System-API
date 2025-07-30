namespace TaskManagement.Application.Features.TodoTasks.Dtos;

public class LabelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;
    public Guid CreatedById { get; set; }
    public UserDto CreatedBy { get; set; } = default!;
}

namespace TaskManagement.Application.TodoTasks.Dtos;

public class LabelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;
}

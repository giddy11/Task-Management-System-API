namespace TaskManagement.Application.Labels.Dtos;

public class CreateLabelResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;
    public Guid CreatedById { get; set; }
}

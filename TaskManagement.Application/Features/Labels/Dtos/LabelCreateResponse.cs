namespace TaskManagement.Application.Features.Labels.Dtos;

public class LabelCreateResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;
    public Guid CreatedById { get; set; }
}

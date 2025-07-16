namespace TaskManagement.Domain;

public class Label
{
    public Guid Id { get; init; }
    public string Name { get; set; } = default!;
    public string? Color { get; set; }
}

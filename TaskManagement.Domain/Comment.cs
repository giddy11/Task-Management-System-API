using TaskManagement.Domain.TodoTasks;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Domain;

public class Comment
{
    protected Comment() { }

    public static Comment New(
        Guid userId,
        Guid todoTaskId,
        string content,
        Guid? id = null)
    {
        return new Comment
        {
            Id = id ?? Guid.NewGuid(),
            UserId = userId,
            TodoTaskId = todoTaskId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string content)
    {
        Content = content;
        UpdatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public Guid TodoTaskId { get; set; }
    public User User { get; set; } = default!;
    public TodoTask TodoTask { get; set; } = default!;
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}

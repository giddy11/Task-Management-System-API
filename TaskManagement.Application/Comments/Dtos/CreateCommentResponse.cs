namespace TaskManagement.Application.Comments.Dtos;

public class CreateCommentResponse
{
    public Guid Id { get; set; }
    //public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

namespace TaskManagement.Application.TodoTasks.Dtos
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public UserDto Author { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedBy { get; set; } = default!;
    }
}

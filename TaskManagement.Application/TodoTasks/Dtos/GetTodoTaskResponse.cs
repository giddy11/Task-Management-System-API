using TaskManagement.Domain.TodoTasks;

namespace TaskManagement.Application.TodoTasks.Dtos;

public class GetTodoTaskResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public Guid CreatedById { get; set; }
    public UserDto CreatedBy { get; set; } = default!;
    public List<UserDto> Assignees { get; set; } = [];
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<CommentDto> Comments { get; set; } = [];
    public List<LabelDto> Labels { get; set; } = [];
    public Guid ProjectId { get; set; }
    public string ProjectTitle { get; set; } = default!;
    public TodoTaskStatus TodoTaskStatus { get; set; }
    public PriorityStatus PriorityStatus { get; set; }
}

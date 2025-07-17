using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Comments.Dtos;

public class CreateCommentRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid TaskId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Content { get; set; } = default!;
}

using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Comments.Dtos;

public class UpdateCommentRequest
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Content { get; set; } = default!;
}

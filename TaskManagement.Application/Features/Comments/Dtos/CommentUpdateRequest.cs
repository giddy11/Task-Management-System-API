using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Features.Comments.Dtos;

public class CommentUpdateRequest
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Content { get; set; } = default!;
}

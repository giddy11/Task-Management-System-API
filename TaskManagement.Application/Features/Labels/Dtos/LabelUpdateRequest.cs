using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Features.Labels.Dtos;

public class LabelUpdateRequest
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = default!;

    [Required]
    [StringLength(7)]
    public string Color { get; set; } = default!;
}

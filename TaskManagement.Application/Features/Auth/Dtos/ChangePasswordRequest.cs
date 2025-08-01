using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Features.Auth.Dtos;

public class ChangePasswordRequest
{
    [Required]
    public string Token { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
    public string NewPassword { get; set; } = default!;
}

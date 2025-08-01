using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Features.Auth.Dtos;

public class VerifyEmailRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Verification code must be 6 characters.")]
    public string Code { get; set; } = default!;
}

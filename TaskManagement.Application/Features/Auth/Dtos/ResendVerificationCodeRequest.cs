using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Features.Auth.Dtos;

public class ResendVerificationCodeRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
}

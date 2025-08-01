using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Features.Auth.Dtos;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
}

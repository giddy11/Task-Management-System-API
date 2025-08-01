using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Features.Auth.Dtos;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = default!;
}

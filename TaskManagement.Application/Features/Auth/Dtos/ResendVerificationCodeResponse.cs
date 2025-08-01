namespace TaskManagement.Application.Features.Auth.Dtos;

public class ResendVerificationCodeResponse
{
    public string Message { get; set; } = "If the email exists and is not yet verified, a new verification code has been sent.";
}

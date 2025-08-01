namespace TaskManagement.Application.Contracts.Infrastructure;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}

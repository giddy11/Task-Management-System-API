using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Contracts.Infrastructure;
using TaskManagement.Infrastructure.EmailService;

namespace TaskManagement.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IEmailService, SendGridServer>();
        return services;
    }
}

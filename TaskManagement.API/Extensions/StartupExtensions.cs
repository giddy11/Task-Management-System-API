using TaskManagement.Application;
using TaskManagement.Persistence;
using TaskManagement.Infrastructure;

namespace TaskManagement.API.Extensions;

public static class StartupExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddPersistenceServices(connectionString);
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Services.AddInfrastructureServices(builder.Configuration);
        return builder.Build();
    }
}

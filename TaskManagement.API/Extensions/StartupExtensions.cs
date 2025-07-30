using TaskManagement.Application;
using TaskManagement.Persistence;

namespace TaskManagement.API.Extensions;

public static class StartupExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddPersistenceServices(connectionString);
        builder.Services.AddApplicationServices(builder.Configuration);
        return builder.Build();
    }
}

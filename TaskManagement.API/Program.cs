using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TaskManagement.Application.Labels;
using TaskManagement.Application.Mappings;
using TaskManagement.Application.Projects;
using TaskManagement.Application.TodoTasks;
using TaskManagement.Application.UserManagement;
using TaskManagement.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("TaskManagementDbConnection");
builder.Services.AddDbContext<TaskManagementDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();
builder.Services.AddScoped<ILabelRepository, LabelRepository>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(MappingProfile).Assembly);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
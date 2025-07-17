using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using TaskManagement.Application.Comments;
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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Management Application API",
        Version = "v1",
        Description = "Documentation on Task Management Application API",
        Contact = new OpenApiContact
        {
            Name = "Edoghotu Gideon Azibaobuom",
            Email = "gideon.edoghotu@cyphercrescent.com",
            Url = new Uri("https://yourwebsite.com")
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();
builder.Services.AddScoped<ILabelRepository, LabelRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();


builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(MappingProfile).Assembly);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API V1");
        //c.RoutePrefix = string.Empty; // Makes Swagger UI available at the root (e.g., http://localhost:7000/)
        c.RoutePrefix = "/index";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
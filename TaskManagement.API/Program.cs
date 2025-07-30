using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using TaskManagement.API;
using TaskManagement.API.Extensions;
using TaskManagement.Domain.UserManagement;
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
//builder.Services.AddPersistenceServices(connectionString!);
builder.Services.AddDbContext<TaskManagementDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
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
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
////builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IAuthRepository, AuthRepository>();
////builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
//builder.Services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();
//builder.Services.AddScoped<ILabelRepository, LabelRepository>();
//builder.Services.AddScoped<ICommentRepository, CommentRepository>();
//builder.Services.AddAutoMapper(cfg =>
//{
//    cfg.AddMaps(typeof(MappingProfile).Assembly);
//});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(AccountTypes.Admin.ToString()));
    options.AddPolicy("UserOrAbove", policy => policy.RequireRole(AccountTypes.User.ToString(), AccountTypes.Admin.ToString()));
});

//var app = builder.Build();
var app = builder.ConfigureServices(connectionString!);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API V1");
        c.RoutePrefix = string.Empty;
    });

    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<TaskManagementDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher<User>>();
        logger.LogInformation("Migrating database...");
        context.Database.Migrate();
        DatabaseSeeder.Seed(context, passwordHasher);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();
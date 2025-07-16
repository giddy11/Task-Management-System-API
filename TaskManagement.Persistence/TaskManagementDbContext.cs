using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.TodoTasks;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Persistence;

public class TaskManagementDbContext : DbContext
{
    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskManagementDbContext).Assembly);
    }

    // Core Domain Entities
    public DbSet<Project> Projects { get; set; }
    public DbSet<TodoTask> Tasks { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Label> Labels { get; set; }


    //user management entities
    public DbSet<User> Users { get; set; }
}

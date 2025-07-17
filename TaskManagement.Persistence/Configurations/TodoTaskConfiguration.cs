using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain;
using TaskManagement.Domain.TodoTasks;
using TaskManagement.Domain.UserManagement;
using TaskManagement.Persistence.Utils;

namespace TaskManagement.Persistence.Configurations;

public class TodoTaskConfiguration : IEntityTypeConfiguration<TodoTask>
{
    public void Configure(EntityTypeBuilder<TodoTask> builder)
    {
        builder.ToTable(nameof(TaskManagementDbContext.Tasks), schema: PersistenceConstants.TaskManagement_SCHEMA);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(t => t.Description)
               .HasMaxLength(1000);

        builder.Property(t => t.StartDate)
               .IsRequired(false);

        builder.Property(t => t.EndDate)
               .IsRequired(false);

        builder.Property(t => t.TodoTaskStatus)
               .IsRequired()
               .HasConversion<string>();

        builder.Property(t => t.PriorityStatus)
               .IsRequired()
               .HasConversion<string>();

        builder.Property(t => t.ProjectId)
               .IsRequired();

        builder.Property(p => p.CreatedById)
               .IsRequired();

        builder.HasOne(t => t.CreatedBy)
               .WithMany() 
               .HasForeignKey(t => t.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Assignees)
                .WithMany(t => t.TodoTasks)
                .UsingEntity<Dictionary<string, object>>(
                    "TodoTaskAssignee",
                    j => j.HasOne<User>()
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<TodoTask>()
                            .WithMany()
                            .HasForeignKey("TodoTaskId")
                            .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("TodoTaskId", "UserId");
                        j.ToTable("TodoTaskAssignee");
                        j.Property<Guid>("TodoTaskId");
                        j.Property<Guid>("UserId");
                    });

        builder.HasOne(t => t.Project)
               .WithMany(p => p.TodoTasks)
               .HasForeignKey(t => t.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Comments)
               .WithOne(c => c.TodoTask)
               .HasForeignKey(c => c.TodoTaskId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Labels)
               .WithMany(t => t.TodoTasks)
               .UsingEntity<Dictionary<string, object>>(
                   "TodoTaskLabels",
                   j => j.HasOne<Label>()
                         .WithMany()
                         .HasForeignKey("LabelId")
                         .OnDelete(DeleteBehavior.Cascade),
                   j => j.HasOne<TodoTask>()
                         .WithMany()
                         .HasForeignKey("TodoTaskId")
                         .OnDelete(DeleteBehavior.Cascade),
                   j =>
                   {
                       j.HasKey("TodoTaskId", "LabelId");
                       j.ToTable("TodoTaskLabels");
                       j.Property<Guid>("TodoTaskId");
                       j.Property<Guid>("LabelId");
                   });
    }
}

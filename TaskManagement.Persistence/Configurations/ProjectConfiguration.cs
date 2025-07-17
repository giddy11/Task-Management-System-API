using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Projects;
using TaskManagement.Persistence.Utils;

namespace TaskManagement.Persistence.Configurations;
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable(nameof(TaskManagementDbContext.Projects), schema: PersistenceConstants.TaskManagement_SCHEMA);

        builder.Property(p => p.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(p => p.Description)
               .HasMaxLength(1000);

        builder.Property(p => p.StartDate)
               .IsRequired();

        builder.Property(p => p.EndDate)
               .IsRequired();

        builder.Property(p => p.ProjectStatus)
               .IsRequired()
               .HasConversion<string>()
               .HasDefaultValue(ProjectStatus.Not_Started);

        builder.Property(p => p.CreatedById)
               .IsRequired();

        builder.HasOne(p => p.CreatedBy)
               .WithMany()
               .HasForeignKey(p => p.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.TodoTasks)
               .WithOne(t => t.Project)
               .HasForeignKey(t => t.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
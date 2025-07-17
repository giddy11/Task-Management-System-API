using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain;
using TaskManagement.Persistence.Utils;

namespace TaskManagement.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable(nameof(TaskManagementDbContext.Comments), schema: PersistenceConstants.TaskManagement_SCHEMA);

        builder.Property(c => c.Content).IsRequired();
        builder.Property(c => c.UserId).IsRequired();
        builder.Property(c => c.TodoTaskId).IsRequired();

        builder.HasOne(c => c.User)
               .WithMany() 
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.TodoTask)
               .WithMany(t => t.Comments)
               .HasForeignKey(c => c.TodoTaskId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

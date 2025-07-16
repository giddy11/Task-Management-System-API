using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.UserManagement;
using TaskManagement.Persistence.Utils;

namespace TaskManagement.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(TaskManagementDbContext.Users), schema: PersistenceConstants.TaskManagement_SCHEMA);

        // Properties
        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(u => u.FirstName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.LastName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
               .HasMaxLength(500);

        builder.Property(u => u.AccountType)
               .IsRequired()
               .HasConversion<string>();

        builder.Property(u => u.UserStatus)
               .IsRequired()
               .HasConversion<string>()
               .HasDefaultValue(UserStatus.Active);

        builder.HasIndex(u => u.Email)
               .IsUnique()
               .HasDatabaseName("IX_Users_Email");
    }
}

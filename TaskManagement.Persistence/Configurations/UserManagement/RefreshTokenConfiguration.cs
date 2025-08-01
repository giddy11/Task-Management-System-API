using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.UserManagement;
using TaskManagement.Persistence.Utils;

namespace TaskManagement.Persistence.Configurations.UserManagement;

public class RefreshTokenConfiguration
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable(nameof(TaskManagementDbContext.RefreshTokens), schema: PersistenceConstants.TaskManagement_SCHEMA);
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Token).IsRequired().HasMaxLength(256);
        builder.Property(rt => rt.ExpiryDate).IsRequired();
        builder.Property(rt => rt.IsRevoked).IsRequired();
        builder.HasIndex(r => r.UserId);
        builder.HasOne(rt => rt.User)
               .WithMany()
               .HasForeignKey(rt => rt.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

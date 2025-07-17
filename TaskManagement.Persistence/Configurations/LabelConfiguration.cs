using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain;
using TaskManagement.Persistence.Utils;

namespace TaskManagement.Persistence.Configurations;

public class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable(nameof(TaskManagementDbContext.Labels), schema: PersistenceConstants.TaskManagement_SCHEMA);

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(l => l.Color)
               .HasMaxLength(7)
               .IsRequired(false);

        builder.HasIndex(l => l.Name)
               .IsUnique()
               .HasDatabaseName("IX_Labels_Name_Unique");
    }
}

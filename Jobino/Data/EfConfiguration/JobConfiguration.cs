
using Jobino.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobino.Data.EfConfigurations;

public class JobConfiguration
    : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("Jobs");

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Jobs)
            .HasForeignKey(x => x.JobCategoryId);

        builder.HasOne(x => x.Company)
            .WithMany(x => x.Jobs)
            .HasForeignKey(x => x.CompanyId);

    }
}

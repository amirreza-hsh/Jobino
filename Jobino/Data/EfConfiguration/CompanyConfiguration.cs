
using Jobino.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobino.Data.EfConfigurations;

public class CompanyConfiguration
    : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PersianName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.EnglishName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.LogoUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Website)
            .HasMaxLength(250);

        builder.Property(x => x.EmployeeCountRange)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Province)
            .HasMaxLength(100);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);
    }
}


using Jobino.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobino.Data.EfConfigurations;

public class CompanyIndustryEntityConfiguration
    : IEntityTypeConfiguration<CompanyIndustry>
{
    public void Configure(EntityTypeBuilder<CompanyIndustry> builder)
    {
        builder.ToTable("CompanyIndustries");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Company)
            .WithMany(x => x.CompanyIndustries)
            .HasForeignKey(x => x.CompanyId);

        builder.HasOne(x => x.Industry)
            .WithMany(x => x.CompanyIndustries)
            .HasForeignKey(x => x.IndustryId);
    }
}

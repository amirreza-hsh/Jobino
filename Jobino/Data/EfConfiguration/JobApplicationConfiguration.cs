using Jobino.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobino.Data.EfConfigurations;

public class JobApplicationConfiguration
    : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.ToTable("JobApplications");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Job)
            .WithMany()
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Resume)
            .WithMany()
            .HasForeignKey(x => x.ResumeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
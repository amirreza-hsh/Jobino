using Jobino.Data.EfConfigurations;
using Jobino.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jobino.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<JobCategory> JobCategories { get; set; } = null!;
    public DbSet<Job> Jobs { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;

    public DbSet<Industry> Industries { get; set; } = null!;

    public DbSet<CompanyIndustry> CompanyIndustries { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(JobCategoryEntityConfiguration).Assembly);
    }
}
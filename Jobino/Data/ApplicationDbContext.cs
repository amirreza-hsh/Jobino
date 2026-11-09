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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(JobCategoryEntityConfiguration).Assembly);
    }
}
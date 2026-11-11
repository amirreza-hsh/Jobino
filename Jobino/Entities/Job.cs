
using Jobino.Entities;

namespace Jobino.Entities;

public class Job : BaseEntitiy
{
    public int JobCategoryId { get; set; }
    public int CompanyId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long? Salary { get; set; }
    public string City { get; set; } = null!;

    public EmploymentType EmploymentType { get; set; }

    public bool IsRemote { get; set; }

    public bool IsInternship { get; set; }

    public JobCategory Category { get; set; } = null!;
    public Company Company { get; set; } = null!;
}

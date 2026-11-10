using Jobino.Entities;

namespace Jobino.DTOs.Jobs;

public class GetJobResponseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public long? Salary { get; set; }

    public string City { get; set; } = null!;

    public int JobCategoryId { get; set; }

    public string JobCategoryName { get; set; } = null!;

    public EmploymentType EmploymentType { get; set; }

    public bool IsRemote { get; set; }
    public bool IsInternship { get; set; }

    public DateTime DateCreated { get; set; }
}
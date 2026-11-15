using Jobino.Entities;

namespace Jobino.DTOs.JobApplications;

public class GetMyJobApplicationResponseDto
{
    public int Id { get; set; }

    public int JobId { get; set; }

    public string JobTitle { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public JobApplicationStatus Status { get; set; }

    public DateTime DateCreated { get; set; }
}
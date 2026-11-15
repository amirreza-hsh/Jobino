using Jobino.Entities;

namespace Jobino.DTOs.JobApplications;

public class GetEmployerJobApplicationResponseDto
{
    public int Id { get; set; }

    public int JobId { get; set; }

    public string JobTitle { get; set; } = null!;

    public string ApplicantName { get; set; } = null!;

    public string ResumeTitle { get; set; } = null!;

    public JobApplicationStatus Status { get; set; }
}
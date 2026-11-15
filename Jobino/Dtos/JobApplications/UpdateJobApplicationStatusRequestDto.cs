using Jobino.Entities;

namespace Jobino.DTOs.JobApplications;

public class UpdateJobApplicationStatusRequestDto
{
    public JobApplicationStatus Status { get; set; }
}
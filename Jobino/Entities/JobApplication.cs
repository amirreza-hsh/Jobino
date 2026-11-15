namespace Jobino.Entities;

public class JobApplication : BaseEntitiy
{
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int ResumeId { get; set; }
    public Resume Resume { get; set; } = null!;

    public JobApplicationStatus Status { get; set; }
        = JobApplicationStatus.Submitted;
}
namespace Jobino.Entities;

public class SavedJob : BaseEntitiy
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int JobId { get; set; }
    public Job Job { get; set; } = null!;
}
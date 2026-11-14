namespace Jobino.Entities;

public class Resume : BaseEntitiy
{
    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public User User { get; set; } = null!;
}
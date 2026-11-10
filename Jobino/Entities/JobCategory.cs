namespace Jobino.Entities;

public class JobCategory : BaseEntitiy
{

    public string Name { get; set; } = null!;
    public List<Job> Jobs { get; set; } = null!;
}

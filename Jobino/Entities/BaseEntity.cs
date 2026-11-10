namespace Jobino.Entities;
public abstract class BaseEntitiy
{
    public int Id { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}

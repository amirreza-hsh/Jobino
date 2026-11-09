namespace Jobino.Entities;
public abstract class BaseEntitiy
{
    public int id { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}

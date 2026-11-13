
namespace Jobino.Entities;

public class Role : BaseEntitiy
{
    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public List<UserRole> UserRoles { get; set; } = null!;
}



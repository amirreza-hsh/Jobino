
namespace Jobino.Entities;

public class User : BaseEntitiy
{
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public List<UserRole> UserRoles { get; set; } = null!;
}

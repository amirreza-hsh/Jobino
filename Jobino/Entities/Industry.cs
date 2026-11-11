
namespace Jobino.Entities;

public class Industry : BaseEntitiy
{
    public string Name { get; set; } = null!;

    public List<CompanyIndustry> CompanyIndustries { get; set; } = null!;
}

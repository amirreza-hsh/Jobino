
namespace Jobino.Entities;

public class CompanyIndustry : BaseEntitiy
{
    public int CompanyId { get; set; }

    public int IndustryId { get; set; }

    public Company Company { get; set; } = null!;

    public Industry Industry { get; set; } = null!;
}


namespace Jobino.Entities;

public class Company : BaseEntitiy
{
    public string PersianName { get; set; } = null!;

    public string EnglishName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string LogoUrl { get; set; } = null!;


    public string? Website { get; set; }

    public string? Description { get; set; }

    public int? FoundedYear { get; set; }
    public string EmployeeCountRange { get; set; } = null!;
    public string? Province { get; set; }
    public string? City { get; set; }

    public List<Job> Jobs { get; set; } = null!;
    public List<CompanyIndustry> CompanyIndustries { get; set; } = null!;

}

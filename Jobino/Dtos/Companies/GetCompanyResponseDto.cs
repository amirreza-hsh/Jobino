
namespace Jobino.DTOs.Companies;

public class GetCompanyResponseDto
{
    public int Id { get; set; }

    public string PersianName { get; set; } = null!;

    public string EnglishName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? LogoUrl { get; set; }
    public string? Website { get; set; }

    public string? Description { get; set; }

    public int? FoundedYear { get; set; }

    public string EmployeeCountRange { get; set; } = null!;

    public string? Province { get; set; }

    public string? City { get; set; }

    public DateTime DateCreated { get; set; }
}

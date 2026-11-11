
using System.ComponentModel.DataAnnotations;

namespace Jobino.DTOs.Companies;

public class CreateCompanyRequestDto
{
    [Required(ErrorMessage = "نام فارسی شرکت الزامی است")]
    [MaxLength(150)]
    public string PersianName { get; set; } = null!;

    [Required(ErrorMessage = "نام انگلیسی شرکت الزامی است")]
    [MaxLength(150)]
    public string EnglishName { get; set; } = null!;

    [Required(ErrorMessage = "شماره تماس الزامی است")]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = null!;

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [MaxLength(250)]
    public string? Website { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public int? FoundedYear { get; set; }

    [Required(ErrorMessage = "تعداد کارکنان الزامی است")]
    [MaxLength(50)]
    public string EmployeeCountRange { get; set; } = null!;

    [MaxLength(100)]
    public string? Province { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }
}

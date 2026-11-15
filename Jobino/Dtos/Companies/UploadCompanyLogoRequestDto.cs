using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Jobino.DTOs.Companies;

public class UploadCompanyLogoRequestDto
{
    [Required(ErrorMessage = "فایل لوگو الزامی است")]
    public IFormFile Logo { get; set; } = null!;
}
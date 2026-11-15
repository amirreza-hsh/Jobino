using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Jobino.DTOs.Resumes;

public class CreateResumeRequestDto
{
    [Required(ErrorMessage = "عنوان رزومه الزامی است")]
    [MaxLength(150, ErrorMessage = "عنوان رزومه حداکثر ۱۵۰ کاراکتر است")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "فایل رزومه الزامی است")]
    public IFormFile File { get; set; } = null!;
}
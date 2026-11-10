using System.ComponentModel.DataAnnotations;

namespace Jobino.DTOs.JobCategories;

public class CreateJobCategoryRequestDto
{
    [Required(ErrorMessage = "نام دسته‌بندی الزامی می‌باشد")]

    [MaxLength(100, ErrorMessage = "نام نمی‌تواند بیشتر از 100 کاراکتر باشد")]
    public string Name { get; set; } = null!;
}
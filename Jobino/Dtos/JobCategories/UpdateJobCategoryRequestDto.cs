using System.ComponentModel.DataAnnotations;

namespace Jobino.DTOs.JobCategories;

public class UpdateJobCategoryRequestDto
{

    [Required(ErrorMessage = "نام دسته‌بندی الزامی است")]
    [MaxLength(100, ErrorMessage = "نام نمی‌تواند بیشتر از 100 کاراکتر باشد")]
    public string Name { get; set; } = null!;
}
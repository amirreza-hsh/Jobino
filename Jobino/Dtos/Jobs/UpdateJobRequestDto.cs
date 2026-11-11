using Jobino.Entities;
using System.ComponentModel.DataAnnotations;

namespace Jobino.DTOs.Jobs;

public class UpdateJobRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "دسته‌بندی معتبر نیست")]
    public int JobCategoryId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "شرکت معتبر نیست")]
    public int CompanyId { get; set; }

    [Required(ErrorMessage = "عنوان آگهی الزامی است")]
    [MaxLength(100, ErrorMessage = "عنوان نمی‌تواند بیشتر از 100 کاراکتر باشد")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "توضیحات آگهی الزامی است")]
    public string Description { get; set; } = null!;

    [Range(0, long.MaxValue, ErrorMessage = "حقوق نمی‌تواند منفی باشد")]
    public long? Salary { get; set; }

    [Required(ErrorMessage = "شهر محل کار الزامی است")]
    [MaxLength(100, ErrorMessage = "نام شهر نمی‌تواند بیشتر از 100 کاراکتر باشد")]
    public string City { get; set; } = null!;

    [Range(1, 3, ErrorMessage = "لطفاً نوع همکاری را انتخاب کنید")]
    public EmploymentType EmploymentType { get; set; }

    public bool IsRemote { get; set; }

    public bool IsInternship { get; set; }
}
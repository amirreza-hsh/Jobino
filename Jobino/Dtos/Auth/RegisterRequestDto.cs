
using System.ComponentModel.DataAnnotations;

namespace Jobino.DTOs.Auth;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "نام و نام خانوادگی الزامی است")]
    [MaxLength(150)]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "ایمیل الزامی است")]
    [EmailAddress(ErrorMessage = "فرمت ایمیل معتبر نیست")]
    [MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [MinLength(8, ErrorMessage = "رمز عبور باید حداقل ۸ کاراکتر باشد")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "تکرار رمز عبور الزامی است")]
    [Compare(nameof(Password),
        ErrorMessage = "رمز عبور و تکرار آن یکسان نیستند")]
    public string ConfirmPassword { get; set; } = null!;
}

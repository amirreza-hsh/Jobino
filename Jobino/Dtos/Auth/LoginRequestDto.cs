using System.ComponentModel.DataAnnotations;

namespace Jobino.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "ایمیل الزامی است")]
    [EmailAddress(ErrorMessage = "فرمت ایمیل معتبر نیست")]
    [MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [MinLength(8, ErrorMessage = "رمز عبور باید حداقل ۸ کاراکتر باشد")]
    public string Password { get; set; } = null!;
}
using Jobino.Data;
using Jobino.DTOs.Auth;
using Jobino.Entities;
using Jobino.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Jobino.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    ApplicationDbContext context,
    IPasswordHasherService passwordHasherService,
    IJwtService jwtService)
    : ControllerBase
{
    [HttpPost("register")]
    public Task<ActionResult> Register(RegisterRequestDto request)
    {
        return RegisterUser(request, "JOBSEEKER", "کارجو");
    }

    [HttpPost("register-employer")]
    public Task<ActionResult> RegisterEmployer(RegisterRequestDto request)
    {
        return RegisterUser(request, "EMPLOYER", "کارفرما");
    }

    private async Task<ActionResult> RegisterUser(
        RegisterRequestDto request,
        string roleName,
        string roleDisplayName)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await context.Users.AnyAsync(x => x.Email == normalizedEmail))
        {
            return BadRequest("این ایمیل قبلاً ثبت شده است");
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest("نام و نام خانوادگی معتبر نیست");
        }
        var hashedPassword = passwordHasherService.Hash(request.Password);

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = hashedPassword
        };

        var role = await context.Roles
            .FirstOrDefaultAsync(x => x.Name == roleName);

        if (role is null)
        {
            role = new Role
            {
                Name = roleName,
                DisplayName = roleDisplayName
            };

            context.Roles.Add(role);
        }

        var userRole = new UserRole
        {
            User = user,
            Role = role
        };

        context.Users.Add(user);
        context.UserRoles.Add(userRole);

        await context.SaveChangesAsync();

        return StatusCode(201, new
        {
            Id = user.Id,
            Message = "ثبت‌ نام با موفقیت انجام شد"
        });
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult> GetMe()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var user = await context.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            return NotFound("حساب کاربری یافت نشد");
        }

        return Ok(new
        {
            user.Id,
            user.FullName,
            user.Email,
            Roles = user.UserRoles.Select(x => x.Role.Name).ToList()
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await context.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail);

        var errorMessage = "ایمیل یا رمز عبور صحیح نیست";

        if (user is null)
        {
            return BadRequest(errorMessage);
        }

        var isPasswordCorrect = passwordHasherService
            .Verify(user.PasswordHash, request.Password);

        if (!isPasswordCorrect)
        {
            return BadRequest(errorMessage);
        }

        var token = jwtService.GenerateToken(user);

        return Ok(new
        {
            Token = token
        });
    }
}


using Jobino.Data;
using Jobino.DTOs.Auth;
using Jobino.Entities;
using Jobino.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobino.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    ApplicationDbContext context,
    IPasswordHasherService passwordHasherService,
    IJwtService jwtService)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(
        RegisterRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await context.Users
            .AnyAsync(x => x.Email == normalizedEmail))
        {
            return BadRequest("این ایمیل قبلاً ثبت شده است");
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest("نام و نام خانوادگی معتبر نیست");
        }

        var hashedPassword = passwordHasherService.Hash(
            request.Password);

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = hashedPassword
        };

        var jobSeekerRole = await context.Roles
            .FirstOrDefaultAsync(x => x.Name == "JOBSEEKER");

        if (jobSeekerRole is null)
        {
            jobSeekerRole = new Role
            {
                Name = "JOBSEEKER",
                DisplayName = "کارجو"
            };

            context.Roles.Add(jobSeekerRole);
        }

        var userRole = new UserRole
        {
            User = user,
            Role = jobSeekerRole
        };

        context.Users.Add(user);
        context.UserRoles.Add(userRole);

        await context.SaveChangesAsync();

        return StatusCode(201, new
        {
            Id = user.Id,
            Message = "ثبت ‌نام با موفقیت انجام شد"
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

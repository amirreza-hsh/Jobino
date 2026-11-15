using System.Security.Claims;
using Jobino.Data;
using Jobino.DTOs.Resumes;
using Jobino.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobino.Controllers;

[Authorize(Roles = "JOBSEEKER")]
[Route("api/[controller]")]
[ApiController]
public class ResumesController(ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create([FromForm] CreateResumeRequestDto request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        if (request.File.Length == 0 || request.File.Length > 4 * 1024 * 1024)
        {
            return BadRequest("حجم فایل باید بیشتر از صفر و حداکثر 4 مگابایت باشد");
        }

        var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

        if (extension != ".pdf")
        {
            return BadRequest("فقط فایل با پسوند PDF مجاز است");
        }

        var localAppData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);

        var uploadFolder = Path.Combine(localAppData, "Jobino", "Resumes");
        Directory.CreateDirectory(uploadFolder);

        var fileName = $"{Guid.NewGuid()}.pdf";
        var fullPath = Path.Combine(uploadFolder, fileName);

        await using (var stream = new FileStream(fullPath, FileMode.CreateNew))
        {
            await request.File.CopyToAsync(stream);
        }

        var resume = new Resume
        {
            UserId = userId,
            Title = request.Title.Trim(),
            FilePath = fileName
        };

        context.Resumes.Add(resume);
        await context.SaveChangesAsync();

        return StatusCode(201, new { Id = resume.Id });
    }

    [HttpGet("{id}/download")]
    public async Task<ActionResult> Download(int id)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var fileName = await context.Resumes
            .Where(x => x.Id == id && x.UserId == userId)
            .Select(x => x.FilePath)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return NotFound("رزومهٔ موردنظر یافت نشد");
        }

        var localAppData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);

        var fullPath = Path.Combine(localAppData, "Jobino", "Resumes", fileName);

        if (!System.IO.File.Exists(fullPath))
        {
            return NotFound("فایل رزومه یافت نشد");
        }

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

        return File(stream, "application/pdf", "resume.pdf");
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var resumes = await context.Resumes
            .Where(x => x.UserId == userId)
            .Select(x => new
            {
                x.Id,
                x.Title
            })
            .ToListAsync();

        return Ok(resumes);
    }
}
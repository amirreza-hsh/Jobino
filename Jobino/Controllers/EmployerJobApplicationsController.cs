using Jobino.Data;
using Jobino.DTOs.JobApplications;
using Jobino.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Jobino.Controllers;

[Authorize(Roles = "EMPLOYER")]
[Route("api/[controller]")]
[ApiController]
public class EmployerJobApplicationsController(ApplicationDbContext context)
    : ControllerBase
{
    [HttpPut("{id}/status")]
    public async Task<ActionResult> UpdateStatus(
        int id,
        UpdateJobApplicationStatusRequestDto request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var application = await context.JobApplications
            .FirstOrDefaultAsync(x => x.Id == id
                && x.Job.Company.OwnerUserId == userId);

        if (application is null)
        {
            return NotFound("درخواست استخدام موردنظر یافت نشد");
        }

        if (request.Status != JobApplicationStatus.UnderReview
            && request.Status != JobApplicationStatus.Interview
            && request.Status != JobApplicationStatus.Rejected
            && request.Status != JobApplicationStatus.Hired)
        {
            return BadRequest("وضعیت انتخاب‌شده برای کارفرما معتبر نیست");
        }

        if (application.Status == JobApplicationStatus.Rejected
            || application.Status == JobApplicationStatus.Hired
            || application.Status == JobApplicationStatus.Withdrawn)
        {
            return BadRequest("وضعیت این درخواست دیگر قابل تغییر نیست");
        }

        if (request.Status == JobApplicationStatus.Hired
            && application.Status != JobApplicationStatus.Interview)
        {
            return BadRequest("برای ثبت استخدام، درخواست باید در وضعیت مصاحبه باشد");
        }

        application.Status = request.Status;
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/resume")]
    public async Task<ActionResult> DownloadResume(int id)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var fileName = await context.JobApplications
            .Where(x => x.Id == id && x.Job.Company.OwnerUserId == userId)
            .Select(x => x.Resume.FilePath)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return NotFound("رزومهٔ این درخواست یافت نشد");
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

        var applications = await context.JobApplications
            .Where(x => x.Job.Company.OwnerUserId == userId)
            .Select(x => new GetEmployerJobApplicationResponseDto
            {
                Id = x.Id,
                JobId = x.JobId,
                JobTitle = x.Job.Title,
                ApplicantName = x.Resume.User.FullName,
                ResumeTitle = x.Resume.Title,
                Status = x.Status
            })
            .ToListAsync();

        return Ok(applications);
    }
}
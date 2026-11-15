using System.Security.Claims;
using Jobino.Data;
using Jobino.DTOs.JobApplications;
using Jobino.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobino.Controllers;

[Authorize(Roles = "JOBSEEKER")]
[Route("api/[controller]")]
[ApiController]
public class JobApplicationsController(ApplicationDbContext context)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(CreateJobApplicationRequestDto request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        if (!await context.Jobs.AnyAsync(x => x.Id == request.JobId))
        {
            return NotFound("آگهی موردنظر یافت نشد");
        }

        var ownsResume = await context.Resumes
            .AnyAsync(x => x.Id == request.ResumeId && x.UserId == userId);

        if (!ownsResume)
        {
            return NotFound("رزومهٔ موردنظر یافت نشد");
        }

        var alreadyApplied = await context.JobApplications
            .AnyAsync(x => x.JobId == request.JobId
                && x.Resume.UserId == userId);

        if (alreadyApplied)
        {
            return BadRequest("قبلاً برای این آگهی درخواست فرستاده‌ای");
        }

        var application = new JobApplication
        {
            JobId = request.JobId,
            ResumeId = request.ResumeId,
            Status = JobApplicationStatus.Submitted
        };

        context.JobApplications.Add(application);
        await context.SaveChangesAsync();

        return StatusCode(201, new { Id = application.Id });
    }
}
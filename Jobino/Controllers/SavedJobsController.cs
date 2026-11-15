using System.Security.Claims;
using Jobino.Data;
using Jobino.DTOs.Jobs;
using Jobino.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobino.Controllers;

[Authorize(Roles = "JOBSEEKER")]
[Route("api/[controller]")]
[ApiController]
public class SavedJobsController(ApplicationDbContext context) : ControllerBase
{
    [HttpPost("{jobId}")]
    public async Task<ActionResult> Create(int jobId)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        if (!await context.Jobs.AnyAsync(x => x.Id == jobId && !x.IsClosed))
        {
            return NotFound("آگهی باز موردنظر یافت نشد");
        }

        if (await context.SavedJobs
            .AnyAsync(x => x.JobId == jobId && x.UserId == userId))
        {
            return BadRequest("این آگهی قبلاً نشان شده است");
        }

        var savedJob = new SavedJob
        {
            JobId = jobId,
            UserId = userId
        };

        context.SavedJobs.Add(savedJob);
        await context.SaveChangesAsync();

        return StatusCode(201, new { savedJob.JobId });
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var jobs = await context.SavedJobs
            .Where(x => x.UserId == userId)
            .Select(x => new GetJobResponseDto
            {
                Id = x.Job.Id,
                Title = x.Job.Title,
                Description = x.Job.Description,
                Salary = x.Job.Salary,
                City = x.Job.City,
                JobCategoryId = x.Job.JobCategoryId,
                JobCategoryName = x.Job.Category.Name,
                CompanyId = x.Job.CompanyId,
                CompanyName = x.Job.Company.PersianName,
                EmploymentType = x.Job.EmploymentType,
                IsRemote = x.Job.IsRemote,
                IsInternship = x.Job.IsInternship,
                IsClosed = x.Job.IsClosed,
                DateCreated = x.Job.DateCreated
            })
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpDelete("{jobId}")]
    public async Task<ActionResult> Delete(int jobId)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var savedJob = await context.SavedJobs
            .FirstOrDefaultAsync(x => x.JobId == jobId && x.UserId == userId);

        if (savedJob is null)
        {
            return NotFound("این آگهی در فهرست نشان‌شده‌های شما نیست");
        }

        context.SavedJobs.Remove(savedJob);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
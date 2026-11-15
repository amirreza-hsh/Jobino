using System.Security.Claims;
using Jobino.Data;
using Jobino.DTOs.Jobs;
using Jobino.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobino.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobsController(ApplicationDbContext context) : ControllerBase
{
    [Authorize(Roles = "EMPLOYER")]
    [HttpPost]
    public async Task<ActionResult> Create(CreateJobRequestDto request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        if (!await context.JobCategories
            .AnyAsync(x => x.Id == request.JobCategoryId))
        {
            return BadRequest("دسته‌بندی انتخاب‌شده معتبر نیست");
        }

        if (!await context.Companies
            .AnyAsync(x => x.Id == request.CompanyId
                && x.OwnerUserId == userId))
        {
            return BadRequest("شرکت انتخاب‌شده در اختیار این حساب نیست");
        }

        var job = new Job
        {
            JobCategoryId = request.JobCategoryId,
            CompanyId = request.CompanyId,
            Title = request.Title.Trim(),
            Description = request.Description,
            Salary = request.Salary,
            City = request.City.Trim(),
            EmploymentType = request.EmploymentType,
            IsRemote = request.IsRemote,
            IsInternship = request.IsInternship
        };

        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = job.Id },
            new { Id = job.Id });
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var jobs = await context.Jobs
            .Select(x => new GetJobResponseDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Salary = x.Salary,
                City = x.City,
                JobCategoryId = x.JobCategoryId,
                JobCategoryName = x.Category.Name,
                CompanyId = x.CompanyId,
                CompanyName = x.Company.PersianName,
                EmploymentType = x.EmploymentType,
                IsRemote = x.IsRemote,
                IsInternship = x.IsInternship,
                DateCreated = x.DateCreated
            })
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var job = await context.Jobs
            .Where(x => x.Id == id)
            .Select(x => new GetJobResponseDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Salary = x.Salary,
                City = x.City,
                JobCategoryId = x.JobCategoryId,
                JobCategoryName = x.Category.Name,
                CompanyId = x.CompanyId,
                CompanyName = x.Company.PersianName,
                EmploymentType = x.EmploymentType,
                IsRemote = x.IsRemote,
                IsInternship = x.IsInternship,
                DateCreated = x.DateCreated
            })
            .FirstOrDefaultAsync();

        if (job is null)
        {
            return NotFound("آگهی موردنظر یافت نشد");
        }

        return Ok(job);
    }

    [Authorize(Roles = "EMPLOYER")]
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateJobRequestDto request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var job = await context.Jobs
            .FirstOrDefaultAsync(x => x.Id == id
                && x.Company.OwnerUserId == userId);

        if (job is null)
        {
            return NotFound("آگهی موردنظر یافت نشد");
        }

        if (!await context.JobCategories
            .AnyAsync(x => x.Id == request.JobCategoryId))
        {
            return BadRequest("دسته‌بندی انتخاب‌شده معتبر نیست");
        }

        if (!await context.Companies
            .AnyAsync(x => x.Id == request.CompanyId
                && x.OwnerUserId == userId))
        {
            return BadRequest("شرکت انتخاب‌شده در اختیار این حساب نیست");
        }

        job.JobCategoryId = request.JobCategoryId;
        job.CompanyId = request.CompanyId;
        job.Title = request.Title.Trim();
        job.Description = request.Description;
        job.Salary = request.Salary;
        job.City = request.City.Trim();
        job.EmploymentType = request.EmploymentType;
        job.IsRemote = request.IsRemote;
        job.IsInternship = request.IsInternship;

        await context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "EMPLOYER")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var job = await context.Jobs
            .FirstOrDefaultAsync(x => x.Id == id
                && x.Company.OwnerUserId == userId);

        if (job is null)
        {
            return NotFound("آگهی موردنظر یافت نشد");
        }

        if (await context.JobApplications.AnyAsync(x => x.JobId == id))
        {
            return BadRequest("این آگهی درخواست استخدام دارد و قابل حذف نیست");
        }

        context.Jobs.Remove(job);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
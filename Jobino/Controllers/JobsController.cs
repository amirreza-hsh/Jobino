
using Jobino.Data;
using Jobino.DTOs.Jobs;
using Jobino.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobino.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobsController(ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(CreateJobRequestDto request)
    {
        if (!await context.JobCategories
            .AnyAsync(x => x.Id == request.JobCategoryId))
        {
            return BadRequest("دسته‌بندی انتخاب‌شده معتبر نیست");
        }

        var job = new Job
        {
            JobCategoryId = request.JobCategoryId,
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
            new { Id = job.Id }
        );
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

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(
        int id,
        UpdateJobRequestDto request)
    {
        if (!await context.JobCategories
            .AnyAsync(x => x.Id == request.JobCategoryId))
        {
            return BadRequest("دسته‌بندی انتخاب‌شده معتبر نیست");
        }

        var job = await context.Jobs
            .FirstOrDefaultAsync(x => x.Id == id);

        if (job is null)
        {
            return NotFound("آگهی موردنظر یافت نشد");
        }

        job.JobCategoryId = request.JobCategoryId;
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

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var job = await context.Jobs
            .FirstOrDefaultAsync(x => x.Id == id);

        if (job is null)
        {
            return NotFound("آگهی موردنظر یافت نشد");
        }

        context.Jobs.Remove(job);

        await context.SaveChangesAsync();

        return NoContent();
    }
}

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
    public async Task<ActionResult> Get(int page = 1, int pageSize = 10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 50)
        {
            return BadRequest("شماره صفحه باید حداقل ۱ و اندازه صفحه بین ۱ و ۵۰ باشد");
        }

        var skip = ((long)page - 1) * pageSize;

        if (skip > int.MaxValue)
        {
            return BadRequest("شماره صفحه بیش از حد بزرگ است");
        }

        var jobs = await context.Jobs
            .Where(x => !x.IsClosed)
            .OrderByDescending(x => x.DateCreated)
            .ThenByDescending(x => x.Id)
            .Skip((int)skip)
            .Take(pageSize)
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
                IsClosed = x.IsClosed,
                DateCreated = x.DateCreated
            })
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpGet("by-category/{id}")]
    public async Task<ActionResult> GetByCategory(
        int id,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 50)
        {
            return BadRequest("شماره صفحه باید حداقل ۱ و اندازه صفحه بین ۱ و ۵۰ باشد");
        }

        var skip = ((long)page - 1) * pageSize;

        if (skip > int.MaxValue)
        {
            return BadRequest("شماره صفحه بیش از حد بزرگ است");
        }

        var jobs = await context.Jobs
            .Where(x => x.JobCategoryId == id && !x.IsClosed)
            .OrderByDescending(x => x.DateCreated)
            .ThenByDescending(x => x.Id)
            .Skip((int)skip)
            .Take(pageSize)
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
                IsClosed = x.IsClosed,
                DateCreated = x.DateCreated
            })
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpGet("by-city/{city}")]
    public async Task<ActionResult> GetByCity(
        string city,
        int page = 1,
        int pageSize = 10)
    {
        var cityName = city.Trim();

        if (page < 1 || pageSize < 1 || pageSize > 50)
        {
            return BadRequest("شماره صفحه باید حداقل ۱ و اندازه صفحه بین ۱ و ۵۰ باشد");
        }

        var skip = ((long)page - 1) * pageSize;

        if (skip > int.MaxValue)
        {
            return BadRequest("شماره صفحه بیش از حد بزرگ است");
        }

        var jobs = await context.Jobs
            .Where(x => x.City == cityName && !x.IsClosed)
            .OrderByDescending(x => x.DateCreated)
            .ThenByDescending(x => x.Id)
            .Skip((int)skip)
            .Take(pageSize)
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
                IsClosed = x.IsClosed,
                DateCreated = x.DateCreated
            })
            .ToListAsync();

        return Ok(jobs);
    }

    [Authorize(Roles = "EMPLOYER")]
    [HttpGet("my-jobs")]
    public async Task<ActionResult> GetMyJobs(
        int page = 1,
        int pageSize = 10)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        if (page < 1 || pageSize < 1 || pageSize > 50)
        {
            return BadRequest("شماره صفحه باید حداقل ۱ و اندازه صفحه بین ۱ و ۵۰ باشد");
        }

        var skip = ((long)page - 1) * pageSize;

        if (skip > int.MaxValue)
        {
            return BadRequest("شماره صفحه بیش از حد بزرگ است");
        }

        var jobs = await context.Jobs
            .Where(x => x.Company.OwnerUserId == userId)
            .OrderByDescending(x => x.DateCreated)
            .ThenByDescending(x => x.Id)
            .Skip((int)skip)
            .Take(pageSize)
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
                IsClosed = x.IsClosed,
                DateCreated = x.DateCreated
            })
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpGet("by-company/{id}")]
    public async Task<ActionResult> GetByCompany(
        int id,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 50)
        {
            return BadRequest("شماره صفحه باید حداقل ۱ و اندازه صفحه بین ۱ و ۵۰ باشد");
        }

        var skip = ((long)page - 1) * pageSize;

        if (skip > int.MaxValue)
        {
            return BadRequest("شماره صفحه بیش از حد بزرگ است");
        }

        var jobs = await context.Jobs
            .Where(x => x.CompanyId == id && !x.IsClosed)
            .OrderByDescending(x => x.DateCreated)
            .ThenByDescending(x => x.Id)
            .Skip((int)skip)
            .Take(pageSize)
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
                IsClosed = x.IsClosed,
                DateCreated = x.DateCreated
            })
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpGet("by-employment-type/{employmentType}")]
    public async Task<ActionResult> GetByEmploymentType(
        EmploymentType employmentType,
        int page = 1,
        int pageSize = 10)
    {
        if (employmentType != EmploymentType.FullTime
            && employmentType != EmploymentType.PartTime
            && employmentType != EmploymentType.Contractual)
        {
            return BadRequest("نوع همکاری معتبر نیست");
        }

        if (page < 1 || pageSize < 1 || pageSize > 50)
        {
            return BadRequest("شماره صفحه باید حداقل ۱ و اندازه صفحه بین ۱ و ۵۰ باشد");
        }

        var skip = ((long)page - 1) * pageSize;

        if (skip > int.MaxValue)
        {
            return BadRequest("شماره صفحه بیش از حد بزرگ است");
        }

        var jobs = await context.Jobs
            .Where(x => x.EmploymentType == employmentType && !x.IsClosed)
            .OrderByDescending(x => x.DateCreated)
            .ThenByDescending(x => x.Id)
            .Skip((int)skip)
            .Take(pageSize)
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
                IsClosed = x.IsClosed,
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
                IsClosed = x.IsClosed,
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
    [HttpPut("{id}/close")]
    public async Task<ActionResult> Close(int id)
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

        job.IsClosed = true;
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
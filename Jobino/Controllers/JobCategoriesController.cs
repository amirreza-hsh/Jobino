using Jobino.Data;
using Jobino.DTOs.JobCategories;
using Jobino.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobino.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobCategoriesController(ApplicationDbContext context)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(
        CreateJobCategoryRequestDto request)
    {
        var normalizedName = request.Name.Trim().ToUpper();

        if (await context.JobCategories
            .AnyAsync(x => x.Name == normalizedName))
        {
            return BadRequest("این دسته‌بندی قبلاً ثبت شده است");
        }

        var category = new JobCategory
        {
            Name = normalizedName
        };

        context.JobCategories.Add(category);

        await context.SaveChangesAsync();

        return Created($"/api/JobCategories/{category.Id}", new
        {
            Id = category.Id
        });
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var categories = await context.JobCategories
            .Select(x => new GetJobCategoryResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                DateCreated = x.DateCreated
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var category = await context.JobCategories
            .Where(x => x.Id == id)
            .Select(x => new GetJobCategoryResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                DateCreated = x.DateCreated
            })
            .FirstOrDefaultAsync();

        if (category is null)
        {
            return NotFound("دسته‌بندی موردنظر یافت نشد");
        }

        return Ok(category);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(
        int id,
        UpdateJobCategoryRequestDto request)
    {
        var category = await context.JobCategories
            .FirstOrDefaultAsync(x => x.Id == id);

        if (category is null)
        {
            return NotFound("دسته‌بندی موردنظر یافت نشد");
        }

        var normalizedName = request.Name.Trim().ToUpper();

        if (normalizedName == "")
        {
            return BadRequest("نام دسته‌بندی نمی‌تواند خالی باشد");
        }

        if (await context.JobCategories
            .AnyAsync(x => x.Name == normalizedName && x.Id != id))
        {
            return BadRequest("این دسته‌بندی قبلاً ثبت شده است");
        }

        category.Name = normalizedName;

        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var category = await context.JobCategories
            .FirstOrDefaultAsync(x => x.Id == id);

        if (category is null)
        {
            return NotFound("دسته‌بندی موردنظر یافت نشد");
        }

        if (await context.Jobs
            .AnyAsync(x => x.JobCategoryId == id))
        {
            return BadRequest(
                "این دسته‌بندی دارای آگهی است و نمی‌توان آن را حذف کرد");
        }

        context.JobCategories.Remove(category);

        await context.SaveChangesAsync();

        return NoContent();
    }


}
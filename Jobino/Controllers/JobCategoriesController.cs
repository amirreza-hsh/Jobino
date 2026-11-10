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
}
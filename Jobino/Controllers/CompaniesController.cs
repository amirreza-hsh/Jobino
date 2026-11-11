
using Jobino.Data;
using Jobino.DTOs.Companies;
using Jobino.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobino.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompaniesController(ApplicationDbContext context)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(
        CreateCompanyRequestDto request)
    {
        var company = new Company
        {
            PersianName = request.PersianName.Trim(),
            EnglishName = request.EnglishName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            LogoUrl = request.LogoUrl?.Trim(),
            Website = request.Website,
            Description = request.Description,
            FoundedYear = request.FoundedYear,
            EmployeeCountRange = request.EmployeeCountRange,
            Province = request.Province,
            City = request.City
        };

        context.Companies.Add(company);

        await context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = company.Id },
            new { Id = company.Id }
        );
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var companies = await context.Companies
            .Select(x => new GetCompanyResponseDto
            {
                Id = x.Id,
                PersianName = x.PersianName,
                EnglishName = x.EnglishName,
                PhoneNumber = x.PhoneNumber,
                LogoUrl = x.LogoUrl,
                Website = x.Website,
                Description = x.Description,
                FoundedYear = x.FoundedYear,
                EmployeeCountRange = x.EmployeeCountRange,
                Province = x.Province,
                City = x.City,
                DateCreated = x.DateCreated
            })
            .ToListAsync();

        return Ok(companies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var company = await context.Companies
            .Where(x => x.Id == id)
            .Select(x => new GetCompanyResponseDto
            {
                Id = x.Id,
                PersianName = x.PersianName,
                EnglishName = x.EnglishName,
                PhoneNumber = x.PhoneNumber,
                LogoUrl = x.LogoUrl,
                Website = x.Website,
                Description = x.Description,
                FoundedYear = x.FoundedYear,
                EmployeeCountRange = x.EmployeeCountRange,
                Province = x.Province,
                City = x.City,
                DateCreated = x.DateCreated
            })
            .FirstOrDefaultAsync();

        if (company is null)
        {
            return NotFound("شرکت موردنظر یافت نشد");
        }

        return Ok(company);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(
        int id,
        UpdateCompanyRequestDto request)
    {
        var company = await context.Companies
            .FirstOrDefaultAsync(x => x.Id == id);

        if (company is null)
        {
            return NotFound("شرکت موردنظر یافت نشد");
        }

        company.PersianName = request.PersianName.Trim();
        company.EnglishName = request.EnglishName.Trim();
        company.PhoneNumber = request.PhoneNumber.Trim();
        company.LogoUrl = request.LogoUrl?.Trim();
        company.Website = request.Website;
        company.Description = request.Description;
        company.FoundedYear = request.FoundedYear;
        company.EmployeeCountRange = request.EmployeeCountRange;
        company.Province = request.Province;
        company.City = request.City;

        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var company = await context.Companies
            .FirstOrDefaultAsync(x => x.Id == id);

        if (company is null)
        {
            return NotFound("شرکت موردنظر یافت نشد");
        }

        if (await context.Jobs
            .AnyAsync(x => x.CompanyId == id))
        {
            return BadRequest(
                "این شرکت دارای آگهی استخدام است و نمی‌توان آن را حذف کرد");
        }

        context.Companies.Remove(company);

        await context.SaveChangesAsync();

        return NoContent();
    }
}

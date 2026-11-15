using Jobino.Data;
using Jobino.DTOs.Companies;
using Jobino.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Jobino.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompaniesController(
    ApplicationDbContext context,
    IWebHostEnvironment environment)
    : ControllerBase
{
    [Authorize(Roles = "EMPLOYER")]
    [HttpPost]
    public async Task<ActionResult> Create(
        CreateCompanyRequestDto request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }
        var company = new Company
        {
            OwnerUserId = userId,
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
    [Authorize(Roles = "EMPLOYER")]
    [HttpPost("{id}/logo")]
    public async Task<ActionResult> UploadLogo(
    int id,
    [FromForm] UploadCompanyLogoRequestDto request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var company = await context.Companies
            .FirstOrDefaultAsync(x => x.Id == id && x.OwnerUserId == userId);

        if (company is null)
        {
            return NotFound("شرکت موردنظر یافت نشد");
        }

        if (request.Logo.Length == 0)
        {
            return BadRequest("فایل لوگو خالی است");
        }

        var extension = Path.GetExtension(request.Logo.FileName).ToLowerInvariant();

        if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
        {
            return BadRequest("پسوند لوگو باید PNG، JPG یا JPEG باشد");
        }

        var uploadFolder = Path.Combine(environment.WebRootPath, "company-logos");
        Directory.CreateDirectory(uploadFolder);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.CreateNew))
        {
            await request.Logo.CopyToAsync(stream);
        }

        company.LogoUrl = $"/company-logos/{fileName}";
        await context.SaveChangesAsync();

        return Ok(new { company.LogoUrl });
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

    [Authorize(Roles = "EMPLOYER")]
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(
        int id,
        UpdateCompanyRequestDto request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }
        var company = await context.Companies
            .FirstOrDefaultAsync(x => x.Id == id && x.OwnerUserId == userId);

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

    [Authorize(Roles = "EMPLOYER")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }
        var company = await context.Companies
            .FirstOrDefaultAsync(x => x.Id == id && x.OwnerUserId == userId);

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

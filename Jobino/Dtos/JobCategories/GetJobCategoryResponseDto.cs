namespace Jobino.DTOs.JobCategories;

public class GetJobCategoryResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime DateCreated { get; set; }
}
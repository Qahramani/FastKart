namespace FastKartProject.Models;

public class UpdateCategoryViewModel
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }
    public IFormFile? ImageFile { get; set; }
}

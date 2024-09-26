using FastKartProject.DataAccessLayer.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastKartProject.DataAccessLayer.Entities;

public class Category : BaseEntity
{
    [MaxLength(10)]
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    public string? Description { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

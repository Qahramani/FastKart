using FastKartProject.DataAccessLayer.Entities.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastKartProject.DataAccessLayer.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public double Price { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public string ImageUrl { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    [NotMapped]
    public List<SelectListItem> Selectitems { get; set; } = new List<SelectListItem>();
}

using FastKartProject.DataAccessLayer.Entities.Common;

namespace FastKartProject.DataAccessLayer.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public ICollection<Product> Products { get; set; }
}

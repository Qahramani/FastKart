using FastKartProject.DataAccessLayer.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastKartProject.DataAccessLayer.Entities;

public class HomeBanner : BaseEntity
{
    [MaxLength(30)]
    public string Title { get; set; }
    [MaxLength(50)]
    public string TopText { get; set; }
    [MaxLength(50)]
    public string BottomText { get; set; }
    public string? ImageUrl { get; set; }
    [NotMapped]
    public IFormFile ImageFile { get; set; }
}

using FastKartProject.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FastKartProject.DataAccessLayer;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }


    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Footer> Footers { get; set; }
    public DbSet<HomeBanner> HomeBanners { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "jeans", ImageUrl = "jeans.svg" },
            new Category { Id = 2, Name = "gown", ImageUrl = "gown.svg" }
        );

        builder.Entity<Product>().HasData(
             new Product { Id = 1, Name = "jeans", Price = 12.11, CategoryId = 1, ImageUrl = "1.jpg" },
             new Product { Id = 2, Name = "tops", Price = 12.90, CategoryId = 2, ImageUrl = "12.jpg" }
        );

        builder.Entity<Footer>().HasData(
            new Footer { Id = 1, FbLink = "fatima", InstLink = "fatimaveliyeva" }
        );

        builder.Entity<HomeBanner>().HasData(
           new HomeBanner { Id = 1, Title = "Main Banner", BottomText = "hello", TopText = "Bye", ImageUrl = "1.jpg" }
        );

        base.OnModelCreating(builder);
    }
}

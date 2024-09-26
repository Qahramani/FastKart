using Microsoft.AspNetCore.Identity;

namespace FastKartProject.DataAccessLayer.Entities;

public class AppUser : IdentityUser
{
    public string Fullname { get; set; }
}

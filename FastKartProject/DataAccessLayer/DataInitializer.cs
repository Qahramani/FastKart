using FastKartProject.DataAccessLayer.Entities;
using FastKartProject.Enums;
using FastKartProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace FastKartProject.DataAccessLayer;

public class DataInitializer
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _dbContext;
    private readonly Admin _admin;

    public DataInitializer(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext dbContext, IOptions<Admin> admin)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _admin = admin.Value;
    }

    public async Task SeedDataAsync()
    {
        await _dbContext.Database.MigrateAsync();

        var roles = new List<string>() { Roles.Admin.ToString(), Roles.User.ToString(), Roles.Moderator.ToString() };

        foreach (var role in roles)
        {
            if (await _roleManager.FindByNameAsync(role) != null) continue;

            await _roleManager.CreateAsync(new IdentityRole { Name = role });
        }

        var user = new AppUser
        {
            Fullname =  _admin.Fullname,
            UserName = _admin.Username,
            Email = _admin.Email,
        };

        var foundUser = await _userManager.FindByNameAsync(user.UserName);

        if (foundUser != null) 
            return;

        var result = await _userManager.CreateAsync(user, _admin.Password);

        if (result.Succeeded)
          await _userManager.AddToRoleAsync(user,Roles.Admin.ToString());

    }
}

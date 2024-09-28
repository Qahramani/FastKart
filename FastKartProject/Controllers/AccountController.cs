using FastKartProject.DataAccessLayer.Entities;
using FastKartProject.Models;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastKartProject.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var user = await _userManager.FindByNameAsync(model.Username);

        if(user != null)
        {
            ModelState.AddModelError("", "User with this name already exist !");
            return View();
        }

        var createdUser = new AppUser()
        {
            Fullname = model.Fullname,
            Email = model.Email,
            UserName = model.Username,
        };

        var result = await _userManager.CreateAsync(createdUser, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }


        return RedirectToAction("login");
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var existingUser = await _userManager.FindByNameAsync(model.Username);

        if(existingUser == null)
        {
            ModelState.AddModelError("", "Username or Password is incorrect");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(existingUser, model.Password, true, true);

        if (result.IsLockedOut)
        {
            ModelState.AddModelError("", "YOUR ACCOUNT WAS BLOCKED DUE TO FALSE PASSWORD ATTEMPTS");
            return View();
        }

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Username or Password is incorrect");
            return View();
        }

        return RedirectToAction("index", "home");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("index", "home");
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
           return View();
        }

        var foundUser = await _userManager.FindByEmailAsync(model.Email);
        
        if (foundUser == null)
        {
            ModelState.AddModelError("", "User with this email was not found");
            return View();
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(foundUser);
        var resetLink = Url.Action(nameof(ResetPassword), "Account", new { model.Email, resetToken }, Request.Scheme, Request.Host.ToString());

        return View(nameof(EmailView), model: resetLink);

    }

    public IActionResult EmailView()
    {
        return View();
    }

    public IActionResult ResetPassword(string email, string resetToken)
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordView model, string email, string resetToken)
    {
        if (!ModelState.IsValid)
        {
           return View();
        }
        var foundUser = await _userManager.FindByEmailAsync(email);

        if (foundUser == null) return BadRequest();

        var result = await _userManager.ResetPasswordAsync(foundUser, resetToken,model.Password);

        if(!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        return RedirectToAction(nameof(Login));

    }
}

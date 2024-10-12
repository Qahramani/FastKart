using FastKartProject.DataAccessLayer.Entities;
using FastKartProject.Models;
using FastKartProject.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastKartProject.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ISenderEmail _emailSender;

    public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, ISenderEmail emailSender)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
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

        if (user != null)
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
            ModelState.AddModelError("", "result is not suceeded");
            return View();
        }

        return RedirectToAction(nameof(Login));
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

        if (existingUser == null)
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

        SendConfirmationEmail(foundUser);

        return RedirectToAction(nameof(Login));
    }


    private async Task SendConfirmationEmail(AppUser user)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        

        var confirmationLink = Url.Action(nameof(ResetPassword), "Account", new { user.Email, token }, Request.Scheme, Request.Host.ToString());


        string body = confirmationLink;
        _emailSender.SendEmailAsync(user.Email, "Salam", body, true);
    }
    public IActionResult ResetPassword(string email, string token)
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordView model, string email, string token)
    {
        if (!ModelState.IsValid)
        {   
            return View();
        }
        var foundUser = await _userManager.FindByEmailAsync(email);

        if (foundUser == null) return BadRequest();

        var result = await _userManager.ResetPasswordAsync(foundUser, token, model.Password);

        if (!result.Succeeded)
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

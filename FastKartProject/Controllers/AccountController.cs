using FastKartProject.DataAccessLayer.Entities;
using FastKartProject.Models;
using FastKartProject.Services.Implementations;
using FastKartProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

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

    private async Task SendConfirmationEmail(string? email, AppUser? user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { user.Id, token }, Request.Scheme);

        await _emailSender.SendEmailAsync(email, "Confirm Your Email", $"Please confirm your account by" +
            $" <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>.", true);
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmEmail(string UserId, string Token)
    {
        if (UserId == null || Token == null)
        {
            ViewBag.Message = "The link is Invalid or Expired";
        }

        var user = await _userManager.FindByIdAsync(UserId);
        if (user == null)
        {
            ViewBag.ErrorMessage = $"The User ID {UserId} is Invalid";
            return View("NotFound");
        }

        var result = await _userManager.ConfirmEmailAsync(user, Token);
        if (result.Succeeded)
        {
            ViewBag.Message = "Thank you for confirming your email";
            return View();
        }

        ViewBag.Message = "Email cannot be confirmed";
        return View();
    }
    [HttpGet]
    public IActionResult ResendConfirmationEmail(bool IsResend = true)
    {
        if (IsResend)
        {
            ViewBag.Message = "Resend Confirmation Email";
        }
        else
        {
            ViewBag.Message = "Send Confirmation Email";
        }
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendConfirmationEmail(string Email)
    {
        var user = await _userManager.FindByEmailAsync(Email);
        if (user == null || await _userManager.IsEmailConfirmedAsync(user))
        {
            return View("ConfirmationEmailSent");
        }

        await SendConfirmationEmail(Email, user);

        return View("ConfirmationEmailSent");
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

        if (result.Succeeded)
        {
            await SendConfirmationEmail(model.Email, createdUser);
            return View("RegistrationSuccessful");

        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();

        }



        //return RedirectToAction(nameof(Login));
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

        var result = await _userManager.ResetPasswordAsync(foundUser, resetToken, model.Password);

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

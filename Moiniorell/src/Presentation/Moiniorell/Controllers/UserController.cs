using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;

namespace Moiniorell.Controllers;

public class UserController : Controller
{
    private readonly IAuthService _service;

    public UserController(IAuthService service)
    {
        _service = service;
    }

    public IActionResult Register()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var res = await _service.ConfirmEmail(token, email);
        if (res.Succeeded)
        {
            TempData["SuccessMessage"] = "Email confirmed successfully.";
        }
        return Login();

    }
    [HttpGet]
    public IActionResult SuccessRegistration()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (ModelState.IsValid)
        {
            var res = await _service.Register(Url, vm);
            if (res.Any())
            {
                foreach (var item in res)
                {

                    ModelState.AddModelError(String.Empty, item);
                }
                return View(vm);
            }

            return RedirectToAction("SuccessRegistration", "User");
        }

        return View(vm);

    }

    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm)
    {
        if (ModelState.IsValid)
        {
            await _service.Logout();
            var res = await _service.Login(Url, vm);
            if (res.Any())
            {
                foreach (var item in res)
                {
                    if (item == "emailconfirm")
                    {

                        return RedirectToAction("SuccessRegistration", "User");
                    }
                    ModelState.AddModelError(String.Empty, item);
                }
                return View(vm);
            }
            return RedirectToAction("Index", "Home");
        }
        return View(vm);
    }
    public async Task<IActionResult> Logout()
    {
        await _service.Logout();
        return RedirectToAction("Login", "User");
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
    {
        if (ModelState.IsValid)
        {
            var result = await _service.ForgotPassword(Url,model);
            if (!result.Any())
            {
                return View("ForgotPasswordConfirmation");
            }
            else
            {
                ModelState.AddModelError("Email", result[0]);
                return View(model);
            }
        }
        return View(model);
    }
    public IActionResult ResetPassword(string token, string email)
    {
        if (token == null || email == null)
        {
            ModelState.AddModelError("", "Invalid password reset token");
        }
        return View();

    }
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
    {
        if (ModelState.IsValid)
        {
            var result = await _service.ResetPasswordAsync(model.Email, model.Token, model.Password);

            if (result.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        return View(model);
    }

}


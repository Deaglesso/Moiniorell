using Microsoft.AspNetCore.Mvc;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;

namespace Moiniorell.Controllers;

public class UserController : Controller
{
    private readonly IAuthService _service;
    private readonly IPostService _postService;

    public UserController(IAuthService service, IPostService postService)
    {
        _service = service;
        _postService = postService;
    }




    //Register
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


    //Login
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
    //Like
    public async Task<IActionResult> LikePost(int postId)
    {

        await _postService.LikePost(postId);

        return RedirectToAction("Index", "Home");
    }
    public async Task<IActionResult> UnlikePost(int postId)
    {

        await _postService.UnlikePost(postId);

        return RedirectToAction("Index", "Home");
    }
    public async Task<IActionResult> DeletePost(int postId)
    {
        await _postService.DeletePost(postId);
        return RedirectToAction("User", "Profile", new { username = User.Identity.Name});
    }

}


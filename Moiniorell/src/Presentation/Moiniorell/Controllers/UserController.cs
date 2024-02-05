using Microsoft.AspNetCore.Mvc;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;

namespace Moiniorell.Controllers
{
    public class UserController : Controller
    {
        private readonly IAuthService _service;

        public UserController(IAuthService service)
        {
            _service = service;
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
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (ModelState.IsValid)
            {
                var res = await _service.Register(vm);
                if (res.Any())
                {
                    foreach (var item in res)
                    {
                        ModelState.AddModelError(String.Empty, item);
                    }
                    return View(vm);
                }
                return RedirectToAction("Index", "Home");
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
                var res =  await _service.Login(vm);
                if (res.Any())
                {
                    foreach (var item in res)
                    {
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


    }
}

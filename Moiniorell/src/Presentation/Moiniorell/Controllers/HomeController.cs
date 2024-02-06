using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Domain.Models;

namespace Moiniorell.Controllers
{
    [Authorize(Roles = "Member")]
    public class HomeController : Controller
    {
        private readonly IAuthService _service;

        public HomeController(IAuthService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 3)
            {
                TempData["ErrorMessage"] = "Search term must be at least 3 characters long.";
                return RedirectToAction(nameof(Index));
            }
            List<AppUser> users = await _service.GetUsers(searchTerm);
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchPost(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 3)
            {
                TempData["ErrorMessage"] = "Search term must be at least 3 characters long.";
                return View("Search");
            }
            List<AppUser> users =  await _service.GetUsers(searchTerm);
            return View("Search", users);
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;
using Moiniorell.Infrastructure.Utilities.Extensions;
using Moiniorell.Persistence.Hubs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Security.Claims;

namespace Moiniorell.Controllers
{
    [Authorize(Roles = "Member")]
    public class HomeController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly IHubContext<OnlineUsersHub> _hubContext;

        public HomeController(IAuthService authService, IPostService postService, IUserService userService, IHubContext<OnlineUsersHub> hubContext)
        {
            _authService = authService;
            _postService = postService;
            _userService = userService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
           
            HomeVM vm = new HomeVM
            {
                Posts = await _postService.GetPosts(),
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Index(CreatePostVM vm)
        {
            if (ModelState.IsValid)
            {

                await _postService.CreatePost(vm);

                return RedirectToAction(nameof(Index));
            }

            HomeVM homeVm = new HomeVM
            {
                Posts = await _postService.GetPosts(),
                CreatePostVM = vm 
            };

            return View("Index", homeVm);
        }
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 3)
            {
                TempData["ErrorMessage"] = "Search term must be at least 3 characters long.";
                return RedirectToAction(nameof(Index));
            }
            List<AppUser> users = await _userService.GetUsers(searchTerm);
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
            List<AppUser> users =  await _userService.GetUsers(searchTerm);
            return View("Search", users);
        }


        


    }
}

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;
using System.Security.Claims;

namespace Moiniorell.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public ProfileController(IAuthService authService, IMapper mapper, IUserService userService)
        {
            _authService = authService;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IActionResult> User(string username)
        {

            AppUser user = await _userService.GetUser(username);
            if (user == null)
            {
                return NotFound();
            }
            
            return View(user);
        }

        public async Task<IActionResult> Edit(string username)
        {
            AppUser user = await _userService.GetUser(username);
            if (user.UserName != HttpContext.User.Identity.Name)
            {
                return BadRequest();
            }
            if (user == null)
            {
                return NotFound();
            }
            EditProfileVM vm = _mapper.Map<EditProfileVM>(user);
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditProfileVM vm)
        {
            AppUser user = await _userService.GetUser(HttpContext.User.Identity.Name);
            if (!ModelState.IsValid)
            {
                vm.ProfilePicture = user.ProfilePicture;
                return View(vm);

            }
            var res = await _userService.UpdateUser(user, vm);
            if (res.Any())
            {
                foreach (var item in res)
                {

                    ModelState.AddModelError(String.Empty, item);
                }
                vm.ProfilePicture = user.ProfilePicture;
                return View(vm);
            }
            await _authService.Logout();

            await _authService.LoginNoPass(user.UserName);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Follow(string followeeId)
        {
            await _userService.Follow(followeeId);
            AppUser user = await _userService.GetUserById(followeeId);
            return RedirectToAction("User","Profile",new { username = user.UserName});
        }
        public async Task<IActionResult> Unfollow(string followeeId)
        {
            await _userService.Unfollow(followeeId);
            AppUser user = await _userService.GetUserById(followeeId);
            return RedirectToAction("User", "Profile", new { username = user.UserName });
        }
        public async Task<IActionResult> AcceptFollow(string followId,string followerId)
        {
            await _userService.AcceptRequest(followId,followerId);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> RejectFollow(string followId, string followerId)
        {
            await _userService.RejectRequest(followId, followerId);
            return RedirectToAction("Index", "Home");
        }
    }
}

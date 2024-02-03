using Microsoft.AspNetCore.Mvc;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Domain.Models;
using Moiniorell.ViewModels;

namespace Moiniorell.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IAuthService _service;

        public ProfileController(IAuthService service)
        {
            _service = service;
        }

        public async Task<IActionResult> User(string username)
        {
            AppUser user = await _service.GetUser(username);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        public async Task<IActionResult> Edit(string username) 
        {
            AppUser user = await _service.GetUser(username);
            if (user == null)
            {
                return NotFound();
            }
            EditProfileVM vm = new EditProfileVM
            {
                Name = user.Name,
                Surname = user.Surname,
                Username = user.UserName,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture,
                Gender = user.Gender,
                BirthDate = user.BirthDate,
                Address = user.Address,
                
            };

            return View(vm);
        }
    }
}

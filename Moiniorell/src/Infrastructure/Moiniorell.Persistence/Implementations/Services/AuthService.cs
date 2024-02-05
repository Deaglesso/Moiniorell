using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Enums;
using Moiniorell.Domain.Models;
using Moiniorell.Infrastructure.Utilities.Extensions;

namespace Moiniorell.Persistence.Implementations.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _http;

        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IWebHostEnvironment env, IHttpContextAccessor http)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _env = env;
            _http = http;
        }

        public async Task<AppUser> GetUser(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<List<AppUser>> GetUsers(string searchTerm)
        {
            return await _userManager.Users.Where(x => x.UserName.ToLower().Contains(searchTerm.ToLower())).ToListAsync();
        }

        public async Task<List<string>> Login(LoginVM vm)
        {
            vm.UsernameOrEmail = vm.UsernameOrEmail.ToLower();

            List<string> str = new List<string>();
            AppUser user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
            if (user is null)
            {
                user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);
                if(user is null)
                {
                    str.Add("Username email or password is wrong!");
                    return str;
                }
                
            }
            var res = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.isRemembered, true);
            if (res.IsLockedOut)
            {
                str.Add("Username email or password is wrong!");
                return str;
            }
            if (!res.Succeeded)
            {
                str.Add("Username email or password is wrong!");
                return str;
            }

            return new List<string>();
        }

        public async Task<List<string>> LoginNoPass(string username)
        {
            

            List<string> str = new List<string>();
            AppUser user = await _userManager.FindByEmailAsync(username);
            if (user is null)
            {
                user = await _userManager.FindByNameAsync(username);
                if (user is null)
                {
                    str.Add("Username email or password is wrong!");
                    return str;
                }

            }
             await _signInManager.SignInAsync(user,true);
            
            return new List<string>();
        }

        public async Task Logout()
        {
            foreach (var cookie in _http.HttpContext.Request.Cookies.Keys)
            {
                _http.HttpContext.Response.Cookies.Delete(cookie);
            }

            await _signInManager.SignOutAsync();
            
        }

        public async Task<List<string>> Register(RegisterVM vm)
        {
            vm.Name = vm.Name.Capitalize();
            vm.Surname = vm.Surname.Capitalize();
            vm.Username = vm.Username.ToLower();

            AppUser user = _mapper.Map<AppUser>(vm);
            List<string> str = new List<string>();
            var res = await _userManager.CreateAsync(user, vm.Password);
            
            if (!res.Succeeded)
            {
                foreach (var item in res.Errors)
                {
                    str.Add(item.Description);
                }
                return str;
            }
            //Create Role
            /*foreach (var item in Enum.GetValues(typeof(Role)))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = item.ToString() });
            }*/
            await _userManager.AddToRoleAsync(user, Role.Member.ToString());
            await _signInManager.SignInAsync(user, isPersistent: false);


            return new List<string>();


        }

        public async Task<List<string>> UpdateUser(AppUser user,EditProfileVM vm)
        {
            List<string> str = new List<string>();

            user.Name = vm.Name.Capitalize();
            user.Surname = vm.Surname.Capitalize();
            user.Address = vm.Address;
            user.PhoneNumber = vm.PhoneNumber;
            user.BirthDate = vm.BirthDate;
            user.Gender = vm.Gender;
            user.UserName = vm.Username;
            if (user.Email != vm.Email)
            {
                var eres = await _userManager.SetEmailAsync(user, vm.Email);
                if (!eres.Succeeded)
                {
                    foreach (var item in eres.Errors)
                    {
                        str.Add(item.Description);
                    }
                    return str;
                }
            }
            if (vm.NewPassword is not null)
            {
                var pres = await _userManager.ChangePasswordAsync(user, vm.CurrentPassword, vm.NewPassword);
                if (!pres.Succeeded)
                {
                    foreach (var item in pres.Errors)
                    {
                        str.Add(item.Description);
                    }
                    return str;
                }
            }


            if (vm.ProfilePictureFile is not null)
            {
                if (vm.ProfilePictureFile.CheckFileType("Image"))
                {
                    str.Add("Only images allowed");
                    return str;
                }
                if (!vm.ProfilePictureFile.CheckFileSize(2))
                {
                    str.Add("Max file size is 2 Mb");
                    return str;
                }
                user.ProfilePicture = await vm.ProfilePictureFile.CreateFileAsync(_env.WebRootPath, "assets", "images");
            }
            

            var res = await _userManager.UpdateAsync(user);
            if (!res.Succeeded)
            {
                foreach (var item in res.Errors)
                {
                    str.Add(item.Description);
                }
                return str;
            }
            return new List<string>();
        }
    }
}

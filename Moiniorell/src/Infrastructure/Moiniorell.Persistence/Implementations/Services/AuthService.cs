using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Enums;
using Moiniorell.Domain.Models;
using Moiniorell.Infrastructure.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Persistence.Implementations.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<AppUser> GetUser(string username)
        {
            return await _userManager.FindByNameAsync(username);
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

        public async Task Logout()
        {
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



        


    }
}

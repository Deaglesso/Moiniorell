using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moiniorell.Application.Abstractions.Repositories;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Enums;
using Moiniorell.Domain.Models;
using Moiniorell.Infrastructure.Utilities.Extensions;
using System;
using System.Linq.Expressions;

namespace Moiniorell.Persistence.Implementations.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IFollowRepository _followRepo;
        private readonly IHttpContextAccessor _http;
        private readonly IEmailService _emailService;

        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IWebHostEnvironment env, IHttpContextAccessor http, IFollowRepository followRepo, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _env = env;
            _http = http;
            _followRepo = followRepo;
            _emailService = emailService;
        }

        
        public async Task<List<string>> Login(IUrlHelper Url, LoginVM vm)
        {
            vm.UsernameOrEmail = vm.UsernameOrEmail.ToLower();

            List<string> str = new List<string>();
            AppUser user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
            if (user is null)
            {
                user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);
                if (user is null)
                {
                    str.Add("Username email or password is wrong!");
                    return str;
                }

            }
            if (!user.EmailConfirmed)
            {
                str.Add("emailconfirm");
                var confres = await SendVerificationMail(Url, vm);

                return str;
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
            await _signInManager.SignInAsync(user, true);

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

        public async Task<List<string>> Register(IUrlHelper Url, RegisterVM vm)
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
            foreach (var item in Enum.GetValues(typeof(Role)))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = item.ToString() });
            }
            //Email Conf
            var confres = await SendVerificationMail(Url, new LoginVM { UsernameOrEmail = vm.Email, Password = vm.Password });

            await _userManager.AddToRoleAsync(user, Role.Member.ToString());


            return new List<string>();


        }


        public async Task<IdentityResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return await _userManager.ConfirmEmailAsync(user, token);

        }

        public async Task<List<string>> SendVerificationMail(IUrlHelper Url, LoginVM vm)
        {
            List<string> str = new List<string>();
            AppUser user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
            if (user is null)
            {
                user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);
                if (user is null)
                {
                    str.Add("Username email or password is wrong!");
                    return str;
                }
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "User", new { email = user.Email, token }, _http.HttpContext.Request.Scheme);
            string confirmationButton = $"<a href=\"{confirmationLink}\" style=\"display:inline-block;padding:10px 20px;margin:10px;color:#fff;background-color:#007bff;text-decoration:none;border-radius:5px;\">Confirm Email</a>";

            string body = $"Dear user,<br><br>Thank you for signing up! Please click the following button to confirm your email:<br>{confirmationButton}";
            await _emailService.SendMailAsync(user.Email, "Confirmation email link", body, true);
            return str;
        }

        public async Task<List<string>> ForgotPassword(IUrlHelper Url, ForgotPasswordVM model)
        {
            List<string> str = new List<string>();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                str.Add("Account not found");
                return str;
            }
            else if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                str.Add("Account email not verified");
                return str;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var passwordResetLink = Url.Action("ResetPassword", "User",
                    new { email = model.Email, token = token }, _http.HttpContext.Request.Scheme);
            string confirmationButton = $"<a href=\"{passwordResetLink}\" style=\"display:inline-block;padding:10px 20px;margin:10px;color:#fff;background-color:#007bff;text-decoration:none;border-radius:5px;\">Reset Password</a>";

            string body = $"Dear user,<br><br>Click the link below to reset your password and regain access to your account.<br>{confirmationButton}";
            await _emailService.SendMailAsync(user.Email, "Reset password", body, true);

            return str;

        }
        
        public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, token, password);
                return result;
            }

            // Handle the case where the user is not found
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }
    }
}

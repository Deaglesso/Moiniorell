using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;

namespace Moiniorell.Application.Abstractions.Services
{
    public interface IAuthService
    {
        Task<List<string>> Register(IUrlHelper url,RegisterVM vm);
        Task<List<string>> SendVerificationMail(IUrlHelper url,LoginVM vm);
        Task<List<string>> ForgotPassword(IUrlHelper Url, ForgotPasswordVM model);
        Task<IdentityResult> ResetPasswordAsync(string email, string token, string password);
        Task<List<string>> Login(IUrlHelper url, LoginVM vm);
        Task<List<string>> LoginNoPass(string username);
        Task<IdentityResult> ConfirmEmail(string email,string token);
        Task Logout();
        Task<AppUser> GetUser(string username);
        Task<AppUser> GetUserById(string userId);

        Task<List<string>> UpdateUser(AppUser user,EditProfileVM vm);
        Task<List<AppUser>> GetUsers(string searchTerm);
        Task Follow(string userId);
        Task Unfollow(string followedId);

    }

}

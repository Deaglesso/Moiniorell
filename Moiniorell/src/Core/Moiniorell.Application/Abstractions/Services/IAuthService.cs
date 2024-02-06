using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;

namespace Moiniorell.Application.Abstractions.Services
{
    public interface IAuthService
    {
        Task<List<string>> Register(RegisterVM vm);
        Task<List<string>> Login(LoginVM vm);
        Task<List<string>> LoginNoPass(string username);

        Task Logout();
        Task<AppUser> GetUser(string username);
        Task<AppUser> GetUserById(string userId);

        Task<List<string>> UpdateUser(AppUser user,EditProfileVM vm);
        Task<List<AppUser>> GetUsers(string searchTerm);
        Task Follow(string userId);
        Task Unfollow(string followedId);

    }

}

using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<AppUser> GetUser(string username);
        Task<AppUser> GetUserForUI(string username);

        Task<AppUser> GetUserById(string userId);

        Task<List<string>> UpdateUser(AppUser user, EditProfileVM vm);
        Task<List<AppUser>> GetUsers(string searchTerm);
        Task Follow(string userId);
        Task AcceptRequest(string followId, string followerId);
        Task RejectRequest(string followId, string followerId);


        Task Unfollow(string followedId);
    }
}

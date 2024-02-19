using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;

namespace Moiniorell.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<AppUser> GetUser(string username);
        Task<AppUser> GetUserForUI(string username);

        Task<AppUser> GetUserById(string userId);

        Task<List<string>> UpdateUser(AppUser user, EditProfileVM vm);
        Task<List<AppUser>> GetUsers(string searchTerm);


        //Following System
        Task Follow(string userId);
        Task AcceptRequest(string followId, string followerId);
        Task RejectRequest(string followId, string followerId);
        Task Unfollow(string followedId);

        //Chat
        List<UserDTO> GetUsersToChat();
        Task<string> RemoveUserConnection(string ConnectionId);
        Task<string> AddUserConnection(string ConnectionId);
        IList<string> GetUserConnections(string userId);
        Task RemoveAllUserConnections(string userId);
    }
}

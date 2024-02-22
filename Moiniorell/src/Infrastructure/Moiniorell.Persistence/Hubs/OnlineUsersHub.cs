using Microsoft.AspNetCore.SignalR;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Domain.Models;
using System.Security.Claims;

namespace Moiniorell.Persistence.Hubs
{
    public class OnlineUsersHub : Hub
    {
        private static readonly List<OnlineUser> OnlineUsers = new List<OnlineUser>();

        private readonly IUserService _userService;

        public OnlineUsersHub(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
            string username = Context.User.Identity.Name;

            if (OnlineUsers.Any(x => x.Username == username))
            {
                OnlineUsers.FirstOrDefault(x => x.Username == username).ConnectionIds.Add(Context.ConnectionId);
            }
            else
            {
                var onlineUser = new OnlineUser
                {
                    ConnectionIds = new List<string>() { Context.ConnectionId },
                    Username = username,
                };
                OnlineUsers.Add(onlineUser);
            }
            await UpdateOnlineUsers();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //var user = OnlineUsers.Find(u => u.ConnectionId == Context.ConnectionId);
            var username = Context.User?.Identity.Name;

            if (username != null)
            {
                OnlineUsers.RemoveAll(x=>x.Username==username);
                await UpdateOnlineUsers();
            }

            await base.OnDisconnectedAsync(exception);
        }

        private List<string> GetOnlineUsernames()
        {
            return OnlineUsers.Select(user => user.Username).ToList();
        }
        public async Task<AppUser> GetUserByUsername(string username)
        {
            return await _userService.GetUser(username);
        }
        private async Task UpdateOnlineUsers()
        {
            await Clients.All.SendAsync("updateOnlineUsers", GetOnlineUsernames());
        }
    }
}

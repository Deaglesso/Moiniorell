using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // Get the username from the context or wherever you store it
            string username = Context.User.Identity.Name;

            var onlineUser = new OnlineUser
            {
                ConnectionId = Context.ConnectionId,
                Username = username,
                // Set other user information as needed
            };

            OnlineUsers.Add(onlineUser);
            await UpdateOnlineUsers();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = OnlineUsers.Find(u => u.ConnectionId == Context.ConnectionId);

            if (user != null)
            {
                OnlineUsers.Remove(user);
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

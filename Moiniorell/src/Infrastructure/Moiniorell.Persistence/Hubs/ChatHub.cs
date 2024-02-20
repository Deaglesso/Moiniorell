using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;
using Moiniorell.Persistence.Implementations.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Moiniorell.Persistence.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _http;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatHub(IUserService userService, IHttpContextAccessor http, IHubContext<ChatHub> hubContext)
        {
            _userService = userService;
            _http = http;
            _hubContext = hubContext;
        }

        public override async Task OnConnectedAsync()
        {
            string userId = await _userService.AddUserConnection(Context.ConnectionId);
            await Clients.All.SendAsync("UpdateOnlineUsers", userId);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            
            string userId = await _userService.RemoveUserConnection(Context.ConnectionId);
            await Clients.All.SendAsync("UpdateOfflineUsers", userId);

            await base.OnDisconnectedAsync(exception);
        }

        public void GetUsersToChat()
        {
            var UserId = _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<UserDTO> users = _userService.GetUsersToChat(); 
            Clients.Clients(_userService.GetUserConnections(UserId)).SendAsync("GetUsers",users);
        }
        public async Task OfflineUser(string userId)
        {
            await Clients.All.SendAsync("BroadcastOfflineUser", userId);
        }
        public async Task ReceiveMessage(string fromUserId, string toUserId, string message)
        {
            
            var connections = _userService.GetUserConnections(toUserId);

            await _hubContext.Clients.Clients(connections).SendAsync("ReceiveMessage", fromUserId, message);
        }
    }
}

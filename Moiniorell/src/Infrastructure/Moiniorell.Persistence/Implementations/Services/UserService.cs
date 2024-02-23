using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moiniorell.Application.Abstractions.Repositories;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;
using Moiniorell.Infrastructure.Utilities.Extensions;
using Moiniorell.Persistence.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Moiniorell.Persistence.Implementations.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IFollowRepository _followRepo;
        private readonly IUserConnectionRepository _userConnectionRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IHttpContextAccessor _http;
        private readonly ICloudService _cloudService;
        private readonly IHubContext<ChatHub> _hubContext;

        public UserService(UserManager<AppUser> userManager, IWebHostEnvironment env, IFollowRepository followRepo, IUserConnectionRepository userConnectionRepository, IMessageRepository messageRepository, IHttpContextAccessor http, ICloudService cloudService, IHubContext<ChatHub> hubContext)
        {
            _userManager = userManager;
            _env = env;
            _followRepo = followRepo;
            _userConnectionRepository = userConnectionRepository;
            _messageRepository = messageRepository;
            _http = http;
            _cloudService = cloudService;
            _hubContext = hubContext;
        }

        public async Task<List<string>> UpdateUser(AppUser user, EditProfileVM vm)
        {
            List<string> str = new List<string>();

            user.Name = vm.Name.Capitalize();
            user.Surname = vm.Surname.Capitalize();
            user.Address = vm.Address;
            user.Biography = vm.Biography;

            user.PhoneNumber = vm.PhoneNumber;
            user.BirthDate = vm.BirthDate;
            user.Gender = vm.Gender;
            user.UserName = vm.Username;
            user.ProfilePicture = vm.ProfilePicture;
            user.IsPrivate = vm.IsPrivate;
            user.Availability = vm.Availability;
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
                if (!vm.ProfilePictureFile.CheckFileType("image"))
                {
                    str.Add("Only images allowed");
                    return str;
                }
                if (vm.ProfilePictureFile.CheckFileSize(2))
                {
                    str.Add("Max file size is 2 Mb");
                    return str;
                }
                await _cloudService.FileDeleteAsync(user.ProfilePicture);
                user.ProfilePicture = await _cloudService.FileCreateAsync(vm.ProfilePictureFile);
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


        public async Task Follow(string followedId)
        {
            string userId = _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                throw new Exception("User not found");
            }
            AppUser user = await _userManager.FindByIdAsync(userId);
            AppUser followed = await _userManager.FindByIdAsync(followedId);


            Follow foll = new Follow
            {

                FolloweeId = followedId,
                FollowerId = userId,
                Status = false

            };
            if (!followed.IsPrivate)
            {
                foll.Status = true;
                followed.FollowerCount++;
                user.FollowingCount++;
            }
            await _followRepo.CreateAsync(foll);
            await _followRepo.SaveChangesAsync();
        }
        public async Task Unfollow(string followedId)
        {
            string userId = _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                throw new Exception("User not found");
            }
            AppUser user = await _userManager.FindByIdAsync(userId);
            AppUser followed = await _userManager.FindByIdAsync(followedId);

            followed.FollowerCount--;
            user.FollowingCount--;

            Follow foll = await _followRepo.GetSingleAsync(f => f.FolloweeId == followedId && f.FollowerId == userId);

            if (foll != null)
            {
                _followRepo.Delete(foll);
                await _followRepo.SaveChangesAsync();
            }
        }

        public async Task<AppUser> GetUser(string username)
        {
           var user = await _userManager.Users
                .Include("Followers.Follower").Include("Followees.Followee").Include("Posts.Comments.Replies").Include("Posts.Likes").Include("Posts.Comments.Author").Include("Posts.Comments.Replies.Author")
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (user is null)
            {
                throw new Exception("User not found");
            }
            return user;

        }
        public async Task<AppUser> GetUserForUI(string username)
        {
            var user = await _userManager.Users
               .FirstOrDefaultAsync(u => u.UserName == username);
            if (user is null)
            {
                throw new Exception("User not found");
            }
            return user;

        }
        public async Task<AppUser> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                throw new Exception("User not found");
            }
            return user;
        }
        public async Task<List<AppUser>> GetUsers(string searchTerm)
        {

            var user = await _userManager.Users.Where(x => x.UserName.ToLower().Contains(searchTerm.ToLower()) || x.Name.ToLower().Contains(searchTerm.ToLower()) || x.Surname.ToLower().Contains(searchTerm.ToLower())).ToListAsync();
            if (user is null)
            {
                throw new Exception("User not found");
            }
            return user;
        }

        public async Task AcceptRequest(string followId, string followerId)
        {
            Follow follow = await _followRepo.GetSingleAsync(x => x.FolloweeId == followId && x.FollowerId == followerId);
            if (follow is null)
            {
                throw new Exception("Follow error");
            }
            follow.Status = true;
            var user = await _userManager.Users
              .FirstOrDefaultAsync(u => u.Id == followId);
            var userEd = await _userManager.Users
              .FirstOrDefaultAsync(u => u.Id == followId);
            if (user is null)
            {
                throw new Exception();
            }
            if (userEd is null)
            {
                throw new Exception();
            }
            user.FollowerCount++;
            userEd.FollowingCount++;
            await _followRepo.SaveChangesAsync();

        }

        public async Task RejectRequest(string followId, string followerId)
        {
            Follow follow = await _followRepo.GetSingleAsync(x => x.FolloweeId == followId && x.FollowerId == followerId);
            if (follow is null)
            {
                throw new Exception("Follow error");
            }
            _followRepo.Delete(follow);
            await _followRepo.SaveChangesAsync();
        }

        public List<UserDTO> GetUsersToChat()
        {
            return _userManager.Users.Include("UserConnections").Where(x => x.Id != _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)).Select(x => new UserDTO
            {
                UserId = x.Id,
                Username = x.UserName,
                Fullname = x.Name + " " + x.Surname,
                ProfilePicture = x.ProfilePicture,  
                IsOnline = x.UserConnections.Count > 0,
            }).ToList();
        }
        public async Task<string> AddUserConnection(string ConnectionId)
        {
            var userId = _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                throw new Exception("User error");
            }
            await _userConnectionRepository.CreateAsync(new UserConnection
            {
                ConnectionId = ConnectionId,
                UserId = userId,
            });
            await _userConnectionRepository.SaveChangesAsync();
            return userId;
        }
        public async Task<string> RemoveUserConnection(string ConnectionId)
        {
            string userId = "0";
            var current = await _userConnectionRepository.GetSingleAsync(x => x.ConnectionId == ConnectionId);
            if (current != null)
            {
                userId = current.UserId ?? "0";

                _userConnectionRepository.Delete(current);
                await _userConnectionRepository.SaveChangesAsync();
            }
            return userId;
        }
        public IList<string> GetUserConnections(string userId)
        {
            return _userConnectionRepository.GetAll(x => x.UserId == userId).Select(x => x.ConnectionId.ToString()).ToList();
        }
        public async Task RemoveAllUserConnections(string userId)
        {
            var current = _userConnectionRepository.GetAll(x => x.UserId == userId);
            foreach (var item in current)
            {

                _userConnectionRepository.Delete(item);
            }
            await _userConnectionRepository.SaveChangesAsync();
        }
        public async Task<ChatBoxModel> GetChatbox(string toUserId)
        {
            var userId = _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                throw new Exception("User error");
            }
            var toUser = _userManager.Users.FirstOrDefault(x => x.Id == toUserId);
            if (toUser is null)
            {
                throw new Exception("User error");
            }
            var messages =  _messageRepository.GetAll(x => (x.FromUserId == userId && x.ToUserId == toUserId) || (x.FromUserId == toUserId && x.ToUserId == userId))
                .OrderByDescending(x => x.CreatedAt)
                .Skip(0)
                .Take(50)
                .Select(x => new MessageDTO
                {
                    Id = x.Id,
                    Message = x.Text,
                    Class = x.FromUserId == toUserId ? "from" : "to",
                    Date = x.CreatedAt
                })
                .OrderBy(x => x.Id)
                .ToList();

            return new ChatBoxModel
            {
                ToUser = ToUserDTO(toUser),
                Messages = messages,
            };
        }
        public UserDTO ToUserDTO(AppUser user)
        {
            return new UserDTO
            {
                Fullname = user.Name + " " + user.Surname,
                UserId = user.Id,
                Username = user.UserName,
            };
        }
        public async Task<bool> SendMessage(string toUserId, string message)
        {
            try
            {
                string USER_ID = _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (USER_ID is null)
                {
                    throw new Exception("Wrong id error");
                }
                await _messageRepository.CreateAsync(new Message
                {
                    FromUserId = USER_ID,
                    ToUserId = toUserId,
                    Text = message,
                    CreatedAt = DateTime.Now
                });
                await _messageRepository.SaveChangesAsync();
                await _hubContext.Clients.User(toUserId).SendAsync("ReceiveMessage", USER_ID, message);
                return true;
            }
            catch { return false; }
        }
    }
}

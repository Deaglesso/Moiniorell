﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moiniorell.Application.Abstractions.Repositories;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;
using Moiniorell.Infrastructure.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IHttpContextAccessor _http;

        public UserService(UserManager<AppUser> userManager, IWebHostEnvironment env, IFollowRepository followRepo, IHttpContextAccessor http)
        {
            _userManager = userManager;
            _env = env;
            _followRepo = followRepo;
            _http = http;
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
                user.ProfilePicture = await vm.ProfilePictureFile.CreateFileAsync(_env.WebRootPath, false, "assets", "images");
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
            return await _userManager.Users
                .Include(u => u.Followers).ThenInclude(x => x.Follower).Include(x => x.Followees).ThenInclude(x => x.Followee).Include(x => x.Posts).ThenInclude(p=>p.Comments).ThenInclude(c=>c.Replies).Include(x => x.Posts).ThenInclude(p => p.Likes)
                .FirstOrDefaultAsync(u => u.UserName == username);
        }
        public async Task<AppUser> GetUserForUI(string username)
        {
            return await _userManager.Users
               .FirstOrDefaultAsync(u => u.UserName == username);
        }
        public async Task<AppUser> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
        public async Task<List<AppUser>> GetUsers(string searchTerm)
        {

            return await _userManager.Users.Where(x => x.UserName.ToLower().Contains(searchTerm.ToLower()) || x.Name.ToLower().Contains(searchTerm.ToLower()) || x.Surname.ToLower().Contains(searchTerm.ToLower())).ToListAsync();
        }

        public async Task AcceptRequest(string followId,string followerId)
        {
            Follow follow = await _followRepo.GetSingleAsync(x=>x.FolloweeId == followId && x.FollowerId == followerId);
            follow.Status = true;
            await _followRepo.SaveChangesAsync();

        }

        public async Task RejectRequest(string followId, string followerId)
        {
            Follow follow = await _followRepo.GetSingleAsync(x => x.FolloweeId == followId && x.FollowerId == followerId);
            _followRepo.Delete(follow);
            await _followRepo.SaveChangesAsync();
        }
    }
}
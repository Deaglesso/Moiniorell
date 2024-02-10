using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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

namespace Moiniorell.Persistence.Implementations.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<AppUser> _userManager;

        public PostService(IPostRepository postRepo, IMapper mapper, IWebHostEnvironment env, IHttpContextAccessor http, UserManager<AppUser> userManager)
        {
            _postRepo = postRepo;
            _mapper = mapper;
            _env = env;
            _http = http;
            _userManager = userManager;
        }

        public async Task<List<string>> CreatePost(CreatePostVM vm)
        {
            List<string> str = new List<string>();
            Post post = _mapper.Map<Post>(vm);
            if (vm.File is not null)
            {
                if (!vm.File.CheckFileType("image"))
                {
                    str.Add("Only images allowed");
                    return str;
                }
                if (vm.File.CheckFileSize(2))
                {
                    str.Add("Max file size is 2 Mb");
                    return str;
                }
                post.Image = await vm.File.CreateFileAsync(_env.WebRootPath, true, "assets", "images");
                
            }
            post.AuthorId = _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            await _postRepo.CreateAsync(post);
            await _postRepo.SaveChangesAsync();
            return str;
        }

        public async Task<List<Post>> GetPosts()
        {
            var currentUserId = _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.Users
                .Include(u => u.Followers)
                .Include(u => u.Followees)
                .SingleOrDefaultAsync(u => u.Id == currentUserId);



            var followedUserIds = currentUser.Followees.Select(f => f.FolloweeId).ToList();

            var posts = _postRepo.GetAll(p => followedUserIds.Contains(p.AuthorId) || p.AuthorId == currentUserId,nameof(Post.Author),nameof(Post.Comments)).OrderByDescending(p => p.CreatedAt).ToList();
            
            return posts;

        }
    }
}

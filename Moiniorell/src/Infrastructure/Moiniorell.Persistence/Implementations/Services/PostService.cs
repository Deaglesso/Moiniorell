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
using Moiniorell.Persistence.Implementations.Repositories;
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
        private readonly ILikeRepository _likeRepo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<AppUser> _userManager;

        public PostService(IPostRepository postRepo, ILikeRepository likeRepo, IMapper mapper, IWebHostEnvironment env, IHttpContextAccessor http, UserManager<AppUser> userManager)
        {
            _postRepo = postRepo;
            _likeRepo = likeRepo;
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
            AppUser user =await _userManager.FindByIdAsync(post.AuthorId);
            user.PostCount++;
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



            var followedUserIds = currentUser.Followers.Select(f => f.FolloweeId).ToList();

            var posts = _postRepo.GetAll(p => followedUserIds.Contains(p.AuthorId) || p.AuthorId == currentUserId,nameof(Post.Author),nameof(Post.Comments), nameof(Post.Likes), "Comments.Replies").OrderByDescending(p => p.CreatedAt).ToList();
            
            return posts;

        }
        public async Task<Post> GetPost(int postId)
        {


            return await _postRepo.GetSingleAsync(x=>x.Id == postId);

        }
        public async Task<List<Post>> GetMyPosts()
        {
            var currentUserId = _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.Users
                .SingleOrDefaultAsync(u => u.Id == currentUserId);

            var posts = _postRepo.GetAll(p => p.AuthorId == currentUserId, nameof(Post.Author), nameof(Post.Comments), nameof(Post.Likes), "Comments.Replies").OrderByDescending(p => p.CreatedAt).ToList();

            return posts;
        }

        public async Task LikePost(int postId)
        {
            var currentUserId = _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.Users.Include(x => x.Likes)
                .SingleOrDefaultAsync(u => u.Id == currentUserId);

            var post = await _postRepo.GetSingleAsync(p => p.Id == postId);

            var existingLike = await _likeRepo.GetSingleAsync(l => l.PostId == postId && l.LikerId == currentUser.Id);
            if(currentUser.Likes.Any(x => x.PostId == postId))
            {
                return;
            }
            var like = new Like
            {
                LikerId = currentUser.Id,
                PostId = postId
            };

            await _likeRepo.CreateAsync(like);

            post.LikeCount++;

            _postRepo.Update(post);

            await _postRepo.SaveChangesAsync();

        }
        public async Task UnlikePost(int postId)
        {
            var currentUserId = _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.Users
                .SingleOrDefaultAsync(u => u.Id == currentUserId);

            var existingLike = await _likeRepo.GetSingleAsync(l => l.PostId == postId && l.LikerId == currentUser.Id);

            if (existingLike != null)
            {
                _likeRepo.Delete(existingLike);

                var post = await _postRepo.GetSingleAsync(p => p.Id == postId);
                if (post != null && post.LikeCount > 0)
                {
                    post.LikeCount--;
                    _postRepo.Update(post);
                }

                await _likeRepo.SaveChangesAsync();
            }
        }

        public async Task DeletePost(int postId)
        {
            var post = await _postRepo.GetSingleAsync(p => p.Id == postId);
            _postRepo.Delete(post);
            await _postRepo.SaveChangesAsync();
        }
    }
}

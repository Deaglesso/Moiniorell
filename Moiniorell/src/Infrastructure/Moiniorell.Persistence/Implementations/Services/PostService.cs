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
using Moiniorell.Persistence.Implementations.Services;

namespace Moiniorell.Persistence.Implementations.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepo;
        private readonly ILikeRepository _likeRepo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ICloudService _cloudService;
        private readonly IHttpContextAccessor _http;
        private readonly ICommentService _commentService;
        private readonly UserManager<AppUser> _userManager;

        public PostService(IPostRepository postRepo, ILikeRepository likeRepo, IMapper mapper, IWebHostEnvironment env, ICloudService cloudService, IHttpContextAccessor http, ICommentService commentService, UserManager<AppUser> userManager)
        {
            _postRepo = postRepo;
            _likeRepo = likeRepo;
            _mapper = mapper;
            _env = env;
            _cloudService = cloudService;
            _http = http;
            _commentService = commentService;
            _userManager = userManager;
        }

        public async Task<List<string>> CreatePost(CreatePostVM vm)
        {
            List<string> str = new List<string>();
            Post post = _mapper.Map<Post>(vm);
            if (vm.Base64 is not null)
            {

                int commaIndex = vm.Base64.IndexOf(',');
                int colonIndex = vm.Base64.IndexOf(':');
                int slashIndex = vm.Base64.IndexOf('/', colonIndex);

                string type = vm.Base64.Substring(colonIndex + 1, slashIndex - colonIndex - 1);

                int semicolonIndex = vm.Base64.IndexOf(';', slashIndex);

                string extension = vm.Base64.Substring(slashIndex + 1, semicolonIndex - slashIndex - 1);

                string base64Data = vm.Base64.Substring(commaIndex + 1);
                byte[] bytes = Convert.FromBase64String(base64Data);

                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    IFormFile formFile = new FormFile(memoryStream, 0, bytes.Length, type + "/" + extension, "cropped." + extension);

                    post.Image = await _cloudService.FileCreateAsync(formFile);
                }

            }
            else if (vm.File is not null)
            {
                if (vm.File.CheckFileType("image"))
                {
                    if (vm.File.CheckFileSize(2))
                    {
                        str.Add("Max file size is 2 Mb");
                        return str;
                    }
                    post.Image = await _cloudService.FileCreateAsync(vm.File);
                }
                else if (vm.File.CheckFileType("video"))
                {
                    post.Image = await vm.File.CreateFileAsync(_env.WebRootPath, false, "assets", "images");
                }
                else
                {
                    str.Add("Only images and videos allowed");
                    return str;
                }

            }
            post.AuthorId = _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (post.AuthorId is null)
            {
                throw new Exception("User error");
            }
            AppUser user = await _userManager.FindByIdAsync(post.AuthorId);
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

            if (currentUser is null)
            {
                throw new Exception("user error");
            }


            var followedUserIds = currentUser.Followers.Select(f => f.FolloweeId).ToList();

            var posts = _postRepo.GetAll(p => followedUserIds.Contains(p.AuthorId) || p.AuthorId == currentUserId, nameof(Post.Author), "Comments.Author", nameof(Post.Likes), "Comments.Replies").OrderByDescending(p => p.CreatedAt).ToList();

            return posts;

        }
        public async Task<Post> GetPost(int postId)
        {


            return await _postRepo.GetSingleAsync(x => x.Id == postId);

        }
        public async Task<List<Post>> GetMyPosts()
        {
            var currentUserId = _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.Users
                .SingleOrDefaultAsync(u => u.Id == currentUserId);
            if (currentUser is null)
            {
                throw new Exception("user error");
            }
            var posts = _postRepo.GetAll(p => p.AuthorId == currentUserId, nameof(Post.Author), nameof(Post.Comments), nameof(Post.Likes), "Comments.Replies").OrderByDescending(p => p.CreatedAt).ToList();

            return posts;
        }

        public async Task LikePost(int postId)
        {
            var currentUserId = _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.Users.Include(x => x.Likes)
                .SingleOrDefaultAsync(u => u.Id == currentUserId);
            if (currentUser is null)
            {
                throw new Exception("user error");
            }
            var post = await _postRepo.GetSingleAsync(p => p.Id == postId);

            var existingLike = await _likeRepo.GetSingleAsync(l => l.PostId == postId && l.LikerId == currentUser.Id);
            if (currentUser.Likes.Any(x => x.PostId == postId))
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
            if (currentUser is null)
            {
                throw new Exception("user error");
            }
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
            var post = await _postRepo.GetSingleAsync(p => p.Id == postId,"Comments");
            var currentUserId = _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (post.AuthorId != currentUserId)
            {
                throw new Exception("access denied");
            }
            var currentUser = await _userManager.Users
                .SingleOrDefaultAsync(u => u.Id == currentUserId);
            if (currentUser is null)
            {
                throw new Exception("user error");
            }
            currentUser.PostCount--;

            if (post.Image is not null)
            {
                if (post.Image.Contains(".jpeg") || post.Image.Contains(".jpg") || post.Image.Contains(".png"))
                {

                await _cloudService.FileDeleteAsync(post.Image);
                }
                else
                {
                    post.Image.DeleteFile(_env.WebRootPath, "assets", "images");
                }
            }
            foreach (var comment in post.Comments)
            {
                await _commentService.DeleteComment(comment.Id,true);
            }
            _postRepo.Delete(post);
            await _postRepo.SaveChangesAsync();
        }
    }
}

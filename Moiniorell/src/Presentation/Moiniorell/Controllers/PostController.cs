using Microsoft.AspNetCore.Mvc;
using Moiniorell.Application.Abstractions.Services;

namespace Moiniorell.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<IActionResult> LikePost(int postId)
        {

            await _postService.LikePost(postId);

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> UnlikePost(int postId)
        {

            await _postService.UnlikePost(postId);

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> DeletePost(int postId)
        {
            await _postService.DeletePost(postId);
            return RedirectToAction("User", "Profile", new { username = User.Identity.Name });
        }
        
    }
}

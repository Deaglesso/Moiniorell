using Microsoft.AspNetCore.Mvc;
using Moiniorell.Application.Abstractions.Services;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Moiniorell.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        [HttpPost]
        public async Task<IActionResult> LikePost(int postId)
        {

            await _postService.LikePost(postId);
            var updatedPost = await _postService.GetPost(postId);
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
            };

            return Json(updatedPost, jsonOptions);

        }
        [HttpPost]
        public async Task<IActionResult> UnlikePost(int postId)
        {

            await _postService.UnlikePost(postId);
            var updatedPost = await _postService.GetPost(postId);
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
            };

            return Json(updatedPost, jsonOptions);
        }
        public async Task<IActionResult> DeletePost(int postId)
        {
            await _postService.DeletePost(postId);
            return RedirectToAction("User", "Profile", new { username = User.Identity.Name });
        }
        
    }
}

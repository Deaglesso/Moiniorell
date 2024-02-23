using Microsoft.AspNetCore.Mvc;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;

namespace Moiniorell.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IPostService _postService;

        public CommentController(ICommentService commentService, IPostService postService)
        {
            _commentService = commentService;
            _postService = postService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(CreateCommentVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml", new HomeVM { CreateCommentVM = vm,Posts = await _postService.GetPosts()});
            }

            var res = await _commentService.CreateComment(vm);
            if (res.Any())
            {
                throw new Exception();
            }
            return RedirectToAction("Index","Home");
        }
        [HttpPost]
        public async Task<IActionResult> CreateReply(CreateReplyVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml", new HomeVM { CreateReplyVM = vm, Posts = await _postService.GetPosts() });
            }

            var res = await _commentService.CreateReply(vm);
            if (res.Any())
            {
                throw new Exception();
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var res = await _commentService.DeleteComment(commentId);
            if (res.Any())
            {
                throw new Exception();
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> DeleteReply(int replyId)
        {
            var res = await _commentService.DeleteReply(replyId);
            if (res.Any())
            {
                throw new Exception();
            }
            return RedirectToAction("Index", "Home");
        }



    }
}

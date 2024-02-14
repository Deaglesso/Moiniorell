using Microsoft.AspNetCore.Mvc;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;

namespace Moiniorell.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateComment(CreateCommentVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var res = await _commentService.CreateComment(vm);
            if (res.Any())
            {
                throw new Exception();
            }
            return RedirectToAction("Index","Home");
        }


    }
}

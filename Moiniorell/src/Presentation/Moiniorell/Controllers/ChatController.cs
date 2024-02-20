using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;
using Moiniorell.Persistence.DAL;
using Moiniorell.Persistence.Hubs;
using System.Web;

namespace Moiniorell.Controllers
{
    public class ChatController : Controller
    {
        private readonly IUserService _userService;

        public ChatController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetChatbox(string toUserId)
        {
            ChatBoxModel chatBoxModel = await _userService.GetChatbox(toUserId);
            return PartialView("~/Views/Shared/_ChatBox.cshtml", chatBoxModel);
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(string toUserId, string message)
        {
            string decodedMessage = HttpUtility.UrlDecode(message);
            return Json(await _userService.SendMessage(toUserId, decodedMessage));
        }
    }
}

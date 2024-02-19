using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moiniorell.Domain.Models;
using Moiniorell.Persistence.DAL;
using Moiniorell.Persistence.Hubs;

namespace Moiniorell.Controllers
{
    public class ChatController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using MyChat.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyChat.ViewComponents
{
    public class RoomViewComponent : ViewComponent
    {
        private AppDbContext _ctx;

        public RoomViewComponent(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public IViewComponentResult Invoke()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var chats = _ctx.ChatUsers
                .Include(x => x.Chat)
                .Where(x => x.UserId == userId)
                .Select(x=> x.Chat)
                .ToList();

            return View(chats);
        }
    }
}

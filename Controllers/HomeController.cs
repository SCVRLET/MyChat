using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyChat.Database;
using MyChat.Models;
using MyChat.ViewModels;

namespace MyChat.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private AppDbContext _ctx;

        public HomeController(AppDbContext ctx, ILogger<HomeController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("{id}")]
        public IActionResult Chat(int id)
        {
            var chat = _ctx.Chats/*.Include(x=> x.Messages)*/
                .FirstOrDefault(x => x.Id == id);

            chat.Messages = _ctx.Messages.Where(x => x.ChatId == id).ToList();
            chat.Users = _ctx.ChatUsers.Where(x => x.ChatId == id).ToList();

            foreach(var user in chat.Users)
            {
                user.User = _ctx.ChatUsers.Select(x => x.User).Where(x => x.Id == user.UserId).FirstOrDefault();
            }

            var ff = _ctx.ChatUsers.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).FirstOrDefault();

            return View(new ChatViewModel { chatUserView = ff, chatView = chat });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            int joinId;

            while (true)
            {
                joinId = new Random().Next(100000000, 1000000000);

                if (_ctx.Chats.Where(x => x.JoinId == joinId).Count() == 0)
                    break;
            }

            var chat = new Chat
            {
                Name = name,
                Type = ChatType.Private,
                JoinId = joinId
            };

            chat.Users.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Admin
            });

            _ctx.Chats.Add(chat);

            await _ctx.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> JoinRoom(int joinId)
        {
            var chat = _ctx.Chats.Where(x => x.JoinId == joinId);
            if (chat.Count() > 0)
            {
                var chatUser = new ChatUser
                {
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    Role = UserRole.Member,
                    ChatId = chat.FirstOrDefault().Id
                };

                _ctx.ChatUsers.Add(chatUser);

                await _ctx.SaveChangesAsync();
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> KickUser(string userId, int chatId)
        {
            var kicked_user = _ctx.ChatUsers.Where(x => x.UserId == userId & x.ChatId == chatId).FirstOrDefault();

            _ctx.ChatUsers.Remove(kicked_user);

            await _ctx.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int chatId, string message)
        {
            var Message = new Message
            {
                ChatId = chatId,
                Text = message,
                Name = User.Identity.Name,
                Timestamp = DateTime.Now
            };

            _ctx.Messages.Add(Message);
            await _ctx.SaveChangesAsync();

            return RedirectToAction("Chat", new { id = chatId});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

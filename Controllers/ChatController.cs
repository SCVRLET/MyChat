using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyChat.Hubs;
using MyChat.Database;

namespace MyChat.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ChatController : Controller
    {
        private IHubContext<ChatHub> _chat;

        public ChatController(IHubContext<ChatHub> chat)
        {
            _chat = chat;
        }

        [HttpPost("[action]/{connectionId}/{chatId}")]
        public async Task<IActionResult> JoinRoom(string chatId, string connectionId)
        {
            await _chat.Groups.AddToGroupAsync(connectionId, chatId);

            return Ok();
        }

        [HttpPost("[action]/{connectionId}/{chatId}")]
        public async Task<IActionResult> LeaveRoom(string chatId, string connectionId)
        {
            await _chat.Groups.RemoveFromGroupAsync(connectionId, chatId);

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(string message, int chatId, string roomName, [FromServices] AppDbContext ctx)
        {

            var Message = new Message
            {
                ChatId = chatId,
                Text = message,
                Name = User.Identity.Name,
                Timestamp = DateTime.Now
            };

            ctx.Messages.Add(Message);
            await ctx.SaveChangesAsync();
             
            await _chat.Clients.Group(roomName).SendAsync("RecieveMessage", new 
            { 
                Text = Message.Text,
                Name = Message.Name,
                Timestamp = Message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss")
            });

            return Ok();
        }

/*        [HttpPost("[action]")]
        public async Task<IActionResult> KickUser(string userId, int chatId,[FromServices] AppDbContext ctx)
        {
            ctx.ChatUsers = (Microsoft.EntityFrameworkCore.DbSet<ChatUser>)ctx.ChatUsers.Where(x => x.UserId != userId & x.ChatId != chatId);

            await ctx.SaveChangesAsync();

            return Ok();
        }*/
    }
}

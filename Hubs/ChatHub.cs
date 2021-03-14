using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.Hubs
{
    public class ChatHub : Hub
    {
        public string ConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}

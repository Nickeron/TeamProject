using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamameKaiDernoume.Controllers
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task SendMessage(string Sender, string ReceiverId, string message)
        {
            await Clients.Client(ReceiverId).SendAsync("ReceiveMessage", Sender, message);
            await SendMessageToCaller(Sender, message);
        }

        public async Task SendMessageToCaller(string Sender, string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", Sender, message);
        }

        public Task SendMessageToGroups(string message)
        {
            List<string> groups = new List<string>() { "SignalR Users" };
            return Clients.Groups(groups).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TeamProject.Controllers
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            this.logger = logger;
        }
        public async Task SendMessage(string Sender, string ReceiverId, string message)
        {
            await Clients.Caller.SendAsync("ShowSentMessage", Sender, message);
            await Clients.User(ReceiverId).SendAsync("ReceiveMessage", Sender, message);
        }

        public async Task DistributeComment(string PostID, string commentsText)
        {
            await Clients.All.SendAsync("AddTheNewComment", Context.User.Identity.Name, PostID, commentsText);
        }

        public async Task DistributeReaction(string PostID, int likes, int dislikes)
        {
                await Clients.All.SendAsync("AddTheReaction", PostID, likes, dislikes); 
        }

        public Task SendMessageToGroups(string message)
        {
            List<string> groups = new List<string>() { "SignalR Users" };
            return Clients.Groups(groups).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            logger.LogInformation(Context.User.Identity.Name + " connected with CID: " + Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
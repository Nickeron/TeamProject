using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamProject.Data.Entities;

namespace TeamProject.Controllers
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> logger;
        private readonly UserManager<User> userManager;
        private readonly List<string> allConnected;

        public ChatHub(ILogger<ChatHub> logger,
            UserManager<User> userManager,
            List<string> allConnected)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.allConnected = allConnected;
        }

        public async Task SendMessage(string SenderAvatar, string SenderId, string ReceiverId, string message)
        {
            await Clients.Caller.SendAsync("ShowSentMessage", ReceiverId, SenderAvatar, message);
            await Clients.User(ReceiverId).SendAsync("ReceiveMessage", SenderId, SenderAvatar, message);
        }

        public async Task DistributeComment(string usersAvatar, string Name,string AuthorID, bool isOP, string PostID, string CommentID, string commentsText, string url)
        {
            await Clients.All.SendAsync("AddTheNewComment", usersAvatar, Name, AuthorID, isOP, PostID,CommentID, commentsText, url);
        }

        public async Task DistributeReaction(string PostID, int likes, int dislikes)
        {
            await Clients.All.SendAsync("UpdateReactionsOnPost", PostID, likes, dislikes);
        }

        public Task SendMessageToGroups(string message)
        {
            List<string> groups = new List<string>() { "SignalR Users" };
            return Clients.Groups(groups).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            logger.LogInformation(Context.User.Identity.Name + " connected with CID: " + Context.ConnectionId);
            string thisUserID = userManager.GetUserId(Context.User);
            allConnected.Add(thisUserID);
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            await Clients.All.SendAsync("UserConnected", thisUserID);
            await Clients.User(thisUserID).SendAsync("UpdateConnections", allConnected);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string thisUserID = userManager.GetUserId(Context.User);
            allConnected.Remove(thisUserID);
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            await Clients.All.SendAsync("UserDisConnected", userManager.GetUserId(Context.User));
            await base.OnDisconnectedAsync(exception);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TeamProject.Data;
using TeamProject.Data.Entities;
using TeamProject.Models;

namespace TeamProject.Controllers
{
    [Authorize]
    public class MessengerController : Controller
    {
        private readonly IDataRepository dataRepository;
        private readonly UserManager<User> userManager;
        private readonly IHostingEnvironment env;
        private readonly ILogger<MessengerController> logger;

        public MessengerController(IDataRepository dataRepository,
            UserManager<User> userManager,
            IHostingEnvironment env,
            ILogger<MessengerController> logger)
        {
            this.dataRepository = dataRepository;
            this.userManager = userManager;
            this.env = env;
            this.logger = logger;
        }

        public async Task<IActionResult> Chat()
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            List<Message> allUsersMessages = (List<Message>)dataRepository.GetAllMessagesOfUser(thisUser);

            User lastCommUser = null;
            int unreadLatest = 0;
            if (!(allUsersMessages is null) && allUsersMessages.Count > 0)
            {
                if (allUsersMessages.LastOrDefault().Receiver.Id == thisUser.Id)
                {
                    lastCommUser = allUsersMessages.LastOrDefault().Sender;
                }
                else
                {
                    lastCommUser = allUsersMessages.LastOrDefault().Receiver;
                }
                unreadLatest = allUsersMessages.Where(m => m.Sender.Id == lastCommUser.Id && m.isUnread).ToList().Count;
            }
            List<UserChatModel> Correspondance = new List<UserChatModel>();

            List<User> ThisUsersFriends = dataRepository.GetUsersFriends(thisUser).ToList();

            foreach (User friend in ThisUsersFriends)
            {
                List<Message> friendsExchanged = allUsersMessages.Where(m => m.Receiver.Id == friend.Id || m.Sender.Id == friend.Id).ToList();

                UserChatModel userChat = new UserChatModel
                {
                    Correspondant = friend,
                    CorrespondantsMessages = friendsExchanged,
                    UnreadReceived = friendsExchanged.Where(m => m.Sender.Id == friend.Id && m.isUnread).ToList().Count
                };
                Correspondance.Add(userChat);
            }

            MessengerViewModel messengerView = new MessengerViewModel
            {
                ThisUser = thisUser,
                LatestCommunicator = lastCommUser,
                UnreadLatest = unreadLatest,
                FriendsAndMessages = Correspondance
            };
            return View(messengerView);
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody]string UserID)
        {
            User correspondant = await userManager.FindByIdAsync(UserID);
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            List<Message> allUsersMessages = dataRepository.GetAllMessagesOfUsers(thisUser, correspondant).ToList();

            UserChatModel messengerView = new UserChatModel
            {
                Correspondant = correspondant,
                CorrespondantsMessages = allUsersMessages,
                UnreadReceived = allUsersMessages.Where(m => m.Sender.Id == UserID && m.isUnread).ToList().Count,
            };
            return Json(messengerView);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody]SendMessageModel messageModel)
        {
            User receiver = await userManager.FindByIdAsync(messageModel.ReceiverID);
            User thisUser = await userManager.GetUserAsync(HttpContext.User);

            if (receiver is null || thisUser is null) throw new Exception("Cannot have null receiver or sender");

            Message newMessage = new Message
            {
                isUnread = true,
                MessageDate = DateTime.Now,
                MessageText = messageModel.MessageText,
                Receiver = receiver,
                Sender = thisUser
            };

            dataRepository.AddEntity(newMessage);

            if (dataRepository.SaveAll())
            {
                logger.LogError("Ok new message was saved");
                return Ok("New message saved!");
            };
            return BadRequest("Something bad happened");
        }

        [HttpPost]
        public async Task<IActionResult> ReadMessages([FromBody]string SenderId)
        {
            User toThisUser = await userManager.GetUserAsync(HttpContext.User);
            if (dataRepository.ReadAllMessagesFrom(SenderId, toThisUser))
            {
                return Ok("OK");
            };

            return BadRequest("Could not read messages");
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody]CommentModel CommentData)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);
            Comment toEditComment = await dataRepository.GetCommentById(CommentData.CommentID);
            toEditComment.CommentText = CommentData.CommentText;
            toEditComment.CommentDate = DateTime.UtcNow;

            if (dataRepository.SaveAll())
            {
                logger.LogError("saved");
            };
            return Ok(" Comment Eddited");

        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]int id)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);

            Comment toDelete = await dataRepository.GetCommentById(id);

            dataRepository.DeleteEntity(toDelete);

            if (dataRepository.SaveAll())
            {
                logger.LogInformation("saved");
            };
            return Ok("Comment Deleted");
        }
    }
}

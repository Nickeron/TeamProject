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
        private readonly ILogger<MessengerController> logger;

        public MessengerController(IDataRepository dataRepository,
            UserManager<User> userManager,
            ILogger<MessengerController> logger)
        {
            this.dataRepository = dataRepository;
            this.userManager = userManager;
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

            logger.LogInformation("User " + thisUser.UserName + " navigated to Chat Page");
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
                CorrespondantsMessages = allUsersMessages
            };
            logger.LogInformation("User " + thisUser.UserName
                + " navigated to Chat Page with default correspondant "
                + correspondant.UserName);
            return Json(messengerView);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody]SendMessageModel messageModel)
        {
            User receiver = await userManager.FindByIdAsync(messageModel.ReceiverID);
            User thisUser = await userManager.GetUserAsync(HttpContext.User);

            if (receiver is null || thisUser is null) throw new Exception("Cannot have null receiver or sender");
            DateTime timestamp = DateTime.Now;
            Message newMessage = new Message
            {
                isUnread = true,
                MessageDate = timestamp,
                MessageText = messageModel.MessageText,
                Receiver = receiver,
                Sender = thisUser
            };

            dataRepository.AddEntity(newMessage);

            if (dataRepository.SaveAll())
            {
                logger.LogError("Ok new message was saved");
            };

            NewMessageModel returnData = new NewMessageModel
            {
                MessageID = dataRepository.GetMessageIDByTimestampAndUser(timestamp, thisUser),
                MessageDate = timestamp.ToString()
            };

            return Json(returnData);
        }

        [HttpPost]
        public async Task<IActionResult> ReadMessages([FromBody]string SenderId)
        {
            User toThisUser = await userManager.GetUserAsync(HttpContext.User);
            if (dataRepository.ReadAllMessagesFrom(SenderId, toThisUser))
            {
                return Ok("OK messages were read!");
            };

            return BadRequest("Could not read messages");
        }
    }
}

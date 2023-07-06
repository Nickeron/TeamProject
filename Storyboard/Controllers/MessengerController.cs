using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly IDataRepository _dataRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<MessengerController> _logger;

        public MessengerController(IDataRepository dataRepository,
            UserManager<User> userManager,
            ILogger<MessengerController> logger)
        {
            _dataRepository = dataRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Chat()
        {
            User thisUser = await _userManager.GetUserAsync(HttpContext.User);
            List<Message> allUsersMessages = (List<Message>)_dataRepository.GetAllMessagesOfUser(thisUser);

            User lastCommUser = null;
            int unreadLatest = 0;
            if (!(allUsersMessages is null) && allUsersMessages.Count > 0)
            {
                lastCommUser = allUsersMessages.LastOrDefault()?.Receiver.Id == thisUser.Id
                    ? allUsersMessages.LastOrDefault()?.Sender
                    : allUsersMessages.LastOrDefault().Receiver;
                unreadLatest = allUsersMessages.Where(m => m.Sender.Id == lastCommUser.Id && m.isUnread).ToList().Count;
            }

            List<UserChatModel> correspondence = new List<UserChatModel>();

            List<User> thisUsersFriends = _dataRepository.GetUsersFriends(thisUser).ToList();

            foreach (User friend in thisUsersFriends)
            {
                List<Message> friendsExchanged = allUsersMessages
                    .Where(m => m.Receiver.Id == friend.Id || m.Sender.Id == friend.Id).ToList();

                UserChatModel userChat = new UserChatModel
                {
                    Correspondent = friend,
                    CorrespondentsMessages = friendsExchanged,
                    UnreadReceived = friendsExchanged.Where(m => m.Sender.Id == friend.Id && m.isUnread).ToList().Count
                };
                correspondence.Add(userChat);
            }

            MessengerViewModel messengerView = new MessengerViewModel
            {
                ThisUser = thisUser,
                LatestCommunicator = lastCommUser,
                UnreadLatest = unreadLatest,
                FriendsAndMessages = correspondence
            };

            _logger.LogInformation("User " + thisUser.UserName + " navigated to Chat Page");
            return View(messengerView);
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] string userId)
        {
            User correspondent = await _userManager.FindByIdAsync(userId);
            User thisUser = await _userManager.GetUserAsync(HttpContext.User);
            List<Message> allUsersMessages = _dataRepository.GetAllMessagesOfUsers(thisUser, correspondent).ToList();

            UserChatModel messengerView = new UserChatModel
            {
                CorrespondentsMessages = allUsersMessages
            };
            _logger.LogInformation("User " + thisUser.UserName
                                           + " navigated to Chat Page with default correspondant "
                                           + correspondent.UserName);
            return Json(messengerView);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageModel messageModel)
        {
            User receiver = await _userManager.FindByIdAsync(messageModel.ReceiverId);
            User thisUser = await _userManager.GetUserAsync(HttpContext.User);

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

            _dataRepository.AddEntity(newMessage);

            if (_dataRepository.SaveAll())
            {
                _logger.LogError("Ok new message was saved");
            }

            NewMessageModel returnData = new NewMessageModel
            {
                MessageId = _dataRepository.GetMessageIdByTimestampAndUser(timestamp, thisUser),
                MessageDate = timestamp.ToString(CultureInfo.InvariantCulture)
            };

            return Json(returnData);
        }

        [HttpPost]
        public async Task<IActionResult> ReadMessages([FromBody] string senderId)
        {
            User toThisUser = await _userManager.GetUserAsync(HttpContext.User);
            if (_dataRepository.ReadAllMessagesFrom(senderId, toThisUser))
            {
                return Ok("OK messages were read!");
            }

            return BadRequest("Could not read messages");
        }
    }
}
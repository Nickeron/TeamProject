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
    public class FriendController : Controller
    {
        private readonly IDataRepository dataRepository;
        private readonly UserManager<User> userManager;
        private readonly ILogger<FriendController> logger;

        public FriendController(IDataRepository dataRepository,
            UserManager<User> userManager,
            IHostingEnvironment env,
            ILogger<FriendController> logger)
        {
            this.dataRepository = dataRepository;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<IActionResult> Find()
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            var allAvailableFriends = dataRepository.GetAllStrangeUsers(await userManager.GetUserAsync(HttpContext.User));
            List<UserModel> ToBeFriends = new List<UserModel>();
            foreach (User availableFriend in allAvailableFriends)
            {
                UserModel toBeFriend = new UserModel
                {
                    ID = availableFriend.Id,
                    Avatar = availableFriend.UserAvatar,
                    UserName = availableFriend.UserName,
                    Name = availableFriend.FirstName + " " + availableFriend.LastName,
                    FriendshipStatus = dataRepository.GetFriendship(thisUser, availableFriend),
                    TopInterests = dataRepository.GetTopUsersInterests(availableFriend)
                };
                ToBeFriends.Add(toBeFriend);
            }
            FriendViewModel data = new FriendViewModel
            {
                ThisUser = thisUser,
                SentRequests = ToBeFriends.Where(u => u.FriendshipStatus == Friendship.removeRequest).ToList(),
                ReceivedRequests = ToBeFriends.Where(u => u.FriendshipStatus == Friendship.acceptRequest).ToList(),
                OtherUsers = ToBeFriends.Where(u => u.FriendshipStatus == Friendship.addFriend).ToList()
            };
            logger.LogInformation("User " + thisUser.UserName + " navigated to Find Friends Page");

            return View(data);
        }


        [Route("/makefriend/{userid}")]
        [HttpGet("{userid}")]
        public async Task<IActionResult> Make(string userid)
        {
            User theNewFriend = await userManager.FindByIdAsync(userid);
            User thisUser = await userManager.GetUserAsync(HttpContext.User);

            if (theNewFriend is null || thisUser is null) throw new Exception();

            Friend newFriendship = new Friend
            {
                Receiver = theNewFriend,
                Sender = thisUser,
                Accept = false
            };

            dataRepository.AddEntity(newFriendship);
            if (dataRepository.SaveAll())
            {
                logger.LogInformation("Ok a new friend request was created");
            };
            return RedirectToAction("Personal", "Home", new { username = theNewFriend.UserName });
        }

        [Route("/addfriend/{userid}")]
        [HttpGet("{userid}")]
        public async Task<IActionResult> Accept(string userid)
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            User theNewFriend = await userManager.FindByIdAsync(userid);

            if (userid is null || thisUser is null) throw new Exception();

            Friend friendship = dataRepository.GetFriend(thisUser, userid);
            friendship.Accept = true;
            if (dataRepository.SaveAll())
            {
                logger.LogInformation("Ok a new friendship was created");
            };
            return RedirectToAction("Personal", "Home", new { username = theNewFriend.UserName });
        }

        [Route("/unfriend/{userid}")]
        [HttpGet("{userid}")]
        public async Task<IActionResult> Remove(string userid)
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            User theNewFriend = await userManager.FindByIdAsync(userid);

            if (userid is null || thisUser is null) throw new Exception();

            Friend friendship = dataRepository.GetFriend(thisUser, userid);

            dataRepository.DeleteEntity(friendship);
            if (dataRepository.SaveAll())
            {
                logger.LogInformation("Ok a friendship was broken successfully");
            };
            return RedirectToAction("Personal", "Home", new { username = theNewFriend.UserName });
        }
    }
}

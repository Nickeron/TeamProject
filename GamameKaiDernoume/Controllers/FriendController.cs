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
        private readonly IHostingEnvironment env;
        private readonly ILogger<FriendController> logger;

        

        public FriendController(IDataRepository dataRepository,
            UserManager<User> userManager,
            IHostingEnvironment env,
            ILogger<FriendController> logger)
        {
            this.dataRepository = dataRepository;
            this.userManager = userManager;
            this.env = env;
            this.logger = logger;
        }

        public async Task<IActionResult> FindFriends()
        {
            var allAvailableFriends = dataRepository.GetAllStrangeUsers(await userManager.GetUserAsync(HttpContext.User));

            return View(allAvailableFriends);
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
                logger.LogError("Ok a new friend request was created");
                return Ok("New friendship saved!");
            };
            return BadRequest("Something bad happened");
        }

        [Route("/addfriend/{userid}")]
        [HttpGet("{userid}")]
        public async Task<IActionResult> Accept(string userid)
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);

            if (userid is null || thisUser is null) throw new Exception();

            Friend friendship = dataRepository.GetFriend(thisUser, userid);
            friendship.Accept = true;
            if (dataRepository.SaveAll())
            {
                logger.LogError("Ok a new friend request was created");
                return Ok("New friendship saved!");
            };
            return BadRequest("Something bad happened");
        }

        [Route("/unfriend/{userid}")]
        [HttpGet("{userid}")]
        public async Task<IActionResult> Remove(string userid)
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);

            if (userid is null || thisUser is null) throw new Exception();

            Friend friendship = dataRepository.GetFriend(thisUser, userid);

            dataRepository.DeleteEntity(friendship);
            if (dataRepository.SaveAll())
            {
                logger.LogError("Ok a new friend request was created");
                return Ok("New friendship saved!");
            };
            return BadRequest("Something bad happened");
        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TeamProject.Models;
using TeamProject.Data.Entities;
using TeamProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace TeamProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataRepository dataRepository;
        private readonly UserManager<User> userManager;
        private readonly IHostingEnvironment env;
        private readonly ILogger<HomeController> logger;

        public HomeController(IDataRepository dataRepository,
            UserManager<User> userManager,
            IHostingEnvironment env,
            ILogger<HomeController> logger)
        {
            this.dataRepository = dataRepository;
            this.userManager = userManager;
            this.env = env;
            this.logger = logger;
        }

        [Authorize]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            MyWallViewModel MyWallData = new MyWallViewModel
            {
                ThisUser = thisUser,
                Posts = dataRepository.GetAllPostsForUser(thisUser).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };

            return View(MyWallData);
        }

        [Authorize]
        [Route("/{id}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Index(int id)
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            MyWallViewModel MyWallData = new MyWallViewModel
            {
                ThisUser = thisUser,
                Posts = dataRepository.GetAllPostsForUserByInterest(thisUser, id).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };

            return View(MyWallData);
        }

        [Authorize]
        [Route("/profile")]
        public async Task<IActionResult> Personal()
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            MyWallViewModel MyWallData = new MyWallViewModel
            {
                ThisUser = thisUser,
                Posts = dataRepository.GetAllPostsByUser(thisUser).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };

            return View(MyWallData);
        }

        [Authorize]
        [Route("/profile/{username}")]
        [HttpGet("{username}")]
        public IActionResult Personal(string username)
        {
            User thisUser = userManager.Users.FirstOrDefault(u => u.UserName == username);
            MyWallViewModel MyWallData = new MyWallViewModel
            {
                ThisUser = thisUser,
                Posts = dataRepository.GetAllPostsByUser(thisUser).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };

            return View(MyWallData);
        }

        [Authorize]
        public async Task<IActionResult> Messenger()
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            List<Message> allUsersMessages = (List<Message>)dataRepository.GetAllMessagesOfUser(thisUser);
            User lastCommUser = null;
            if (allUsersMessages.Count > 0)
            {
                if (allUsersMessages.LastOrDefault().Receiver.Id == thisUser.Id)
                {
                    lastCommUser = allUsersMessages.LastOrDefault().Sender;
                }
                else
                {
                    lastCommUser = allUsersMessages.LastOrDefault().Receiver;
                }
            }
            MessengerViewModel messengerView = new MessengerViewModel
            {
                ThisUser = thisUser,
                LatestCommunicator = lastCommUser,
                UsersMessages = allUsersMessages,
                ThisUsersFriends = dataRepository.GetUsersFriends(thisUser)
            };
            return View(messengerView);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Messenger([FromBody]string UserID)
        {
            User correspondant = await userManager.FindByIdAsync(UserID);
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            IEnumerable<Message> allUsersMessages = dataRepository.GetAllMessagesOfUsers(thisUser, correspondant);

            MessengerViewModel messengerView = new MessengerViewModel
            {
                LatestCommunicator = correspondant,
                UsersMessages = allUsersMessages,
            };
            return Json(messengerView);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody]SendMessageModel messageModel)
        {
            User receiver = await userManager.FindByIdAsync(messageModel.ReceiverID);
            User thisUser = await userManager.GetUserAsync(HttpContext.User);

            if (receiver is null || thisUser is null) throw new Exception("Cannot have null receiver or sender");

            Message newMessage = new Message
            {
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

        [Authorize]
        public IActionResult CreateInterest()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateInterest(Interest newInterest)
        {
            Interest theNewInterest = new Interest
            {
                InterestCategory = newInterest.InterestCategory
            };

            dataRepository.AddEntity(theNewInterest);
            if (dataRepository.SaveAll())
            {
                logger.LogError("Ok new interest was created and saved to database");
            };
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

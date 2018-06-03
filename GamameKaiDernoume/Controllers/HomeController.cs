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
                ProfileUser = thisUser,
                TopUserInterests = dataRepository.GetTopUsersInterests(thisUser),
                Posts = dataRepository.GetAllPostsByUser(thisUser).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };

            return View(MyWallData);
        }

        [Authorize]
        [Route("/profile/{username}")]
        [HttpGet("{username}")]
        public async Task<IActionResult> Personal(string username)
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);                        
            User profileUser = userManager.Users.FirstOrDefault(u => u.UserName == username);

            MyWallViewModel MyWallData = new MyWallViewModel
            {
                ThisUser = thisUser,
                ProfileUser = profileUser,
                TopUserInterests = dataRepository.GetTopUsersInterests(profileUser),
                FriendshipStatus = dataRepository.GetFriendship(thisUser, profileUser),
                Posts = dataRepository.GetAllPostsByUser(profileUser).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };

            return View(MyWallData);
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

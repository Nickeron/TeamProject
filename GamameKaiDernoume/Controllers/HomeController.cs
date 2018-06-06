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

namespace TeamProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataRepository dataRepository;
        private readonly UserManager<User> userManager;
        private readonly ILogger<HomeController> logger;

        public HomeController(IDataRepository dataRepository,
            UserManager<User> userManager,
            ILogger<HomeController> logger)
        {
            this.dataRepository = dataRepository;
            this.userManager = userManager;
            this.logger = logger;
        }

        [Authorize]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            if (thisUser is null)
            {
                throw new System.Exception("In index httpContext had no user");
            }
            MyWallViewModel MyWallData = new MyWallViewModel
            {
                ThisUser = thisUser,
                Posts = dataRepository.GetAllPostsForUser(thisUser).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };
            logger.LogInformation("User " + thisUser.UserName + " navigated to Home Page");
            return View(MyWallData);
        }

        [Authorize]
        [Route("/search-by-interest/{id}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Index(int id)
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            if (thisUser is null)
            {
                throw new System.Exception("In index httpContext had no user");
            }
            MyWallViewModel MyWallData = new MyWallViewModel
            {
                ThisUser = thisUser,
                Posts = dataRepository.GetAllPostsForUserByInterest(thisUser, id).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };
            logger.LogInformation("User " + thisUser.UserName + " navigated to Home Page");
            return View(MyWallData);
        }

        [Authorize]
        [Route("/profile")]
        public async Task<IActionResult> Personal()
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            if (thisUser is null)
            {
                throw new System.Exception("In Personal httpContext had no user");
            }
            MyWallViewModel MyWallData = new MyWallViewModel
            {
                ThisUser = thisUser,
                ProfileUser = thisUser,
                TopUserInterests = dataRepository.GetTopUsersInterests(thisUser),
                Posts = dataRepository.GetAllPostsByUser(thisUser).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };
            logger.LogInformation("User " + thisUser.UserName + " navigated to his personal Page");
            return View(MyWallData);
        }

        [Authorize]
        [Route("/profile/{username}")]
        [HttpGet("{username}")]
        public async Task<IActionResult> Personal(string username)
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            if (thisUser is null)
            {
                throw new System.Exception("In Personal httpContext had no user");
            }
            User profileUser = userManager.Users.FirstOrDefault(u => u.UserName == username);

            if (profileUser is null)
            {
                throw new System.Exception("In Personal, username " + username + " matched no user");
            }
            MyWallViewModel MyWallData = new MyWallViewModel
            {
                ThisUser = thisUser,
                ProfileUser = profileUser,
                TopUserInterests = dataRepository.GetTopUsersInterests(profileUser),
                FriendshipStatus = dataRepository.GetFriendship(thisUser, profileUser),
                Posts = dataRepository.GetAllPostsByUser(profileUser).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };
            logger.LogInformation("User " + thisUser.UserName + " navigated to " + profileUser.UserName + "'s Page");
            return View(MyWallData);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
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

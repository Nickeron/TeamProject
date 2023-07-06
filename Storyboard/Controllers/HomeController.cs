using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TeamProject.Data;
using TeamProject.Data.Entities;
using TeamProject.Models;

namespace TeamProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataRepository _dataRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IDataRepository dataRepository,
            UserManager<User> userManager,
            ILogger<HomeController> logger)
        {
            _dataRepository = dataRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var thisUser = await _userManager.GetUserAsync(HttpContext.User);
            if (thisUser is null)
            {
                throw new System.Exception("In index httpContext had no user");
            }
            
            var myWallData = new MyWallViewModel
            {
                ThisUser = thisUser,
                Posts = _dataRepository.GetAllPostsForUser(thisUser).ToList(),
                Interests = _dataRepository.GetAllInterests().ToList()
            };
            _logger.LogInformation("User " + thisUser.UserName + " navigated to Home Page");
            return View(myWallData);
        }

        [Authorize]
        [Route("/search-by-interest/{id}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Index(int id)
        {
            User thisUser = await _userManager.GetUserAsync(HttpContext.User);
            if (thisUser is null)
            {
                throw new System.Exception("In index httpContext had no user");
            }
            MyWallViewModel MyWallData = new MyWallViewModel
            {
                ThisUser = thisUser,
                Posts = _dataRepository.GetAllPostsForUserByInterest(thisUser, id).ToList(),
                Interests = _dataRepository.GetAllInterests().ToList()
            };
            _logger.LogInformation("User " + thisUser.UserName + " navigated to Home Page");
            return View(MyWallData);
        }

        [Authorize]
        [Route("/profile")]
        public async Task<IActionResult> Personal()
        {
            User thisUser = await _userManager.GetUserAsync(HttpContext.User);
            if (thisUser is null)
            {
                throw new System.Exception("In Personal httpContext had no user");
            }
            MyWallViewModel myWallData = new MyWallViewModel
            {
                ThisUser = thisUser,
                ProfileUser = thisUser,
                TopUserInterests = _dataRepository.GetTopUsersInterests(thisUser),
                Posts = _dataRepository.GetAllPostsByUser(thisUser).ToList(),
                Interests = _dataRepository.GetAllInterests().ToList()
            };
            _logger.LogInformation("User " + thisUser.UserName + " navigated to his personal Page");
            return View(myWallData);
        }

        [Authorize]
        [Route("/profile/{username}")]
        [HttpGet("{username}")]
        public async Task<IActionResult> Personal(string username)
        {
            User thisUser = await _userManager.GetUserAsync(HttpContext.User);
            if (thisUser is null)
            {
                throw new System.Exception("In Personal httpContext had no user");
            }
            User profileUser = _userManager.Users.FirstOrDefault(u => u.UserName == username);

            if (profileUser is null)
            {
                throw new System.Exception("In Personal, username " + username + " matched no user");
            }
            MyWallViewModel myWallData = new MyWallViewModel
            {
                ThisUser = thisUser,
                ProfileUser = profileUser,
                TopUserInterests = _dataRepository.GetTopUsersInterests(profileUser),
                FriendshipStatus = _dataRepository.GetFriendship(thisUser, profileUser),
                Posts = _dataRepository.GetAllPostsByUser(profileUser).ToList(),
                Interests = _dataRepository.GetAllInterests().ToList()
            };
            _logger.LogInformation("User " + thisUser.UserName + " navigated to " + profileUser.UserName + "'s Page");
            return View(myWallData);
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

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TeamProject.Data;
using TeamProject.Data.Entities;
using TeamProject.Models;

namespace TeamProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IDataRepository _dataRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IDataRepository dataRepository,
            UserManager<User> userManager,
            ILogger<UsersController> logger)
        {
            _dataRepository = dataRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [Route("/manageusers")]
        public async Task<IActionResult> Manage()
        {
            var thisUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsers = _dataRepository.GetAllUsersExcept(thisUser);

            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            var accessUsers = allUsers.Select(assessUser =>
                    new UserManagementModel
                    {
                        Id = assessUser.Id,
                        Avatar = assessUser.UserAvatar,
                        UserName = assessUser.UserName,
                        Name = assessUser.FirstName + " " + assessUser.LastName,
                        IsAdmin = admins.Select(u => u.Id).Contains(assessUser.Id)
                    })
                .ToList();

            _logger.LogInformation("Admin {Name} navigated to manage users Page", thisUser.UserName);
            return View(accessUsers);
        }

        [HttpPost]
        public async Task<IActionResult> MakeAdministrator([FromBody] string userid)
        {
            if (userid is null) throw new Exception();
            var upToAdmin = await _userManager.FindByIdAsync(userid);

            await _userManager.AddToRoleAsync(upToAdmin, "Admin");
            if (_dataRepository.SaveAll())
            {
                _logger.LogInformation("Ok the user was promoted");
            }

            return Ok("User Promoted");
        }

        [HttpPost]
        public async Task<IActionResult> MakeUser([FromBody] string userid)
        {
            if (userid is null) throw new Exception();
            var backToUser = await _userManager.FindByIdAsync(userid);

            await _userManager.RemoveFromRoleAsync(backToUser, "Admin");
            if (_dataRepository.SaveAll())
            {
                _logger.LogInformation("Ok the admin was downgraded");
            }

            return Ok("Admin no more");
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] string userid)
        {
            var toBeDeleted = await _userManager.FindByIdAsync(userid);

            if (userid is null) throw new Exception();

            _dataRepository.DeleteUser(toBeDeleted);
            if (_dataRepository.SaveAll())
            {
                _logger.LogInformation("Ok the user was deleted");
            }
            return Ok("User Deleted");
        }
    }
}
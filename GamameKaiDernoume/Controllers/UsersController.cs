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
    public class UsersController : Controller
    {
        private readonly IDataRepository dataRepository;
        private readonly UserManager<User> userManager;
        private readonly IHostingEnvironment env;
        private readonly ILogger<UsersController> logger;

        public UsersController(IDataRepository dataRepository,
            UserManager<User> userManager,
            IHostingEnvironment env,
            ILogger<UsersController> logger)
        {
            this.dataRepository = dataRepository;
            this.userManager = userManager;
            this.env = env;
            this.logger = logger;
        }

        public async Task<IActionResult> Manage()
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            var allUsers = dataRepository.GetAllUsersExcept(thisUser);

            List<UserManagementModel> accessUsers = new List<UserManagementModel>();
            var admins = await userManager.GetUsersInRoleAsync("Admin");

            foreach (User assessUser in allUsers)
            {
                UserManagementModel managedUser = new UserManagementModel
                {
                    ID = assessUser.Id,
                    Avatar = assessUser.UserAvatar,
                    UserName = assessUser.UserName,
                    Name = assessUser.FirstName + " " + assessUser.LastName,
                    IsAdmin = admins.Select(u => u.Id).Contains(assessUser.Id)
                };
                accessUsers.Add(managedUser);
            }
            return View(accessUsers);
        }

        [HttpPost]
        public async Task<IActionResult> MakeAdministrator([FromBody]string userid)
        {
            if (userid is null) throw new Exception();
            User upToAdmin = await userManager.FindByIdAsync(userid);            

            await userManager.AddToRoleAsync(upToAdmin, "Admin");
            if (dataRepository.SaveAll())
            {
                logger.LogError("Ok the user was promoted");
            };
            return Ok("User Promoted");
        }

        [HttpPost]
        public async Task<IActionResult> MakeUser([FromBody]string userid)
        {
            if (userid is null) throw new Exception();
            User backToUser = await userManager.FindByIdAsync(userid);            

            await userManager.RemoveFromRoleAsync(backToUser, "Admin");
            if (dataRepository.SaveAll())
            {
                logger.LogError("Ok the admin was downgraded");
            };
            return Ok("Admin no more");
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]string userid)
        {
            User toBeDeleted = await userManager.FindByIdAsync(userid);

            if (userid is null) throw new Exception();

            dataRepository.DeleteEntity(toBeDeleted);
            if (dataRepository.SaveAll())
            {
                logger.LogError("Ok the user was deleted");
            };
            return Ok("User Deleted");
        }
    }
}

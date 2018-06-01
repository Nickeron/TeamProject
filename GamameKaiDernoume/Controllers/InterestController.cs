using System;
using System.Collections.Generic;
using System.IO;
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
    [Authorize(Roles = "Admin")]
    public class InterestController : Controller
    {
        private readonly IDataRepository dataRepository;
        private readonly UserManager<User> userManager;
        private readonly IHostingEnvironment env;
        private readonly ILogger<InterestController> logger;

        public InterestController(IDataRepository dataRepository,
            UserManager<User> userManager,
            IHostingEnvironment env,
            ILogger<InterestController> logger)
        {
            this.dataRepository = dataRepository;
            this.userManager = userManager;
            this.env = env;
            this.logger = logger;
        }

        
        public IActionResult Manage()
        {
            InterestViewModel interestData = new InterestViewModel
            {
                Interests = dataRepository.GetAllInterests().ToList()
            };

            return View(interestData);
        }

        [HttpPost]
        public IActionResult Create(Interest newInterest)
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

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]int id)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);

            Post toDelete = dataRepository.GetPostById(id);

            dataRepository.DeleteEntity(toDelete);

            if (dataRepository.SaveAll())
            {
                logger.LogInformation("saved");
            };
            return Ok("Comment Deleted");
        }
    }
}

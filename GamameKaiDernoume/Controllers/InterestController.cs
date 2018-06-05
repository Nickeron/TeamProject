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
        private readonly ILogger<InterestController> logger;

        public InterestController(IDataRepository dataRepository,
            UserManager<User> userManager,
            ILogger<InterestController> logger)
        {
            this.dataRepository = dataRepository;
            this.userManager = userManager;
            this.logger = logger;
        }


        public IActionResult Manage()
        {
            InterestViewModel interestData = new InterestViewModel
            {
                Interests = dataRepository.GetAllInterests().ToList()
            };
            logger.LogInformation("Admin navigated to Interest Management Page");
            return View(interestData);
        }

        [HttpPost]
        public IActionResult Create([FromBody]string newInterest)
        {
            Interest theNewInterest = new Interest
            {
                InterestCategory = newInterest
            };

            dataRepository.AddEntity(theNewInterest);
            if (dataRepository.SaveAll())
            {
                logger.LogInformation("Ok new interest was created and saved to database");
            };
            return Ok("Interest Created");
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody]InterestPostViewModel interestData)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);

            Interest toEdit = dataRepository.GetInterestById(interestData.InterestId);
            toEdit.InterestCategory = interestData.InterestCategory;

            if (dataRepository.SaveAll())
            {
                logger.LogInformation("Interest with id: " + toEdit.InterestID + " successfully changed to " + toEdit.InterestCategory);
            };
            return Ok("Interest Edited");
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]int id)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);

            Interest toDelete = dataRepository.GetInterestById(id);

            dataRepository.DeleteEntity(toDelete);

            if (dataRepository.SaveAll())
            {
                logger.LogInformation("Deletion of interest was Successfull");
            };
            return Ok("Interest Deleted");
        }
    }
}

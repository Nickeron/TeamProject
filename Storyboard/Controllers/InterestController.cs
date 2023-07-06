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
    public class InterestController : Controller
    {
        private readonly IDataRepository _dataRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<InterestController> _logger;

        public InterestController(IDataRepository dataRepository,
            UserManager<User> userManager,
            ILogger<InterestController> logger)
        {
            _dataRepository = dataRepository;
            _userManager = userManager;
            _logger = logger;
        }


        public IActionResult Manage()
        {
            InterestViewModel interestData = new InterestViewModel
            {
                Interests = _dataRepository.GetAllInterests().ToList()
            };
            _logger.LogInformation("Admin navigated to Interest Management Page");
            return View(interestData);
        }

        [HttpPost]
        public IActionResult Create([FromBody]string newInterest)
        {
            var theNewInterest = new Interest
            {
                InterestCategory = newInterest
            };

            _dataRepository.AddEntity(theNewInterest);
            if (_dataRepository.SaveAll())
            {
                _logger.LogInformation("Ok new interest was created and saved to database");
            }
            return Ok("Interest Created");
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody]InterestPostViewModel interestData)
        {
            var thisUser = await _userManager.GetUserAsync(HttpContext.User);

            Interest toEdit = _dataRepository.GetInterestById(interestData.InterestId);
            toEdit.InterestCategory = interestData.InterestCategory;

            if (_dataRepository.SaveAll())
            {
                _logger.LogInformation("Interest with id: " + toEdit.InterestID + " successfully changed to " + toEdit.InterestCategory);
            }
            return Ok("Interest Edited");
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]int id)
        {
            var thisUser = await _userManager.GetUserAsync(HttpContext.User);

            var toDelete = _dataRepository.GetInterestById(id);

            _dataRepository.DeleteEntity(toDelete);

            if (_dataRepository.SaveAll())
            {
                _logger.LogInformation("Deletion of interest was Successful");
            }
            return Ok("Interest Deleted");
        }
    }
}

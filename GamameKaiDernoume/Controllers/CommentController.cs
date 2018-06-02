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
    public class CommentController : Controller
    {
        private readonly IDataRepository dataRepository;
        private readonly UserManager<User> userManager;
        private readonly IHostingEnvironment env;
        private readonly ILogger<CommentController> logger;

        public CommentController(IDataRepository dataRepository,
            UserManager<User> userManager,
            IHostingEnvironment env,
            ILogger<CommentController> logger)
        {
            this.dataRepository = dataRepository;
            this.userManager = userManager;
            this.env = env;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddToPost([FromBody]CommentModel CommentData)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);

            if (ModelState.IsValid)
            {
                Post commentedPost = dataRepository.GetPostById(CommentData.PostID);
                DateTime timeStamp = DateTime.UtcNow;
                Comment newComment = new Comment
                {
                    User = thisUser,
                    CommentDate = timeStamp,
                    CommentText = CommentData.CommentText,
                    Post = commentedPost
                };

                dataRepository.AddEntity(newComment);

                if (dataRepository.SaveAll())
                {
                    logger.LogError("saved");
                };
                Comment savedComment = await dataRepository.GetCommentByDate(timeStamp);
                return Json(savedComment.CommentID);
            }
            return BadRequest("Something bad happened");
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody]CommentModel CommentData)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);
            Comment toEditComment = await dataRepository.GetCommentById(CommentData.CommentID);
            toEditComment.CommentText = CommentData.CommentText;
            toEditComment.CommentDate = DateTime.UtcNow;

            if (dataRepository.SaveAll())
            {
                logger.LogError("saved");
            };
            return Ok(" Comment Eddited");

        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]int id)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);

            Comment toDelete = await dataRepository.GetCommentById(id);

            dataRepository.DeleteEntity(toDelete);

            if (dataRepository.SaveAll())
            {
                logger.LogInformation("saved");
            };
            return Ok("Comment Deleted");
        }
    }
}

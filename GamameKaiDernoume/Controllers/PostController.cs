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
    [Authorize]
    public class PostController : Controller
    {

        private readonly IDataRepository dataRepository;
        private readonly UserManager<User> userManager;
        private readonly IHostingEnvironment env;
        private readonly ILogger<PostController> logger;

        public PostController(IDataRepository dataRepository,
            UserManager<User> userManager,
            IHostingEnvironment env,
            ILogger<PostController> logger)
        {
            this.dataRepository = dataRepository;
            this.userManager = userManager;
            this.env = env;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CreatePostViewModel newPostData)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);

            if (ModelState.IsValid)
            {
                List<Interest> addedInterests = new List<Interest>();
                if (!(newPostData.Interests[0] is null))
                {
                    List<Interest> interests = (List<Interest>)dataRepository.GetAllInterests();
                    foreach (Interest interest in interests)
                    {
                        if (newPostData.Interests[0].Contains(interest.InterestCategory))
                        {
                            addedInterests.Add(interest);
                        }
                    }
                }

                DateTime timeStamp = DateTime.Now;
                Post theNewPost = new Post
                {
                    User = thisUser,
                    PostDate = timeStamp,
                    PostText = newPostData.PostText,
                    PostScope = (Scope)newPostData.PostScope
                };

                dataRepository.AddEntity(theNewPost);

                if (dataRepository.SaveAll())
                {
                    logger.LogError("Ok new post was saved");
                };
                Post savedPost = dataRepository.GetPostByTimeStamp(timeStamp);

                var image = newPostData.PostImage;
                if (image != null && image.Length > 0)
                {
                    if (image.ContentType.Contains("png") || image.ContentType.Contains("jpg") || image.ContentType.Contains("jpeg"))
                    {
                        var filePath = env.WebRootPath + "\\postImages\\" + savedPost.PostID + '.' + image.ContentType.Split('/')[1];

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }
                        savedPost.PostImage = "/postImages/" + savedPost.PostID + '.' + image.ContentType.Split('/')[1];
                    }
                }

                foreach (Interest interest in addedInterests)
                {
                    PostInterest postInterest = new PostInterest
                    {
                        Interest = interest,
                        Post = savedPost
                    };
                    dataRepository.AddEntity(postInterest);
                }
                if (dataRepository.SaveAll())
                {
                    logger.LogError("Ok relationship of post and interests was saved");
                };
                return Ok("Ok new post was saved");
            }
            return BadRequest("Something was missing");

        }

        [HttpPost]
        public async Task<IActionResult> AddReaction([FromBody]ReactionModel ReactionData)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);
            Post reactedPost;
            if (ModelState.IsValid)
            {

                Reaction postReaction = dataRepository.GetReactionByPostAndUser(ReactionData.PostID, thisUser);

                // Does the reaction already exist?
                if (postReaction is null)
                {
                    reactedPost = dataRepository.GetPostById(ReactionData.PostID);
                    postReaction = new Reaction
                    {
                        Post = reactedPost,
                        User = thisUser,
                        IsLike = ReactionData.IsLike
                    };

                    dataRepository.AddEntity(postReaction);

                    if (dataRepository.SaveAll())
                    {
                        logger.LogInformation("NEW Reaction Saved to Database");
                    };
                }
                else
                {
                    if (postReaction.IsLike != ReactionData.IsLike)
                    {
                        postReaction.IsLike = ReactionData.IsLike;
                        if (dataRepository.SaveAll())
                        {
                            logger.LogInformation("Reaction was changed and Saved to Database");
                        };
                    }
                }
            }
            reactedPost = dataRepository.GetPostById(ReactionData.PostID);

            // Return the new amount of likes and dislikes
            return Json(new
            {
                likes = reactedPost.Reactions.Where(r => r.IsLike).ToList().Count,
                dislikes = reactedPost.Reactions.Where(r => !r.IsLike).ToList().Count
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]int id)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);

            Post toDelete = dataRepository.GetPostById(id);

            if (dataRepository.DeletePost(toDelete))
            {
                logger.LogInformation("The post was deleted successfully");
            };
            return Ok("Post Deleted");
        }
    }
}

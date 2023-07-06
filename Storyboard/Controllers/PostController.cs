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

        private readonly IDataRepository _dataRepository;
        private readonly UserManager<User> _userManager;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<PostController> _logger;

        public PostController(IDataRepository dataRepository,
            UserManager<User> userManager,
            IHostingEnvironment env,
            ILogger<PostController> logger)
        {
            _dataRepository = dataRepository;
            _userManager = userManager;
            _env = env;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CreatePostViewModel newPostData)
        {
            var thisUser = await _userManager.GetUserAsync(HttpContext.User);

            if (ModelState.IsValid)
            {
                List<Interest> addedInterests = new List<Interest>();
                if (!(newPostData.Interests[0] is null))
                {
                    List<Interest> interests = (List<Interest>)_dataRepository.GetAllInterests();
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

                _dataRepository.AddEntity(theNewPost);

                if (_dataRepository.SaveAll())
                {
                    _logger.LogError("Ok new post was saved");
                };
                Post savedPost = _dataRepository.GetPostByTimeStamp(timeStamp);

                var image = newPostData.PostImage;
                if (image != null && image.Length > 0)
                {
                    if (image.ContentType.Contains("png") || image.ContentType.Contains("jpg") || image.ContentType.Contains("jpeg"))
                    {
                        var filePath = _env.WebRootPath + "\\postImages\\" + savedPost.PostID + '.' + image.ContentType.Split('/')[1];

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
                    _dataRepository.AddEntity(postInterest);
                }
                if (_dataRepository.SaveAll())
                {
                    _logger.LogError("Ok relationship of post and interests was saved");
                };
                return Ok("Ok new post was saved");
            }
            return BadRequest("Something was missing");

        }

        [HttpPost]
        public async Task<IActionResult> AddReaction([FromBody]ReactionModel reactionData)
        {
            var thisUser = await _userManager.GetUserAsync(HttpContext.User);
            Post reactedPost;
            if (ModelState.IsValid)
            {

                Reaction postReaction = _dataRepository.GetReactionByPostAndUser(reactionData.PostId, thisUser);

                // Does the reaction already exist?
                if (postReaction is null)
                {
                    reactedPost = _dataRepository.GetPostById(reactionData.PostId);
                    postReaction = new Reaction
                    {
                        Post = reactedPost,
                        User = thisUser,
                        IsLike = reactionData.IsLike
                    };

                    _dataRepository.AddEntity(postReaction);

                    if (_dataRepository.SaveAll())
                    {
                        _logger.LogInformation("NEW Reaction Saved to Database");
                    };
                }
                else
                {
                    if (postReaction.IsLike != reactionData.IsLike)
                    {
                        postReaction.IsLike = reactionData.IsLike;
                        if (_dataRepository.SaveAll())
                        {
                            _logger.LogInformation("Reaction was changed and Saved to Database");
                        };
                    }
                }
            }
            reactedPost = _dataRepository.GetPostById(reactionData.PostId);

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
            var thisUser = await _userManager.GetUserAsync(HttpContext.User);

            Post toDelete = _dataRepository.GetPostById(id);

            if (_dataRepository.DeletePost(toDelete))
            {
                _logger.LogInformation("The post was deleted successfully");
            }
            return Ok("Post Deleted");
        }
    }
}

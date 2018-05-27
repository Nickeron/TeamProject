﻿using System;
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
using System.Collections.Generic;

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
        public async Task<IActionResult> Index()
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            MyWallViewModel MyWallData = new MyWallViewModel
            {
                Posts = dataRepository.GetAllPostsForUser(thisUser).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };

            return View(MyWallData);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Index(int id)
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            MyWallViewModel MyWallData = new MyWallViewModel
            {
                Posts = dataRepository.GetAllPostsForUserByInterest(thisUser, id).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };

            return View(MyWallData);
        }

        [Authorize]
        public async Task<IActionResult> Personal()
        {
            User thisUser = await userManager.GetUserAsync(HttpContext.User);
            MyWallViewModel MyWallData = new MyWallViewModel
            {
                Posts = dataRepository.GetAllPostsByUser(thisUser).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };

            return View(MyWallData);
        }

        [Authorize]
        [HttpGet("{username}")]
        public IActionResult Personal(string username)
        {
            User thisUser = userManager.Users.FirstOrDefault(u=>u.UserName == username);
            logger.LogInformation("The username is !!!! :"+username + thisUser.UserName);
            MyWallViewModel MyWallData = new MyWallViewModel
            {
                Posts = dataRepository.GetAllPostsByUser(thisUser).ToList(),
                Interests = dataRepository.GetAllInterests().ToList()
            };

            return View(MyWallData);
        }

        [Authorize]
        public async Task<IActionResult> Messenger()
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);
            MessengerViewModel messengerView = new MessengerViewModel
            {
                ThisUserID = thisUser.Id,
                UserName = (thisUser.FirstName is null) ? thisUser.UserName : thisUser.FirstName,
                ThisUsersFriends = dataRepository.GetUsersFriends(thisUser)
            };
            return View(messengerView);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody]SendMessageModel messageModel)
        {
            User receiver = await userManager.FindByIdAsync(messageModel.ReceiverID);
            User thisUser = await userManager.GetUserAsync(HttpContext.User);

            if (receiver is null || thisUser is null) throw new Exception("Cannot have null receiver or sender");

            Message newMessage = new Message
            {
                MessageDate = DateTime.Now,
                MessageText = messageModel.MessageText,
                Receiver = receiver,
                Sender = thisUser
            };

            dataRepository.AddEntity(newMessage);

            if (dataRepository.SaveAll())
            {
                logger.LogError("Ok new message was saved");
                return Ok("New message saved!");
            };
            return BadRequest("Something bad happened");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddNewCommentToPost([FromBody]CommentModel CommentData)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);

            if (ModelState.IsValid)
            {
                Post commentedPost = dataRepository.GetPostById(CommentData.PostID);
                Comment newComment = new Comment
                {
                    User = thisUser,
                    CommentDate = DateTime.Now,
                    CommentText = CommentData.CommentText,
                    Post = commentedPost
                };

                dataRepository.AddEntity(newComment);

                if (dataRepository.SaveAll())
                {
                    logger.LogError("saved");
                };
                return Ok("New Comment Added");
            }
            return BadRequest("Something bad happened");

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReactionToPost([FromBody]ReactionModel ReactionData)
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
                        logger.LogInformation("Reaction Saved to Database");
                    };
                    //return Ok("New Reaction Added");
                }
                else
                {
                    //return Ok("No change");
                    if (postReaction.IsLike != ReactionData.IsLike)
                    {
                        postReaction.IsLike = ReactionData.IsLike;
                        if (dataRepository.SaveAll())
                        {
                            logger.LogInformation("Reaction Saved to Database");
                        };
                        //return Ok("Reaction Changed");
                    }
                }
            }
            reactedPost = dataRepository.GetPostById(ReactionData.PostID);

            return Json(new
            {
                likes = reactedPost.Reactions.Where(r => r.IsLike).ToList().Count,
                dislikes = reactedPost.Reactions.Where(r => !r.IsLike).ToList().Count
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody]CreatePostViewModel newPostData)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);

            if (ModelState.IsValid)
            {
                List<Interest> interests = (List<Interest>)dataRepository.GetAllInterests();
                List<Interest> addedInterests = new List<Interest>();
                foreach (Interest interest in interests)
                {
                    if (newPostData.Interests.Contains(interest.InterestCategory))
                    {
                        addedInterests.Add(interest);
                    }
                }

                DateTime timeStamp = DateTime.Now;
                Post theNewPost = new Post
                {
                    User = thisUser,
                    PostDate = timeStamp,
                    PostText = newPostData.PostText,
                    PostScope = (Scope) newPostData.PostScope
                };

                dataRepository.AddEntity(theNewPost);

                if (dataRepository.SaveAll())
                {
                    logger.LogError("Ok new post was saved");
                };
                Post savedPost = dataRepository.GetPostByTimeStamp(timeStamp);

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
                return Ok("Ok relationship of post and interests was saved");
            }
            return BadRequest("Something was missing");

        }

        [Authorize]
        public async Task<IActionResult> FindFriends()
        {
            var allAvailableFriends = dataRepository.GetAllStrangeUsers(await userManager.GetUserAsync(HttpContext.User));

            return View(allAvailableFriends);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendFriendRequest([FromBody]string UserId)
        {
            User theNewFriend = await userManager.FindByIdAsync(UserId);
            User thisUser = await userManager.GetUserAsync(HttpContext.User);

            if (theNewFriend is null || thisUser is null) throw new Exception();

            Friend newFriendship = new Friend
            {
                Receiver = theNewFriend,
                Sender = thisUser,
                Accept = false
            };

            dataRepository.AddEntity(newFriendship);
            if (dataRepository.SaveAll())
            {
                logger.LogError("Ok a new friend request was created");
                return Ok("New friendship saved!");
            };
            return BadRequest("Something bad happened");
        }

        [Authorize]
        public IActionResult CreateInterest()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateInterest(Interest newInterest)
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

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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

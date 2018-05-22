﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GamameKaiDernoume.Models;
using GamameKaiDernoume.Data.Entities;
using GamameKaiDernoume.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace GamameKaiDernoume.Controllers
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

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult CreatePost()
        {
            CreatePostViewModel theWantedPost = new CreatePostViewModel
            {
                newPostInterests = dataRepository.GetAllInterests()
            };
            return View(theWantedPost);
        }

        [HttpPost]
        public async Task<IActionResult> ShowUserPosts(CreatePostViewModel newPostViewModel)
        {
            var thisUser = await userManager.GetUserAsync(HttpContext.User);

            Post theNewPost = new Post
            {
                User = thisUser,
                PostDate = DateTime.Now,
                PostText = newPostViewModel.newPost.PostText,
                PostInterests = newPostViewModel.newPost.PostInterests,
                PostScope = Scope.Global
            };

            dataRepository.AddEntity(theNewPost);
            if (dataRepository.SaveAll())
            {
                logger.LogError("Ok ola mia xara");
            };

            return View(dataRepository.GetAllPostsByUser(thisUser.UserName, false));
        }

        [Authorize]
        public async Task<IActionResult> FindFriends()
        {
            var availableFriends = new FriendRequestViewModel
            {
                allAvailableFriends = dataRepository.GetAllStrangeUsers(await userManager.GetUserAsync(HttpContext.User))
            };
            return View(availableFriends);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendRequest(FriendRequestViewModel ToBeFriend)
        {
            User theNewFriend = await userManager.FindByIdAsync(ToBeFriend.UserToBeFriend.Id);
            if (theNewFriend is null) throw new Exception();
            Friend newFriendship = new Friend
            {
                Receiver = theNewFriend,
                Sender = await userManager.GetUserAsync(HttpContext.User),
                Accept = false
            };

            dataRepository.AddEntity(newFriendship);
            if (dataRepository.SaveAll())
            {
                logger.LogError("Ok ola mia xara");
            };
            return View(dataRepository.GetAllStrangeUsers(await userManager.GetUserAsync(HttpContext.User)));
        }

        [Authorize]
        public IActionResult CreateInterest()
        {
            return View();
        }

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
                logger.LogError("Ok ola mia xara");
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

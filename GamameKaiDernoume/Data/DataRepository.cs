﻿using GamameKaiDernoume.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamameKaiDernoume.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly ApplicationDbContext _ctx;
        private readonly ILogger<DataRepository> _logger;
        private readonly UserManager<User> userManager;

        public DataRepository(ApplicationDbContext ctx, 
            ILogger<DataRepository> logger,
            UserManager<User> userManager)
        {
            _ctx = ctx;
            _logger = logger;
            this.userManager = userManager;
        }

        public void AddEntity(object model)
        {
            _ctx.Add(model);
        }

        public IEnumerable<Post> GetAllPosts(bool includeReactions)
        {
            if (includeReactions)
            {

                return _ctx.Posts
                    .Include(u => u.Comments)
                    .Include(u => u.PostInterests)
                    .Include(u => u.Reactions)
                    .ToList();

            }
            else
            {
                return _ctx.Posts
                    .ToList();
            }
        }

        public IEnumerable<Post> GetAllPostsByUser(string username, bool includeComments)
        {
            if (includeComments)
            {

                return _ctx.Posts
                           .Where(o => o.User.UserName == username)
                           .Include(o => o.Reactions)
                           .Include(i => i.PostInterests)
                           .Include(i => i.Comments)
                           .ToList();

            }
            else
            {
                return _ctx.Posts
                           .Where(o => o.User.UserName == username)
                           .ToList();
            }
        }

        public IEnumerable<Interest> GetAllInterests()
        {
            try
            {
                _logger.LogInformation("Get All Interests was called");

                return _ctx.Interests
                           .OrderBy(p => p.InterestCategory)
                           .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all products: {ex}");
                return null;
            }
        }

        public Post GetPostById(string username, int id)
        {
            return _ctx.Posts
                       .Include(o => o.Comments)
                       .Include(o => o.Reactions)
                       .Include(o => o.PostInterests)
                       .Where(o => o.PostID == id && o.User.UserName == username)
                       .FirstOrDefault();
        }

        public IEnumerable<Post> GetPostsByCategoryInterest(PostInterest interestCategory)
        {
            return _ctx.Posts
                       .Where(p => p.PostInterests.Contains(interestCategory))
                       .ToList();
        }

        public bool SaveAll()
        {
            return _ctx.SaveChanges() > 0;
        }

        public IEnumerable<User> GetAllStrangeUsers(User thisUser)
        {
            IEnumerable<Friend> allKnown = _ctx.Friends.Where(u => u.Receiver.Id == thisUser.Id || u.Sender.Id == thisUser.Id).ToList();
            IEnumerable<string> allFriends = new List<string>() { thisUser.Id };
            foreach (Friend knownPerson in allKnown)
            {
                if (knownPerson.Receiver != thisUser)
                {
                    allFriends.Append(knownPerson.Receiver.Id);
                }
                if (knownPerson.Sender != thisUser)
                {
                    allFriends.Append(knownPerson.Sender.Id);
                }
            }
            foreach (var friend in allFriends)
            {
                _logger.LogCritical("!!!!!!!!!!!!!!!!"+friend);
            }
                

            return _ctx.Users.Where(u => !allFriends.Contains(u.Id)).ToList();
        }
    }
}
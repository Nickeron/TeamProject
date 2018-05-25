using TeamProject.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamProject.Data
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

        public IEnumerable<Post> GetAllPosts()
        {
            try
            {
                _logger.LogInformation("Get All Posts was called");
                return _ctx.Posts
                    .Include(u => u.Comments)
                    .Include(u => u.PostInterests)
                    .ThenInclude(u => u.Interest)
                    .Include(u => u.Reactions)
                    .OrderBy(p => p.PostDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all Posts: {ex}");
                return null;
            }
        }

        public IEnumerable<Post> GetAllPostsForUser(User user)
        {
            try
            {
                _logger.LogInformation("Get All Posts for User was called");
                var friends = GetUsersFriends(user);

                return _ctx.Posts
                           .Include(p => p.User)
                           .Include(o => o.Reactions)
                           .Include(i => i.Comments)
                           .Include(i => i.PostInterests)
                           .ThenInclude(i => i.Interest)
                           .Where(o => friends.Select(x => x.Id).Contains(o.User.Id) || o.PostScope == Scope.Global)
                           .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all Posts: {ex}");
                return null;
            }
        }

        public IEnumerable<Post> GetAllPostsByUser(User user)
        {
            try
            {
                _logger.LogInformation("Get All Posts by User was called");
                return _ctx.Posts
                           .Include(o => o.Reactions)
                           .Include(i => i.Comments)
                           .Include(i => i.PostInterests)
                           .ThenInclude(i => i.Interest)
                           .Where(o => o.User.Id == user.Id)
                           .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all Posts: {ex}");
                return null;
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
                _logger.LogError($"Failed to get all Interests: {ex}");
                return null;
            }
        }

        public Post GetPostByTimeStamp(DateTime timeStamp)
        {
            try
            {
                _logger.LogInformation("Get Post by Timestamp was called");

                return _ctx.Posts
                       .Include(o => o.Comments)
                       .Include(o => o.Reactions)
                       .Include(o => o.PostInterests)
                       .Where(o => o.PostDate == timeStamp)
                       .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get the requested Post: {ex}");
                return null;
            }
        }

        public Post GetPostById(int postID)
        {
            try
            {
                _logger.LogInformation("Get Post by ID was called");

                return _ctx.Posts
                .Include(p => p.PostInterests)
                .Include(p => p.Reactions)
                .Include(p => p.Comments)
                .Where(u => u.PostID.Equals(postID)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get the requested Post: {ex}");
                return null;
            }
        }

        public IEnumerable<Post> GetPostsByCategoryInterest(PostInterest interestCategory)
        {
            try
            {
                _logger.LogInformation("Get Post by By Category of Interest was called");

                return _ctx.Posts
                       .Where(p => p.PostInterests.Contains(interestCategory))
                       .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get the requested Posts: {ex}");
                return null;
            }
        }

        public bool SaveAll()
        {
            return _ctx.SaveChanges() > 0;
        }

        public IEnumerable<User> GetAllStrangeUsers(User thisUser)
        {
            IEnumerable<Friend> allKnown = _ctx.Friends
                .Include(u => u.Receiver)
                .Include(u => u.Sender)
                .Where(u => u.Receiver.Id == thisUser.Id || u.Sender.Id == thisUser.Id)
                .ToList();

            List<string> allFriends = new List<string>() { thisUser.Id };
            foreach (Friend knownPerson in allKnown)
            {
                if (knownPerson.Receiver.Id != thisUser.Id)
                {
                    allFriends.Add(knownPerson.Receiver.Id);
                }
                if (knownPerson.Sender.Id != thisUser.Id)
                {
                    allFriends.Add(knownPerson.Sender.Id);
                }
            }

            return _ctx.Users.Where(u => !allFriends.Contains(u.Id)).ToList();
        }

        public IEnumerable<User> GetUsersFriends(User thisUser)
        {
            IEnumerable<Friend> allKnown = _ctx.Friends
                .Include(u => u.Receiver)
                .Include(u => u.Sender)
                .Where(u => u.Receiver.Id == thisUser.Id || u.Sender.Id == thisUser.Id)
                .ToList();

            List<string> allFriends = new List<string>() { };
            foreach (Friend knownPerson in allKnown)
            {
                if (knownPerson.Receiver.Id != thisUser.Id)
                {
                    allFriends.Add(knownPerson.Receiver.Id);
                }
                if (knownPerson.Sender.Id != thisUser.Id)
                {
                    allFriends.Add(knownPerson.Sender.Id);
                }
            }

            return _ctx.Users.Where(u => allFriends.Contains(u.Id)).ToList();
        }

        public Reaction GetReactionByPostAndUser(int reactionPostId, User thisUser)
        {
            return _ctx.Reactions.Where(r => r.Post.PostID == reactionPostId && r.User == thisUser).FirstOrDefault();
        }
    }
}

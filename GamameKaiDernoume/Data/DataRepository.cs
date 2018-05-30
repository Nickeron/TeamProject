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

        public void DeleteEntity(object model)
        {
            _ctx.Remove(model);
        }

        public IEnumerable<Post> GetAllPosts()
        {
            try
            {
                _logger.LogInformation("Get All Posts was called");
                return _ctx.Posts
                    .Include(u => u.Comments)
                    .Include(u => u.Reactions)
                    .Include(u => u.PostInterests)
                    .ThenInclude(u => u.Interest)
                    .OrderByDescending(p => p.PostDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all Posts: {ex}");
                return null;
            }
        }

        public IEnumerable<Post> GetAllPostsForUser(User currentUser)
        {
            try
            {
                _logger.LogInformation("Get All Posts for User was called");
                List<User> friends = (List<User>)GetUsersFriends(currentUser);

                return _ctx.Posts
                           .Include(p => p.User)
                           .Include(o => o.Reactions)
                           .Include(i => i.Comments)
                           .Include(i => i.PostInterests)
                           .ThenInclude(i => i.Interest)
                           .Where(o => friends.Select(x => x.Id).Contains(o.User.Id) || o.PostScope == Scope.Global)
                           .OrderByDescending(p => p.PostDate)
                           .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all Posts: {ex}");
                return null;
            }
        }

        public IEnumerable<Post> GetAllPostsForUserByInterest(User currentUser, int interestId)
        {
            try
            {
                _logger.LogInformation("Get All Posts for User by interest was called");
                _logger.LogInformation("Before the call" + interestId);
                List<Post> allAvailablePosts = (List<Post>)GetAllPostsForUser(currentUser);

                foreach (Post everyPost in allAvailablePosts)
                {
                    foreach (PostInterest pi in everyPost.PostInterests)
                    {
                        if (pi.Interest.InterestID == interestId)
                            _logger.LogInformation("Interest Category:" + pi.Interest.InterestID);
                    }
                }

                return allAvailablePosts.Where(o => o.PostInterests.Select(i => i.InterestId).Contains(interestId));
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
                           .ThenInclude(c => c.User)
                           .Include(i => i.PostInterests)
                           .ThenInclude(i => i.Interest)
                           .Where(o => o.User.Id == user.Id)
                           .OrderByDescending(p => p.PostDate)
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

        public Comment GetCommentById(int commentID)
        {
            try
            {
                _logger.LogInformation("Get Comment by ID was called");

                return _ctx.Comments
                .Where(u => u.CommentID.Equals(commentID)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get the requested Comment: {ex}");
                return null;
            }
        }

        public IEnumerable<Post> GetPostsByCategoryInterest(PostInterest interestCategory)
        {
            try
            {
                _logger.LogInformation("Get Post by By Category of Interest was called");

                return _ctx.Posts
                    .Include(p => p.PostInterests)
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
            List<Friend> allKnown = _ctx.Friends
                .Include(u => u.Receiver)
                .Include(u => u.Sender)
                .Where(u => u.Receiver.Id == thisUser.Id || u.Sender.Id == thisUser.Id)
                .ToList();

            List<string> allFriends = new List<string>();

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

            return _ctx.Users
                .Include(u=>u.SentMessages)
                .Where(u => allFriends.Contains(u.Id)).ToList();
        }

        public Reaction GetReactionByPostAndUser(int reactionPostId, User thisUser)
        {
            return _ctx.Reactions.Where(r => r.Post.PostID == reactionPostId && r.User == thisUser).FirstOrDefault();
        }

        public IEnumerable<Message> GetAllMessagesOfUser(User thisUser)
        {
            try
            {
                _logger.LogInformation("Get All messages of user was called");

                return _ctx.Messages
                    .Include(p => p.Sender)
                    .Include(m=>m.Receiver)
                    .Where(m => m.Sender.Id == thisUser.Id || m.Receiver.Id == thisUser.Id)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get the requested Messages: {ex}");
                return null;
            }
        }

        public IEnumerable<Message> GetAllMessagesOfUsers(User thisUser, User talkUser)
        {
            try
            {
                _logger.LogInformation("Get All messages of users was called");

                return _ctx.Messages
                    .Include(p => p.Sender)
                    .Include(m => m.Receiver)
                    .Where(m => 
                    (m.Sender.Id == thisUser.Id && m.Receiver.Id == talkUser.Id) || 
                    (m.Sender.Id == talkUser.Id && m.Receiver.Id == thisUser.Id))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get the requested Messages: {ex}");
                return null;
            }
        }
    }
}

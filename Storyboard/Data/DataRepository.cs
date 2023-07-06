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
        private readonly UserManager<User> _userManager;

        public DataRepository(ApplicationDbContext ctx,
            ILogger<DataRepository> logger,
            UserManager<User> userManager)
        {
            _ctx = ctx;
            _logger = logger;
            _userManager = userManager;
        }

        public void AddEntity(object model)
        {
            _ctx.Add(model);
        }

        public void DeleteEntity(object model)
        {
            _ctx.Remove(model);
        }

        public bool DeletePost(Post toDelete)
        {
            try
            {
                _logger.LogInformation("Deleting post {DeletePostId}", toDelete.PostID);
                var allPostsComments = _ctx.Comments
                    .Include(u => u.Post)
                    .Where(u => u.Post.PostID == toDelete.PostID)
                    .ToList();

                if (allPostsComments.Count > 0)
                {
                    _ctx.RemoveRange(allPostsComments);
                    SaveAll();
                }

                List<Reaction> allPostsReactions = _ctx.Reactions
                    .Include(u => u.Post)
                    .Where(u => (u.Post.PostID == toDelete.PostID))
                    .ToList();

                if (allPostsReactions.Count > 0)
                {
                    _ctx.RemoveRange(allPostsReactions);
                    SaveAll();
                }
                _ctx.Remove(toDelete);
                return SaveAll();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete the requested Post: {Ex}", ex);
                return false;
            }
        }

        public bool DeleteUser(User toBeDeleted)
        {
            try
            {
                _logger.LogInformation("Deleting user " +toBeDeleted.UserName);
                List<Friend> allKnown = _ctx.Friends
                    .Include(u => u.Receiver)
                    .Include(u => u.Sender)
                    .Where(u => (u.Receiver.Id == toBeDeleted.Id || u.Sender.Id == toBeDeleted.Id))
                    .ToList();

                if (allKnown.Count > 0)
                {
                    _ctx.RemoveRange(allKnown);
                    SaveAll();
                }

                var allHisComments = _ctx.Comments
                    .Include(u => u.User)
                    .Where(u => (u.User.Id == toBeDeleted.Id))
                    .ToList();

                if (allHisComments.Count > 0)
                {
                    _ctx.RemoveRange(allHisComments);
                    SaveAll();
                }

                var allHisReactions = _ctx.Reactions
                    .Include(u => u.User)
                    .Where(u => (u.User.Id == toBeDeleted.Id))
                    .ToList();

                if (allHisReactions.Count > 0)
                {
                    _ctx.RemoveRange(allHisReactions);
                    SaveAll();
                }

                List<Post> allHisPosts = _ctx.Posts
                    .Include(u => u.User)
                    .Where(u => (u.User.Id == toBeDeleted.Id))
                    .ToList();

                if (allHisPosts.Count > 0)
                {
                    _ctx.RemoveRange(allHisPosts);
                    SaveAll();
                }

                List<Message> allHisMessages = _ctx.Messages
                    .Include(u => u.Sender)
                    .Include(u => u.Receiver)
                    .Where(u => (u.Sender.Id == toBeDeleted.Id) || (u.Receiver.Id == toBeDeleted.Id))
                    .ToList();

                if (allHisMessages.Count > 0)
                {
                    _ctx.RemoveRange(allHisMessages);
                    SaveAll();
                }
                _ctx.Remove(toBeDeleted);
                return SaveAll();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete the requested User: {Ex}", ex);
                return false;
            }
        }

        public Interest GetInterestById(int interestId)
        {
            try
            {
                _logger.LogInformation("Get Interest by ID was called");

                return _ctx.Interests
                           .SingleOrDefault(i => i.InterestID == interestId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get the requested Interest: {Ex}", ex);
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
                _logger.LogError("Failed to get all Interests: {Ex}", ex);
                return null;
            }
        }

        public List<Interest> GetTopUsersInterests(User thisUser)
        {
            try
            {
                _logger.LogInformation("Get Top Users: " + thisUser.UserName + "Interests was called");

                List<int> usersPostInterests =
                    GetAllPostsByUser(thisUser).SelectMany(p => p.PostInterests).Select(pi => pi.InterestId).ToList();

                List<int> topUsersInterests = usersPostInterests
                    .GroupBy(s => s)
                    .OrderByDescending(s => s.Count())
                    .Take(3).SelectMany(ui => ui).ToList();

                List<Interest> usersInterests = _ctx.Interests
                    .Include(i => i.PostInterests)
                    .Where(i => topUsersInterests.Contains(i.InterestID))
                    .ToList();

                return usersInterests;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get users Interests: {ex}");
                return null;
            }
        }
        public IEnumerable<User> GetAllUsersExcept(User thisUser)
        {
            return _ctx.Users.Where(u => u.Id != thisUser.Id);
        }

        public IEnumerable<User> GetAllStrangeUsers(User thisUser)
        {
            try
            {
                _logger.LogInformation("Get all strange Users for: " + thisUser.UserName + " was called");

                IEnumerable<Friend> allKnown = _ctx.Friends
                .Include(u => u.Receiver)
                .Include(u => u.Sender)
                .Where(u => (u.Receiver.Id == thisUser.Id || u.Sender.Id == thisUser.Id) && (u.Accept))
                .ToList();

                _logger.LogInformation("All strange Users successfully retrieved for: " + thisUser.UserName);
                
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
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all strange users: {ex}");
                return null;
            }
        }

        public IEnumerable<User> GetUsersFriends(User thisUser)
        {
            try
            {
                _logger.LogInformation("Get all friend Users for: " + thisUser.UserName + " was called");

                List<Friend> allKnown = _ctx.Friends
                .Include(u => u.Receiver)
                .Include(u => u.Sender)
                .Where(u =>
                (u.Receiver.Id == thisUser.Id || u.Sender.Id == thisUser.Id) && u.Accept)
                .ToList();

                _logger.LogInformation("All friend Users successfully retrieved for: " + thisUser.UserName);

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
                    .Include(u => u.SentMessages)
                    .Where(u => allFriends.Contains(u.Id)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all friend users: {ex}");
                return null;
            }
        }

        public Friend GetFriend(User thisUser, string friendsId)
        {
            try
            {
                _logger.LogInformation("Get friend was called for " + thisUser.UserName);
                return _ctx.Friends.SingleOrDefault(f =>
            (f.Receiver.Id == thisUser.Id && f.Sender.Id == friendsId) ||
            (f.Receiver.Id == friendsId && f.Sender.Id == thisUser.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get friend: {Ex}", ex);
                return null;
            }
        }

        public Friendship GetFriendship(User thisUser, User anotherUser)
        {
            try
            {
                _logger.LogInformation("Get friendship of "
                    + thisUser.UserName
                    + " and "
                    + anotherUser.UserName
                    + " was called");
                if (thisUser.Id == anotherUser.Id) { return Friendship.myself; }

                Friend friendship = _ctx.Friends.SingleOrDefault(f =>
                (f.Receiver.Id == thisUser.Id && f.Sender.Id == anotherUser.Id) ||
                (f.Receiver.Id == anotherUser.Id && f.Sender.Id == thisUser.Id));

                if (friendship is null) { return Friendship.addFriend; }
                else if (friendship.Receiver.Id == thisUser.Id && friendship.Accept == false) { return Friendship.acceptRequest; }
                else if (friendship.Sender.Id == thisUser.Id && friendship.Accept == false) { return Friendship.removeRequest; }
                else { return Friendship.removeFriend; }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get the requested Friendship: {ex}");
                return Friendship.myself;
            }
        }

        public int GetMessageIdByTimestampAndUser(DateTime timestamp, User thisUser)
        {
            try
            {
                _logger.LogInformation("Get message id through user and timestamp was called");

                return _ctx.Messages
                    .Include(p => p.Sender)
                    .Include(m => m.Receiver)
                    .SingleOrDefault(m => m.Sender.Id == thisUser.Id && m.MessageDate == timestamp)
                    .MessageID;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get the requested Message: {Ex}", ex);
                return 0;
            }
        }

        public IEnumerable<Message> GetAllMessagesOfUser(User thisUser)
        {
            try
            {
                _logger.LogInformation("Get All messages of user was called");

                return _ctx.Messages
                    .Include(p => p.Sender)
                    .Include(m => m.Receiver)
                    .Where(m => m.Sender.Id == thisUser.Id || m.Receiver.Id == thisUser.Id)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get the requested Messages: {Ex}", ex);
                return null;
            }
        }

        public IEnumerable<Message> GetAllMessagesOfUsers(User thisUser, User correspondent)
        {
            try
            {
                _logger.LogInformation("Get All messages of users was called");

                return _ctx.Messages
                    .Include(p => p.Sender)
                    .Include(m => m.Receiver)
                    .Where(m =>
                    (m.Sender.Id == thisUser.Id && m.Receiver.Id == correspondent.Id) ||
                    (m.Sender.Id == correspondent.Id && m.Receiver.Id == thisUser.Id))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get the requested Messages: {Ex}", ex);
                return null;
            }
        }

        public bool ReadAllMessagesFrom(string senderId, User toThisUser)
        {
            var allUnreadMessagesReceived = _ctx.Messages
                    .Include(p => p.Sender)
                    .Include(m => m.Receiver)
                    .Where(m => m.Sender.Id == senderId && m.Receiver.Id == toThisUser.Id && m.isUnread)
                    .ToList();

            foreach (var unreadMessage in allUnreadMessagesReceived)
            {
                unreadMessage.isUnread = false;
            }
            return SaveAll();
        }

        public IEnumerable<Post> GetAllPostsForUser(User currentUser)
        {
            try
            {
                _logger.LogInformation("Get All Posts for User {Name}  was called", currentUser.UserName);
                var friends = (List<User>)GetUsersFriends(currentUser);

                return _ctx.Posts
                           .Include(p => p.User)
                           .Include(o => o.Reactions)
                           .Include(i => i.Comments)
                           .ThenInclude(c=>c.User)
                           .Include(i => i.PostInterests)
                           .ThenInclude(i => i.Interest)
                           .Where(o => friends.Select(x => x.Id).Contains(o.User.Id) || o.PostScope == Scope.Global)
                           .OrderByDescending(p => p.PostDate)
                           .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get all Posts: {Ex}", ex);
                return null;
            }
        }

        public IEnumerable<Post> GetAllPostsForUserByInterest(User currentUser, int interestId)
        {
            try
            {
                _logger.LogInformation("Get All Posts for User {Name} by interest was called", currentUser.UserName);
                _logger.LogInformation("Before the call {Interest}", interestId);
                var allAvailablePosts = (List<Post>)GetAllPostsForUser(currentUser);

                foreach (var everyPost in allAvailablePosts)
                {
                    foreach (var pi in everyPost.PostInterests)
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

        public IEnumerable<Post> GetAllPostsByUser(User currentUser)
        {
            try
            {
                _logger.LogInformation("Get All Posts by User " + currentUser.UserName + " was called");
                return _ctx.Posts
                           .Include(o => o.Reactions)
                           .Include(i => i.Comments)
                           .ThenInclude(c => c.User)
                           .Include(i => i.PostInterests)
                           .ThenInclude(i => i.Interest)
                           .Where(o => o.User.Id == currentUser.Id)
                           .OrderByDescending(p => p.PostDate)
                           .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all Posts: {ex}");
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
                _logger.LogInformation("Get Post by ID: " + postID + " was called");

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

        public async Task<Comment> GetCommentById(int commentID)
        {
            try
            {
                _logger.LogInformation("Get Comment by ID: " + commentID + " was called");

                return await _ctx.Comments
                .SingleOrDefaultAsync(u => u.CommentID.Equals(commentID));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get the requested Comment: {ex}");
                return null;
            }
        }

        public async Task<Comment> GetCommentByDate(DateTime timeStamp)
        {
            try
            {
                _logger.LogInformation("Get Comment by date was called");

                return await _ctx.Comments
                .SingleOrDefaultAsync(u => u.CommentDate.Equals(timeStamp));
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

        public Reaction GetReactionByPostAndUser(int reactionPostId, User thisUser)
        {
            return _ctx.Reactions.Where(r => r.Post.PostID == reactionPostId && r.User == thisUser).FirstOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamProject.Data.Entities;

namespace TeamProject.Data
{
    public interface IDataRepository
    {
        void AddEntity(object model);
        void DeleteEntity(object model);
        bool DeletePost(Post toDelete);
        bool DeleteUser(User toBeDeleted);

        Interest GetInterestById(int InterestId);
        IEnumerable<Interest> GetAllInterests();        
        List<Interest> GetTopUsersInterests(User thisUser);

        IEnumerable<User> GetAllUsersExcept(User thisUser);
        IEnumerable<User> GetAllStrangeUsers(User thisUser);
        IEnumerable<User> GetUsersFriends(User thisUser);

        Friend GetFriend(User thisUser, string friendsID);
        Friendship GetFriendship(User thisUser, User anotherUser);

        int GetMessageIDByTimestampAndUser(DateTime timestamp, User thisUser);
        IEnumerable<Message> GetAllMessagesOfUser(User thisUser);
        IEnumerable<Message> GetAllMessagesOfUsers(User thisUser, User correspondant);
        bool ReadAllMessagesFrom(string senderId, User toThisUser);

        IEnumerable<Post> GetAllPostsForUser(User user);
        IEnumerable<Post> GetAllPostsForUserByInterest(User thisUser, int interestID);
        IEnumerable<Post> GetAllPostsByUser(User user);
        IEnumerable<Post> GetPostsByCategoryInterest(PostInterest interestCategory);

        Post GetPostByTimeStamp(DateTime timestamp);
        Post GetPostById(int postID);

        Task<Comment> GetCommentById(int commentID);
        Task<Comment> GetCommentByDate(DateTime timeStamp);

        Reaction GetReactionByPostAndUser(int reactionPostId, User thisUser);

        bool SaveAll();        
    }
}
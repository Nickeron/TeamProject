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

        IEnumerable<Interest> GetAllInterests();
        List<Interest> GetTopUsersInterests(User thisUser);

        IEnumerable<User> GetAllStrangeUsers(User thisUser);
        IEnumerable<User> GetUsersFriends(User thisUser);

        Friend GetFriend(User thisUser, string friendsID);
        Friendship GetFriendship(User thisUser, User anotherUser);

        IEnumerable<Message> GetAllMessagesOfUser(User thisUser);
        IEnumerable<Message> GetAllMessagesOfUsers(User thisUser, User correspondant);

        IEnumerable<Post> GetAllPostsForUser(User user);
        IEnumerable<Post> GetAllPostsForUserByInterest(User thisUser, int interestID);
        IEnumerable<Post> GetAllPostsByUser(User user);
        IEnumerable<Post> GetPostsByCategoryInterest(PostInterest interestCategory);

        Post GetPostByTimeStamp(DateTime timestamp);
        Post GetPostById(int postID);

        Comment GetCommentById(int commentID);

        Reaction GetReactionByPostAndUser(int reactionPostId, User thisUser);

        bool SaveAll();
    }
}
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

        Interest GetInterestById(int interestId);
        IEnumerable<Interest> GetAllInterests();        
        List<Interest> GetTopUsersInterests(User thisUser);

        IEnumerable<User> GetAllUsersExcept(User thisUser);
        IEnumerable<User> GetAllStrangeUsers(User thisUser);
        IEnumerable<User> GetUsersFriends(User thisUser);

        Friend GetFriend(User thisUser, string friendsId);
        Friendship GetFriendship(User thisUser, User anotherUser);

        int GetMessageIdByTimestampAndUser(DateTime timestamp, User thisUser);
        IEnumerable<Message> GetAllMessagesOfUser(User thisUser);
        IEnumerable<Message> GetAllMessagesOfUsers(User thisUser, User correspondent);
        bool ReadAllMessagesFrom(string senderId, User toThisUser);

        IEnumerable<Post> GetAllPostsForUser(User user);
        IEnumerable<Post> GetAllPostsForUserByInterest(User thisUser, int interestId);
        IEnumerable<Post> GetAllPostsByUser(User user);
        IEnumerable<Post> GetPostsByCategoryInterest(PostInterest interestCategory);

        Post GetPostByTimeStamp(DateTime timestamp);
        Post GetPostById(int postId);

        Task<Comment> GetCommentById(int commentId);
        Task<Comment> GetCommentByDate(DateTime timeStamp);

        Reaction GetReactionByPostAndUser(int reactionPostId, User thisUser);

        bool SaveAll();        
    }
}
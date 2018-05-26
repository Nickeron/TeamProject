using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamProject.Data.Entities;

namespace TeamProject.Data
{
    public interface IDataRepository
    {
        void AddEntity(object model);
        IEnumerable<Interest> GetAllInterests();

        IEnumerable<User> GetAllStrangeUsers(User thisUser);
        IEnumerable<User> GetUsersFriends(User thisUser);

        IEnumerable<Post> GetAllPosts();
        IEnumerable<Post> GetAllPostsForUser(User user);
        IEnumerable<Post> GetAllPostsForUserByInterest(User thisUser, int interestID);
        IEnumerable<Post> GetAllPostsByUser(User user);
        IEnumerable<Post> GetPostsByCategoryInterest(PostInterest interestCategory);

        Post GetPostByTimeStamp(DateTime timestamp);
        Post GetPostById(int postID);

        Reaction GetReactionByPostAndUser(int reactionPostId, User thisUser);


        bool SaveAll();
        
    }
}
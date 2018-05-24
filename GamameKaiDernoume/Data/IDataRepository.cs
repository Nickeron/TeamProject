using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamameKaiDernoume.Data.Entities;

namespace GamameKaiDernoume.Data
{
    public interface IDataRepository
    {
        void AddEntity(object model);
        IEnumerable<Interest> GetAllInterests();
        IEnumerable<User> GetAllStrangeUsers(User thisUser);
        IEnumerable<User> GetUsersFriends(User thisUser);
        IEnumerable<Post> GetAllPosts(bool includeReactions);
        IEnumerable<Post> GetAllPostsByUser(string username, bool includeComments);
        IEnumerable<Post> GetPostsByCategoryInterest(PostInterest interestCategory);
        Post GetPostByTimeStamp(DateTime timestamp);
        Post GetPostById(int postID);
        bool SaveAll();
        
    }
}
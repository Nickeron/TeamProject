using TeamProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TeamProject.Models
{
    public class MyWallViewModel
    {  
        public User ProfileUser { get; set; }
        public User ThisUser { get; set; }
        public List<Post> Posts { get; set; }
        public List<string> TopUserInterests { get; set; }
        public List<Interest> Interests { get; set; }
        public Friendship FriendshipStatus { get; set; }
        
    }

    public class InterestPostViewModel
    {
        [Required]
        public int InterestId { get; set; }
    }

    public class CreatePostViewModel
    {        
        public IFormFile PostImage { get; set; }
        [Required]
        public string PostText { get; set; }
        [Required]
        public int PostScope { get; set; }
        [Required]
        public List<string> Interests { get; set; }
    }

    public class CommentModel {
        [Required]
        public int PostID { get; set; }

        [Required]
        public string CommentText { get; set; }
    }

    public class ReactionModel {
        [Required]
        public int PostID { get; set; }

        [Required]
        public bool IsLike { get; set; }
    }

}

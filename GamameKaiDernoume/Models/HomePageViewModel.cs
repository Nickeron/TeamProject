using TeamProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeamProject.Models
{
    public class MyWallViewModel
    {
        public List<Post> Posts { get; set; }
        public List<Interest> Interests { get; set; }
    }

    public class CreatePostViewModel
    {
        public Scope PostScope { get; set; }
        public string PostImage { get; set; }
        [Required]
        public string PostText { get; set; }

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

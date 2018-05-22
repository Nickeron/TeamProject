using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamameKaiDernoume.Data.Entities
{
    public class Comment
    {
        public int CommentID { get; set; }
        public DateTime CommentDate { get; set; }
        public string CommentText { get; set; }

        public User User { get; set; }
        public Post Post { get; set; }
    }
}

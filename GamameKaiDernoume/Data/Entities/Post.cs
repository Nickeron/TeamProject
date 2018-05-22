using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamameKaiDernoume.Data.Entities
{
    public class Post
    {
        public int PostID { get; set; }
        public DateTime PostDate { get; set; }
        public Scope PostScope { get; set; }
        public string PostImage { get; set; }
        public string PostText { get; set; }

        public User User { get; set; }
        public ICollection<PostInterest> PostInterests { get; set; }
        public virtual ICollection<Reaction> Reactions { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamProject.Data.Entities
{
    public class Reaction
    {
        public int ReactionID { get; set; }
        public bool IsLike { get; set; }

        public Post Post { get; set; }
        public User User { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamameKaiDernoume.Data.Entities
{
    public class PostInterest
    {
        public int PostId { get; set; }
        public Post Post { get; set; }

        public int InterestId { get; set; }
        public Interest Interest { get; set; }
    }
}

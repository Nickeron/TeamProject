using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamameKaiDernoume.Data.Entities
{
    public class Interest
    {
        public int InterestID { get; set; }
        public string InterestCategory { get; set; }
        public string InterestIcon { get; set; }

        public ICollection<PostInterest> PostInterests { get; set; }
    }
}

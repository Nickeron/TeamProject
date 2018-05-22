using GamameKaiDernoume.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamameKaiDernoume.Models
{
    public class CreatePostViewModel
    {
        public Post newPost { get; set; }
        public IEnumerable<Interest> newPostInterests { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamProject.Data.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOdBirth { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public Role UserRole { get; set; }
        public virtual string UserAvatar { get; set; }

        public virtual ICollection<Friend> Friends { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; }
        public virtual ICollection<Message> ReceivedMessages { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamProject.Data.Entities
{
    public class Friend
    {
        public int FriendID { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
        public bool Accept { get; set; }
    }
}

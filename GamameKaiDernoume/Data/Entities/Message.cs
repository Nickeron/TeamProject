using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamProject.Data.Entities
{
    public class Message
    {
        public int MessageID { get; set; }
        public DateTime MessageDate { get; set; }
        public string MessageText { get; set; }

        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}

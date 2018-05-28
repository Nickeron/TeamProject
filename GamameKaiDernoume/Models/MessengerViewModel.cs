using TeamProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamProject.Models
{
    public class MessengerViewModel
    {
        public User ThisUser { get; set; }
        public User LatestCommunicator { get; set; }

        public IEnumerable<Message> UsersMessages { get; set; }
        public IEnumerable<User> ThisUsersFriends { get; set; }
    }

    public class SendMessageModel
    {
        public string ReceiverID { get; set; }
        public string MessageText { get; set; }
    }


}

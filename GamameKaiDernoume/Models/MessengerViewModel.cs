using GamameKaiDernoume.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamameKaiDernoume.Models
{
    public class MessengerViewModel
    {
        public string ThisUserID { get; set; }
        public string UserName { get; set; }
        public IEnumerable<User> ThisUsersFriends { get; set; }
    }

    public class SendMessageModel
    {
        public string ReceiverID { get; set; }
        public string MessageText { get; set; }
    }


}

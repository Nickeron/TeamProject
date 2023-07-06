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
        public int UnreadLatest { get; set; }
        public List<UserChatModel> FriendsAndMessages { get; set; }
    }

    public class UserChatModel
    {
        public User Correspondent { get; set; }
        public int UnreadReceived { get; set; }
        public List<Message> CorrespondentsMessages { get; set; }
    }

    public class SendMessageModel
    {
        public string ReceiverId { get; set; }
        public string MessageText { get; set; }        
    }

    public class NewMessageModel
    {
        public int MessageId { get; set; }
        public string MessageDate { get; set; }        
    }


}

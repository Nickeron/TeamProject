using TeamProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TeamProject.Models
{
    public class FriendViewModel
    {  
        public List<UserModel> OtherUsers { get; set; }
        public List<UserModel> SentRequests { get; set; }
        public List<UserModel> ReceivedRequests { get; set; }
        public User ThisUser { get; set; }
    }

    public class UserModel
    {
        [Required]
        public string ID { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public List<Interest> TopInterests { get; set; }
        public Friendship FriendshipStatus { get; set; }
    }
}

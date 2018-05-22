using GamameKaiDernoume.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamameKaiDernoume.Models
{
    public class FriendRequestViewModel
    {
        public User UserToBeFriend { get; set; }
        public IEnumerable<User> allAvailableFriends { get; set; }
    }
}

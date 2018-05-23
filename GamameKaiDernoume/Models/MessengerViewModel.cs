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
        public IEnumerable<User> ThisUsersFriends { get; set; }
    }
}

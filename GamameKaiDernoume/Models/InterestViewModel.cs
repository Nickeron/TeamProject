using TeamProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TeamProject.Models
{
    public class InterestViewModel
    {  
        public List<Interest> Interests { get; set; }
        public int InterestId { get; set; }
        public string InterestCategory { get; set; }
    }

    public class InterestPostViewModel
    {
        [Required]
        public int InterestId { get; set; }
        public string InterestCategory { get; set; }
    }
}

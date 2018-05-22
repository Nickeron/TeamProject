using GamameKaiDernoume.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GamameKaiDernoume.Models
{
    public class CreatePostViewModel
    {
        public Scope PostScope { get; set; }
        public string PostImage { get; set; }
        [Required]
        public string PostText { get; set; }

        [Required]
        public List<string> Interests { get; set; }
    }
}

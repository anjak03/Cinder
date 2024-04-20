using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cinder.Models 
{
    public class Match {
         [Key]
        public int Id_Match { get; set; }
        public string Id_User1 { get; set; }
        public User User1 { get; set; }
        public string Id_User2 { get; set; }
        public User User2 { get; set; }
        public int points { get; set;} = 0;
        public int User1_Swipe { get; set; } = -1;
        public int User2_Swipe { get; set; } = -1;
        public bool Matched { get; set; } = false;
        public bool Seen { get; set; } = false;
    }
}
using System.ComponentModel.DataAnnotations;

namespace Cinder.Models 
{
    /// <summary>
    /// Represents a match between two users within the application.
    /// </summary>
    public class Match {
         [Key]
        public int Id_Match { get; set; }
        public string Id_User1 { get; set; }
        public User User1 { get; set; }
        public string Id_User2 { get; set; }
        public User User2 { get; set; }
        public double points { get; set;} = 0;
        public int User1_Swipe { get; set; } = -1;

        [Range(-1, 1, ErrorMessage = "Swipe value must be -1, 0, or 1.")]
        public int User2_Swipe { get; set; } = -1;
        public bool Matched { get; set; } = false;
        public bool Seen { get; set; } = false;
    }
}
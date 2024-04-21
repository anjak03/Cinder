using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cinder.Models{
    /// <summary>
    /// Represents a hobby that can be associated with a user.
    /// </summary>
    public class Hobby{

        [Key]
        public int Id_Hobby {get; set;}

        [MaxLength(100)]
        public string? Name {get; set;}
        public virtual ICollection<UserHobby> UserHobbies { get; set; }
    }
}
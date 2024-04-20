using System.ComponentModel.DataAnnotations;

namespace Cinder.Models{
    public class Hobby{

        [Key]
        public int Id_Hobby {get; set;}

        public string? Name {get; set;}

        public virtual ICollection<UserHobby> UserHobbies { get; set; }
    }
}
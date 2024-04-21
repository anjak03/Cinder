using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Cinder.Models {
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool MyBooleanProperty { get; set; } = false;
        public string? Bio { get; set; }
        public int? Age { get; set; }
        public int Id_Faculty { get; set; }
        public Faculty? Faculty { get; set; }
        public int? FacultyYear { get; set; }
        public int? Rating { get; set; } 
        public string? Sex { get; set; }
        public bool? Employed { get; set; } = false;
        public string? Employment { get; set; }
        public bool? Smoker { get; set; }
        public bool? Pets { get; set; }
        public string? ImageURL { get; set; }
        
        [NotMapped]
        public List<int>? Id_Language{ get; set; }
        public virtual ICollection<UserLanguage>? UserLanguages { get; set; }

        [NotMapped]
        public List<int>? Id_Hobby { get; set; }
        public virtual ICollection<UserHobby>? UserHobbies { get; set; }
        public int LeaseDuration { get; set; }
        public Property? Property { get; set; }
        public List<User>? MatchedUsers { get; set; } = new List<User>();
    }
}
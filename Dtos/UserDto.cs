using Cinder.Dtos;
using Cinder.Models;

namespace Cinder.Dtos
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool MyBooleanProperty { get; set; } = false;
        public string? Bio { get; set; }
        public int? Age { get; set; }
        public FacultyDto? Faculty { get; set; }
        public int? FacultyYear { get; set; }
        public int? Rating { get; set; } 
        public string? Sex { get; set; }
        public bool? Employed { get; set; } = false;
        public string? Employment { get; set; }
        public bool? Smoker { get; set; }
        public bool? Pets { get; set; }
        public string? ImageURL { get; set; }
        public int LeaseDuration { get; set; }

        public Property? Property { get; set; }

        public List<User>? MatchedUsers { get; set; } = new List<User>();
    
        public List<LanguageDto>? Languages { get; set; }
        public List<HobbyDto>? Hobbies { get; set; }

    }

}

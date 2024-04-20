using System.ComponentModel.DataAnnotations;

namespace Cinder.Models
{
    public class Language
    {
        [Key]
        public int Id_Language { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<UserLanguage> UserLanguages { get; set; }
    }
}
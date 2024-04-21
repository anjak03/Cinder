using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cinder.Models
{
    /// <summary>
    /// Represents a language that can be spoken by users.
    /// </summary>
    public class Language
    {
        [Key]
        public int Id_Language { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }
        public virtual ICollection<UserLanguage> UserLanguages { get; set; }
    }
}
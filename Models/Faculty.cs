using System.ComponentModel.DataAnnotations;

namespace Cinder.Models{
    /// <summary>
    /// Represents a faculty in the context of an educational institution.
    /// </summary>
    public class Faculty{
        [Key]
        public int Id_Faculty {get; set;}

        [MaxLength(200)]
        public string? Name {get; set;}
    }
}
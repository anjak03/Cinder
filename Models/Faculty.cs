
using System.ComponentModel.DataAnnotations;

namespace Cinder.Models{
    public class Faculty{
        [Key]
        public int Id_Faculty {get; set;}

        public string? Name {get; set;}
    }
}
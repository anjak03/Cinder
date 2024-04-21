using System.ComponentModel.DataAnnotations;

namespace Cinder.Models{
    /// <summary>
    /// Represents a room that is part of a property.
    /// </summary>
    public class Room 
    {
        [Key]
        public int Id_Room { get; set; }
        public int? Id_Property { get; set; }
        public Property? Property { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public int? Price { get; set; }
        public int? Utilities { get; set; }
        public DateTime? MoveInDate { get; set; }
        public DateTime? MoveOutDate { get; set; }
        public string? Furnishings { get; set; }
        public int? SquareMeters { get; set; }
        public bool? Heating { get; set; }
        public bool? Cooling { get; set; }
        public bool? PrivateBathroom { get; set; }
        public bool? PrivateKitchen { get; set; }
        public bool? PrivateBalcony { get; set; }
        public bool? PrivateTerrace { get; set; }
    }
}
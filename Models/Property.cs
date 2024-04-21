using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinder.Models {
    /// <summary>
    /// Represents a property available for rent or sale.
    /// </summary>
    public class Property {
        [Key]
        public int Id_Property { get; set; }
        public string? UserId { get; set; }
        public string? Type { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Neighborhood { get; set; }
        public int? SquareMeters { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public int? Deposit { get; set; }
        public bool? Furnishings { get; set; }
        // public string? Amenities { get; set; }
        public bool? Parking { get; set; }
        public bool? PetsAllowed { get; set; }
        public bool? SmokingAllowed { get; set; }
        public bool? GuestsAllowed { get; set; }
        public bool? Wifi { get; set; }
        public bool? WashingMachine { get; set; }
        // public string? OutDoorSpace { get; set; }
        public int? ClosestPublicTransport { get; set; }
        public int? ClosestGorceryStore { get; set; }
        // public string? NoiseLevel { get; set; }
        public string? HouseRules { get; set; }
        public int? NumberOfBathrooms { get; set; }
        public int? NumberOfBedrooms { get; set; }
        public int? MaxNumberOfTenants { get; set; }
        public List <Room>? Rooms { get; set; } = new List<Room>();
    }
}

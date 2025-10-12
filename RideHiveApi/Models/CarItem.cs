using RideHiveApi.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models
{
    public class CarItem
    {
        [Key]
        public int CarId { get; set; }
        public int OwnerId { get; set; }
        // Basic details
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? Version { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int NumberDoors { get; set; }
        public int NumberSeats { get; set; }
        public int YearProduction { get; set; }
        public int Course { get; set; } // in km
        // Specifications
        public FuelType Fuel { get; set; }
        public float? Consumption { get; set; } // in l/100km
        public DriveTrainLayoutType Drive { get; set; }
        public TransmissionType Transmission { get; set; }
        public BodyType Body { get; set; }
        public float Displacement { get; set; } // in cm3
        public int HorsePower { get; set; } // in hp
        // Important details
        public ConditionType Condition { get; set; }
        public string VinNumber { get; set; } = string.Empty;
        // storing images as byte arrays in database
        public List<CarImageData> CarImages { get; set; } = new List<CarImageData>();

        // Ownership Document - Single document proving ownership
        // storing document as byte array in database        
        [MaxLength(10 * 1024 * 1024, ErrorMessage = "Document size cannot exceed 10MB")]
        public byte[]? OwnershipDocumentData { get; set; }
        
        [MaxLength(50)]
        public string? OwnershipDocumentContentType { get; set; }
    }
}
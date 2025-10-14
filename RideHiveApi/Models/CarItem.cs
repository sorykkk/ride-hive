using RideHiveApi.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models
{
    public class CarItem
    {
        [Key]
        public int CarId { get; set; }
        
        [Required]
        public int OwnerId { get; set; }
        
        // Basic details
        [Required]
        [MaxLength(50)]
        public string Brand { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Model { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? Version { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(30)]
        public string Color { get; set; } = string.Empty;
        
        [Range(2, 6)]
        public int NumberDoors { get; set; }
        
        [Range(2, 9)]
        public int NumberSeats { get; set; }
        
        public int YearProduction { get; set; }
        
        public int Course { get; set; } // in km
        // Specifications
        [Required]
        public FuelType Fuel { get; set; }
        
        [Range(0.1, 50.0)]
        public float? Consumption { get; set; } // in l/100km
        
        [Required]
        public DriveTrainLayoutType Drive { get; set; }
        
        [Required]
        public TransmissionType Transmission { get; set; }
        
        [Required]
        public BodyType Body { get; set; }
        
        [Range(50, 10_000)]
        public float Displacement { get; set; } // in cm3
        
        [Range(10, 2000)]
        public int HorsePower { get; set; } // in hp
        
        // Important details
        [Required]
        public ConditionType Condition { get; set; }
        
        [Required]
        [MaxLength(17)]
        [MinLength(17)]
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
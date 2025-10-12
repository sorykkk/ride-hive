using System.ComponentModel.DataAnnotations;
using RideHiveApi.Models.Validation;
using RideHiveApi.Models.Enums;

namespace RideHiveApi.Models.DataTransferObjects
{
    public class CarCreateDto
    {
        [Required]
        public int OwnerId { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Brand must be between 2 and 50 characters")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Model must be between 1 and 50 characters")]
        public string Model { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Version cannot exceed 50 characters")]
        public string? Version { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Color cannot exceed 30 characters")]
        public string Color { get; set; } = string.Empty;

        [Required]
        [Range(2, 6, ErrorMessage = "Number of doors must be between 2 and 6")]
        public int NumberDoors { get; set; }

        [Required]
        [Range(2, 9, ErrorMessage = "Number of seats must be between 2 and 9")]
        public int NumberSeats { get; set; }

        [Required]
        [CurrentYear(1900)]
        public int YearProduction { get; set; }

        [Required]
        [Range(0, 1_000_000_000, ErrorMessage = "Course must be between 0 and 1 000 000 000 km")]
        public int Course { get; set; }

        [Required]
        [EnumDataType(typeof(FuelType), ErrorMessage = "Invalid fuel type")]
        public string Fuel { get; set; } = string.Empty;

        [Range(0.1, 50.0, ErrorMessage = "Consumption must be between 0.1 and 50 l/100km")]
        public float? Consumption { get; set; }

        [Required]
        [EnumDataType(typeof(DriveTrainLayoutType), ErrorMessage = "Invalid drive train layout type")]
        public string Drive { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(TransmissionType), ErrorMessage = "Invalid transmission type")]
        public string Transmission { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(BodyType), ErrorMessage = "Invalid body type")]
        public string Body { get; set; } = string.Empty;

        [Required]
        [Range(50, 10_000, ErrorMessage = "Displacement must be between 50 and 10,000 cm³")]
        public float Displacement { get; set; }

        [Required]
        [Range(10, 2000, ErrorMessage = "Horse power must be between 10 and 2,000 hp")]
        public int HorsePower { get; set; }

        [Required]
        [EnumDataType(typeof(ConditionType), ErrorMessage = "Invalid condition type")]
        public string Condition { get; set; } = string.Empty;

        [Required(ErrorMessage = "VIN number is required")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "VIN number must be exactly 17 characters")]
        [RegularExpression(@"^[A-HJ-NPR-Z0-9]{17}$", ErrorMessage = "Invalid VIN format")]
        public string VinNumber { get; set; } = string.Empty;

        // Ownership document upload
        [Required(ErrorMessage = "Ownership document is required")]
        public IFormFile OwnershipDocument { get; set; } = null!;

        // Car images upload (optional during creation)
        [Required]
        public List<IFormFile> CarImages { get; set; } = new List<IFormFile>();
    }
}